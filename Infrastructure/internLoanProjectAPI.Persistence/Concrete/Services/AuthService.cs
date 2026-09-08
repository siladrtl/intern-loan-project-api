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

        public async Task<AuthResponseDto> RegisterAsync(
            RegisterRequestDto request,
            VerificationDocumentDto verificationDocument)
        {
            var email = request.Email.Trim();
            var nationalId = request.NationalId.Trim();

            var existingUser = await _userManager.FindByEmailAsync(email);
            if (existingUser != null)
            {
                throw new InvalidOperationException("Bu e-posta adresi zaten kayıtlı.");
            }

            var existingCustomer = await _unitOfWork
                .GetReadRepository<Customer>()
                .GetSingleAsync(x => x.NationalId == nationalId, false);

            if (existingCustomer != null)
            {
                throw new InvalidOperationException("Bu TC Kimlik Numarası ile daha önce müşteri kaydı oluşturulmuş.");
            }

            var existingRegistration = await _unitOfWork
                .GetReadRepository<CustomerRegistration>()
                .GetSingleAsync(x => x.NationalId == nationalId || x.Email == email, false);

            if (existingRegistration != null)
            {
                throw new InvalidOperationException("Bu bilgilerle daha önce müşteri olma başvurusu oluşturulmuş.");
            }

            await using var transaction = await _context.Database.BeginTransactionAsync();
            string? savedFilePath = null;

            try
            {
                var registration = new CustomerRegistration
                {
                    FirstName = request.FirstName.Trim(),
                    LastName = request.LastName.Trim(),
                    BirthDate = request.BirthDate,
                    NationalId = nationalId,
                    Email = email,
                    PhoneNumber = request.PhoneNumber.Trim(),
                    City = request.City.Trim(),
                    District = request.District.Trim(),
                    CustomerType = request.CustomerType,
                    Status = VerificationStatus.Pending,
                    CreatedAt = DateTime.Now
                };

                var registrationResult = await _unitOfWork
                    .GetWriteRepository<CustomerRegistration>()
                    .AddAsync(registration);

                if (!registrationResult)
                {
                    throw new InvalidOperationException("Müşteri olma başvurusu oluşturulamadı.");
                }

                await _unitOfWork.SaveAsync();

                savedFilePath = await _fileStorageService.SaveAsync(
                    verificationDocument.FileStream,
                    verificationDocument.FileName,
                    verificationDocument.ContentType);

                var verificationDocumentEntity = new CustomerVerificationDocument
                {
                    CustomerRegistrationId = registration.Id,
                    OriginalFileName = verificationDocument.FileName,
                    StoredFileName = Path.GetFileName(savedFilePath),
                    ContentType = verificationDocument.ContentType,
                    FileSize = verificationDocument.FileSize,
                    FilePath = savedFilePath,
                    Status = VerificationStatus.Pending,
                    UploadedAt = DateTime.Now
                };

                var documentResult = await _unitOfWork
                    .GetWriteRepository<CustomerVerificationDocument>()
                    .AddAsync(verificationDocumentEntity);

                if (!documentResult)
                {
                    throw new InvalidOperationException("Müşteri doğrulama belgesi kaydedilemedi.");
                }

                await _unitOfWork.SaveAsync();

                var user = new AppUser
                {
                    UserName = email,
                    Email = email,
                    CustomerId = null
                };

                var identityResult = await _userManager.CreateAsync(user, request.Password);

                if (!identityResult.Succeeded)
                {
                    var errors = string.Join(", ", identityResult.Errors.Select(x => x.Description));
                    throw new InvalidOperationException(errors);
                }

                await transaction.CommitAsync();

                return new AuthResponseDto
                {
                    Token = string.Empty,
                    Email = user.Email!,
                    CustomerId = null
                };
            }
            catch
            {
                await transaction.RollbackAsync();

                if (!string.IsNullOrWhiteSpace(savedFilePath))
                {
                    await _fileStorageService.DeleteAsync(savedFilePath);
                }

                throw;
            }
        }

        public async Task<AuthResponseDto> LoginAsync(LoginRequestDto request)
        {
            var email = request.Email.Trim();

            var user = await _userManager.FindByEmailAsync(email);

            if (user == null)
            {
                throw new UnauthorizedAccessException(
                    "Email veya şifre hatalı."
                );
            }

            var passwordValid = await _userManager
                .CheckPasswordAsync(user, request.Password);

            if (!passwordValid)
            {
                throw new UnauthorizedAccessException(
                    "Email veya şifre hatalı."
                );
            }

            var roles = await _userManager.GetRolesAsync(user);

            if (roles.Contains("Admin"))
            {
                var adminToken = await GenerateTokenAsync(user);

                return new AuthResponseDto
                {
                    Token = adminToken,
                    Email = user.Email!,
                    CustomerId = null
                };
            }

            if (user.CustomerId == null)
            {
                var registration = await _unitOfWork
                    .GetReadRepository<CustomerRegistration>()
                    .GetSingleAsync(
                        x => x.Email == email,
                        false
                    );

                if (registration != null)
                {
                    if (registration.Status == VerificationStatus.Pending)
                    {
                        throw new InvalidOperationException(
                            "Müşteri olma başvurunuz henüz admin tarafından onaylanmadı."
                        );
                    }

                    if (registration.Status == VerificationStatus.Rejected)
                    {
                        throw new InvalidOperationException(
                            "Müşteri olma başvurunuz reddedildi."
                        );
                    }
                }

                throw new InvalidOperationException(
                    "Kullanıcıya bağlı müşteri kaydı bulunamadı."
                );
            }
            if (!roles.Contains("Customer"))
            {
                throw new InvalidOperationException(
                    "Kullanıcının müşteri yetkisi bulunamadı."
                );
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
                throw new InvalidOperationException("Jwt:Key appsettings.json içerisinde bulunamadı.");
            }

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}