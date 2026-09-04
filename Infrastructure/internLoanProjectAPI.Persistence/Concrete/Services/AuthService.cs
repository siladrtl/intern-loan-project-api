using internLoanProject.Domain.Entities;
using internLoanProject.Domain.Entities.Enums;
using internLoanProject.Domain.Entities.Identity;
using internLoanProjectAPI.Application.Abstractions.Services;
using internLoanProjectAPI.Application.Abstractions.UnitOfWorks;
using internLoanProjectAPI.Application.DTOs.Auth;
using internLoanProjectAPI.Persistence.Contexts;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace internLoanProjectAPI.Persistence.Concrete.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _configuration;
        private readonly internLoanProjectAPIDbContext _context;
        private readonly IFileStorageService _fileStorageService;

        public AuthService(UserManager<AppUser> userManager, IUnitOfWork unitOfWork, IConfiguration configuration, internLoanProjectAPIDbContext context, IFileStorageService fileStorageService)
        {
            _userManager = userManager;
            _unitOfWork = unitOfWork;
            _configuration = configuration;
            _context = context;
            _fileStorageService = fileStorageService;
        }

        public async Task<AuthResponseDto> RegisterAsync(RegisterRequestDto request,VerificationDocumentDto verificationDocument)
        {
            var email = request.Email.Trim();var nationalId = request.NationalId.Trim();
     
            var existingUser = await _userManager.FindByEmailAsync(email);
            if (existingUser != null)
            {
                throw new Exception("Bu e-posta adresi zaten kayıtlı.");
            }

            var existingCustomer = await _unitOfWork
                .GetReadRepository<Customer>()
                .GetSingleAsync(x => x.NationalId == nationalId, false);

            if (existingCustomer != null)
            {
                throw new Exception("Bu TC Kimlik Numarası ile daha önce kayıt oluşturulmuş.");
            }

            await using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var customer = new Customer
                {
                    FirstName = request.FirstName.Trim(),
                    LastName = request.LastName.Trim(),
                    BirthDate = request.BirthDate,
                    NationalId = nationalId,
                    Email = email,
                    PhoneNumber = request.PhoneNumber.Trim(),
                    City = request.City.Trim(),
                    District = request.District.Trim(),
                    CustomerType = request.CustomerType
                };

                var customerResult = await _unitOfWork
                    .GetWriteRepository<Customer>()
                    .AddAsync(customer);

                if (!customerResult)
                {
                    throw new Exception("Müşteri kaydı oluşturulamadı.");
                }

                await _unitOfWork.SaveAsync();

                var filePath = await _fileStorageService.SaveAsync(
                    verificationDocument.FileStream,
                    verificationDocument.FileName,
                    verificationDocument.ContentType
                );

                var verificationDocumentEntity =
                    new CustomerVerificationDocument
                    {
                        CustomerId = customer.Id,
                        OriginalFileName = verificationDocument.FileName,
                        StoredFileName = Path.GetFileName(filePath),
                        ContentType = verificationDocument.ContentType,
                        FileSize = verificationDocument.FileSize,
                        FilePath = filePath,
                        Status = VerificationStatus.Pending,
                        UploadedAt = DateTime.Now

                    };

                var documentResult = await _unitOfWork
                        .GetWriteRepository<CustomerVerificationDocument>()
                        .AddAsync(verificationDocumentEntity);

                if (!documentResult)
                {
                    throw new Exception(
                        "Müşteri doğrulama belgesi kaydedilemedi."
                    );
                }

                await _unitOfWork.SaveAsync();

                var user = new AppUser
                {
                    UserName = email,
                    Email = email,
                    CustomerId = customer.Id
                };

                var identityResult = await _userManager.CreateAsync(user, request.Password);

                if (!identityResult.Succeeded)
                {
                    var errors = string.Join(", ", identityResult.Errors.Select(x => x.Description));
                    throw new Exception(errors);
                }

                var roleResult = await _userManager.AddToRoleAsync(user, "Customer");

                if (!roleResult.Succeeded)
                {
                    var errors = string.Join(", ", roleResult.Errors.Select(x => x.Description));
                    throw new Exception(errors);
                }

                await transaction.CommitAsync();

                var token = await GenerateTokenAsync(user);

                return new AuthResponseDto
                {
                    Token = token,
                    Email = user.Email!,
                    CustomerId = customer.Id
                };
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<AuthResponseDto> LoginAsync(LoginRequestDto request)
        {
            var email = request.Email.Trim();

            var user = await _userManager.FindByEmailAsync(email);

            if (user == null)
            {
                throw new Exception("Email veya şifre hatalı.");
            }

            var passwordValid = await _userManager.CheckPasswordAsync(user, request.Password);

            if (!passwordValid)
            {
                throw new Exception("Email veya şifre hatalı.");
            }

            var roles = await _userManager.GetRolesAsync(user);

            if (roles.Contains("Customer") && user.CustomerId == null)
            {
                throw new Exception("Kullanıcıya bağlı müşteri kaydı bulunamadı.");
            }

            var token = await GenerateTokenAsync(user);

            return new AuthResponseDto
            {
                Token = token,
                Email = user.Email!,
                CustomerId = user.CustomerId
            };
        }

        private async Task<string> GenerateTokenAsync(AppUser user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email ?? string.Empty)
            };

            if (user.CustomerId != null)
            {
                claims.Add(new Claim("CustomerId", user.CustomerId.Value.ToString()));

                var customer = await _unitOfWork
                    .GetReadRepository<Customer>()
                    .GetSingleAsync(x => x.Id == user.CustomerId.Value, false);

                if (customer != null)
                {
                    claims.Add(new Claim("FirstName", customer.FirstName));
                    claims.Add(new Claim("LastName", customer.LastName));
                }
            }

            var roles = await _userManager.GetRolesAsync(user);

            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var jwtKey = _configuration["Jwt:Key"];
            if (string.IsNullOrWhiteSpace(jwtKey))
            {
                throw new Exception("Jwt:Key appsettings.json içerisinde bulunamadı.");
            }

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}