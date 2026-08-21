using internLoanProject.Domain.Entities;
using internLoanProject.Domain.Entities.Identity;
using internLoanProjectAPI.Application.Abstractions.Services;
using internLoanProjectAPI.Application.Abstractions.UnitOfWorks;
using internLoanProjectAPI.Application.DTOs.Auth;

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


        public AuthService(
            UserManager<AppUser> userManager,
            IUnitOfWork unitOfWork,
            IConfiguration configuration)
        {
            _userManager = userManager;
            _unitOfWork = unitOfWork;
            _configuration = configuration;
        }


        // ==========================================
        // REGISTER
        // ==========================================

        public async Task<AuthResponseDto> RegisterAsync(
            RegisterRequestDto request)
        {
            // Email daha önce kayıtlı mı?
            var existingUser =
                await _userManager.FindByEmailAsync(request.Email);

            if (existingUser != null)
            {
                throw new Exception(
                    "Bu email adresi zaten kayıtlı.");
            }


            // Customer oluştur
            var customer = new Customer
            {
                FirstName = request.FirstName,

                LastName = request.LastName,

                BirthDate = request.BirthDate,

                NationalId = request.NationalId,

                Email = request.Email,

                PhoneNumber = request.PhoneNumber,

                City = request.City,

                District = request.District,

                CustomerType = request.CustomerType
            };


            await _unitOfWork
                .GetWriteRepository<Customer>()
                .AddAsync(customer);


            // ÖNEMLİ:
            // Customer.Id artık int ve DB tarafından üretilecek.
            // Bu yüzden önce Customer'ı kaydediyoruz.
            await _unitOfWork.SaveAsync();


            // Customer kaydedildikten sonra
            // customer.Id artık 1, 2, 3... şeklinde oluşmuş olacak.


            // AppUser oluştur
            var user = new AppUser
            {
                Id = Guid.NewGuid(),

                UserName = request.Email,

                Email = request.Email,

                CustomerId = customer.Id
            };


            var result =
                await _userManager.CreateAsync(
                    user,
                    request.Password);


            if (!result.Succeeded)
            {
                var errors = string.Join(
                    ", ",
                    result.Errors.Select(
                        x => x.Description));

                throw new Exception(errors);
            }


            // JWT oluştur
            var token =
                GenerateToken(user);


            return new AuthResponseDto
            {
                Token = token,

                Email = user.Email!,

                CustomerId = customer.Id
            };
        }


        // ==========================================
        // LOGIN
        // ==========================================

        public async Task<AuthResponseDto> LoginAsync(
            LoginRequestDto request)
        {
            var user =
                await _userManager.FindByEmailAsync(
                    request.Email);


            if (user == null)
            {
                throw new Exception(
                    "Email veya şifre hatalı.");
            }


            var passwordValid =
                await _userManager.CheckPasswordAsync(
                    user,
                    request.Password);


            if (!passwordValid)
            {
                throw new Exception(
                    "Email veya şifre hatalı.");
            }


            if (user.CustomerId == null)
            {
                throw new Exception(
                    "Kullanıcıya bağlı müşteri kaydı bulunamadı.");
            }


            var token =
                GenerateToken(user);


            return new AuthResponseDto
            {
                Token = token,

                Email = user.Email!,

                CustomerId =
                    user.CustomerId.Value
            };
        }


        // ==========================================
        // JWT OLUŞTUR
        // ==========================================

        private string GenerateToken(
            AppUser user)
        {
            var claims =
                new List<Claim>
                {
                    new Claim(
                        ClaimTypes.NameIdentifier,
                        user.Id.ToString()),

                    new Claim(
                        ClaimTypes.Email,
                        user.Email ??
                        string.Empty),

                    new Claim(
                        "CustomerId",
                        user.CustomerId?.ToString()
                        ?? string.Empty)
                };


            var jwtKey =
                _configuration["Jwt:Key"];


            if (string.IsNullOrEmpty(jwtKey))
            {
                throw new Exception(
                    "Jwt:Key appsettings.json içerisinde bulunamadı.");
            }


            var securityKey =
                new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(
                        jwtKey));


            var credentials =
                new SigningCredentials(
                    securityKey,
                    SecurityAlgorithms.HmacSha256);


            var token =
                new JwtSecurityToken(
                    claims: claims,

                    expires:
                        DateTime.UtcNow.AddHours(2),

                    signingCredentials:
                        credentials
                );


            return new JwtSecurityTokenHandler()
                .WriteToken(token);
        }
    }
}