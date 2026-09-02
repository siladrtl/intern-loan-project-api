using internLoanProject.Domain.Entities;
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


        public AuthService(UserManager<AppUser> userManager, IUnitOfWork unitOfWork, IConfiguration configuration, internLoanProjectAPIDbContext context)
        {
            _userManager = userManager;

            _unitOfWork = unitOfWork;

            _configuration = configuration;

            _context = context;
        }

        // Register
        public async Task<AuthResponseDto> RegisterAsync(RegisterRequestDto request)
        {

            var email = request.Email.Trim();

            var nationalId = request.NationalId.Trim();

            var existingUser = await _userManager.FindByEmailAsync(email);


            if (existingUser != null)
            {
                throw new Exception("Bu e-posta adresi zaten kayıtlı.");

            }

            var existingCustomer = await _unitOfWork
                    .GetReadRepository<Customer>()
                    .GetSingleAsync(
                        x =>
                            x.NationalId ==
                            nationalId,
                        false
                    );


            if (existingCustomer != null)
            {
                throw new Exception(
                    "Bu TC Kimlik Numarası ile daha önce kayıt oluşturulmuş."
                );
            }


            // ======================================
            // TRANSACTION
            // ======================================

            await using var transaction =
                await _context.Database
                    .BeginTransactionAsync();


            try
            {
                // ==================================
                // CUSTOMER OLUŞTUR
                // ==================================

                var customer =
                    new Customer
                    {
                        FirstName = request.FirstName.Trim(),

                        LastName = request.LastName.Trim(),

                        BirthDate = request.BirthDate,

                        NationalId = nationalId,

                        Email =
                            email,

                        PhoneNumber =
                            request.PhoneNumber.Trim(),

                        City =
                            request.City.Trim(),

                        District =
                            request.District.Trim(),

                        CustomerType =
                            request.CustomerType
                    };


                var customerResult =
                    await _unitOfWork
                        .GetWriteRepository<Customer>()
                        .AddAsync(
                            customer
                        );


                if (!customerResult)
                {
                    throw new Exception(
                        "Müşteri kaydı oluşturulamadı."
                    );
                }


                await _unitOfWork
                    .SaveAsync();


                // ==================================
                // APP USER OLUŞTUR
                // ==================================

                var user =
                    new AppUser
                    {
                        UserName =
                            email,

                        Email =
                            email,

                        CustomerId =
                            customer.Id
                    };


                var identityResult =
                    await _userManager
                        .CreateAsync(
                            user,
                            request.Password
                        );


                if (!identityResult.Succeeded)
                {
                    var errors =
                        string.Join(
                            ", ",
                            identityResult.Errors
                                .Select(
                                    x =>
                                        x.Description
                                )
                        );


                    throw new Exception(
                        errors
                    );
                }


                // ==================================
                // CUSTOMER ROLÜ VER
                // ==================================

                var roleResult =
                    await _userManager
                        .AddToRoleAsync(
                            user,
                            "Customer"
                        );


                if (!roleResult.Succeeded)
                {
                    var errors =
                        string.Join(
                            ", ",
                            roleResult.Errors
                                .Select(
                                    x =>
                                        x.Description
                                )
                        );


                    throw new Exception(
                        errors
                    );
                }


                // ==================================
                // HER ŞEY BAŞARILI → COMMIT
                // ==================================

                await transaction
                    .CommitAsync();


                // ==================================
                // JWT
                // ==================================

                var token =
                    await GenerateTokenAsync(
                        user
                    );


                // ==================================
                // RESPONSE
                // ==================================

                return new AuthResponseDto
                {
                    Token =
                        token,

                    Email =
                        user.Email!,

                    CustomerId =
                        customer.Id
                };
            }
            catch
            {
                // ==================================
                // HATA → ROLLBACK
                // ==================================

                await transaction
                    .RollbackAsync();


                throw;
            }
        }


        // ==========================================
        // LOGIN
        // ==========================================

        public async Task<AuthResponseDto> LoginAsync(
            LoginRequestDto request)
        {
            var email =
                request.Email.Trim();


            // ======================================
            // USER BUL
            // ======================================

            var user =
                await _userManager
                    .FindByEmailAsync(
                        email
                    );


            if (user == null)
            {
                throw new Exception(
                    "Email veya şifre hatalı."
                );
            }


            // ======================================
            // ŞİFRE KONTROLÜ
            // ======================================

            var passwordValid =
                await _userManager
                    .CheckPasswordAsync(
                        user,
                        request.Password
                    );


            if (!passwordValid)
            {
                throw new Exception(
                    "Email veya şifre hatalı."
                );
            }


            // ======================================
            // KULLANICI ROLLERİNİ AL
            // ======================================

            var roles =
                await _userManager
                    .GetRolesAsync(
                        user
                    );


            // ======================================
            // CUSTOMER İSE CUSTOMER ID ZORUNLU
            // ADMIN İÇİN CUSTOMER ID GEREKMEZ
            // ======================================

            if (
                roles.Contains("Customer") &&
                user.CustomerId == null
            )
            {
                throw new Exception(
                    "Kullanıcıya bağlı müşteri kaydı bulunamadı."
                );
            }


            // ======================================
            // JWT
            // ======================================

            var token =
                await GenerateTokenAsync(
                    user
                );


            // ======================================
            // RESPONSE
            // ======================================

            return new AuthResponseDto
            {
                Token =
                    token,

                Email =
                    user.Email!,

                CustomerId =
                    user.CustomerId
            };
        }


        // ==========================================
        // JWT OLUŞTUR
        // ==========================================

        private async Task<string> GenerateTokenAsync(
     AppUser user)
        {
            // ======================================
            // TEMEL CLAIMLER
            // ======================================

            var claims =
                new List<Claim>
                {
            new Claim(
                ClaimTypes.NameIdentifier,
                user.Id.ToString()
            ),

            new Claim(
                ClaimTypes.Email,
                user.Email ??
                string.Empty
            )
                };


            // ======================================
            // CUSTOMER BİLGİLERİ
            // ======================================

            if (user.CustomerId != null)
            {
                // CustomerId
                claims.Add(
                    new Claim(
                        "CustomerId",
                        user.CustomerId.Value
                            .ToString()
                    )
                );


                // Customer kaydını getir
                var customer =
                    await _unitOfWork
                        .GetReadRepository<Customer>()
                        .GetSingleAsync(
                            x =>
                                x.Id ==
                                user.CustomerId.Value,
                            false
                        );


                if (customer != null)
                {
                    // Ad
                    claims.Add(
                        new Claim(
                            "FirstName",
                            customer.FirstName
                        )
                    );


                    // Soyad
                    claims.Add(
                        new Claim(
                            "LastName",
                            customer.LastName
                        )
                    );
                }
            }


            // ======================================
            // KULLANICI ROLLERİNİ AL
            // ======================================

            var roles =
                await _userManager
                    .GetRolesAsync(
                        user
                    );


            // ======================================
            // ROLLERİ TOKEN'A EKLE
            // ======================================

            foreach (var role in roles)
            {
                claims.Add(
                    new Claim(
                        ClaimTypes.Role,
                        role
                    )
                );
            }


            // ======================================
            // JWT KEY
            // ======================================

            var jwtKey =
                _configuration[
                    "Jwt:Key"
                ];


            if (
                string.IsNullOrWhiteSpace(
                    jwtKey
                )
            )
            {
                throw new Exception(
                    "Jwt:Key appsettings.json içerisinde bulunamadı."
                );
            }


            // ======================================
            // SECURITY KEY
            // ======================================

            var securityKey =
                new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(
                        jwtKey
                    )
                );


            // ======================================
            // SIGNING
            // ======================================

            var credentials =
                new SigningCredentials(
                    securityKey,
                    SecurityAlgorithms.HmacSha256
                );


            // ======================================
            // TOKEN
            // ======================================

            var token =
                new JwtSecurityToken(
                    claims:
                        claims,

                    expires:
                        DateTime.Now
                            .AddHours(2),

                    signingCredentials:
                        credentials
                );


            return new JwtSecurityTokenHandler()
                .WriteToken(
                    token
                );
        }
    }
}