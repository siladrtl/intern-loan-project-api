using internLoanProject.Domain.Entities;
using internLoanProjectAPI.Application.Abstractions.Services;
using internLoanProjectAPI.Application.Abstractions.UnitOfWorks;
using internLoanProjectAPI.Application.DTOs.Application;
using internLoanProjectAPI.Persistence.Contexts;

using Microsoft.AspNetCore.Http;

using System.Security.Claims;

namespace internLoanProjectAPI.Persistence.Concrete.Services
{
    public class LoanApplicationService : ILoanApplicationService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly internLoanProjectAPIDbContext _context;


        public LoanApplicationService(
            IUnitOfWork unitOfWork,
            IHttpContextAccessor httpContextAccessor,
            internLoanProjectAPIDbContext context)
        {
            _unitOfWork = unitOfWork;
            _httpContextAccessor = httpContextAccessor;
            _context = context;
        }


        public async Task<LoanApplicationDto> CreateAsync(
            CreateLoanApplicationDto dto)
        {
            // ==========================================
            // JWT'DEN APP USER ID
            // ==========================================

            var userIdClaim =
                _httpContextAccessor
                    .HttpContext?
                    .User
                    .FindFirst(ClaimTypes.NameIdentifier);


            if (userIdClaim == null)
            {
                throw new Exception(
                    "Kullanıcı kimliği bulunamadı.");
            }


            // AppUser.Id hala Guid
            if (!Guid.TryParse(
                userIdClaim.Value,
                out Guid userId))
            {
                throw new Exception(
                    "Geçersiz kullanıcı kimliği.");
            }


            // ==========================================
            // APP USER
            // ==========================================

            var user =
                await _context.Users.FindAsync(userId);


            if (user == null)
            {
                throw new Exception(
                    "Kullanıcı bulunamadı.");
            }


            if (user.CustomerId == null)
            {
                throw new Exception(
                    "Kullanıcıya bağlı müşteri kaydı bulunamadı.");
            }


            var customerId =
                user.CustomerId.Value;


            // ==========================================
            // CUSTOMER
            // ==========================================

            var customer =
                await _unitOfWork
                    .GetReadRepository<Customer>()
                    .GetSingleAsync(
                        x => x.Id == customerId,
                        false);


            if (customer == null)
            {
                throw new Exception(
                    "Müşteri kaydı bulunamadı.");
            }


            // ==========================================
            // LOAN PRODUCT
            // ==========================================

            var loanProduct =
                await _unitOfWork
                    .GetReadRepository<LoanProduct>()
                    .GetSingleAsync(
                        x =>
                            x.Id == dto.LoanProductId &&
                            x.IsActive,
                        false);


            if (loanProduct == null)
            {
                throw new Exception(
                    "Kredi ürünü bulunamadı.");
            }


            // ==========================================
            // MÜŞTERİ TİPİ UYGUNLUK KONTROLÜ
            // ==========================================

            if (loanProduct.CustomerType !=
                customer.CustomerType)
            {
                throw new Exception(
                    "Seçilen kredi ürünü müşteri tipinize uygun değildir.");
            }


            // ==========================================
            // LOAN CALCULATION
            // ==========================================

            var calculation =
                await _unitOfWork
                    .GetReadRepository<LoanCalculation>()
                    .GetSingleAsync(
                        x =>
                            x.Id == dto.LoanCalculationId,
                        false);


            if (calculation == null)
            {
                throw new Exception(
                    "Kredi hesaplaması bulunamadı.");
            }


            // Hesaplama seçilen ürüne ait mi?
            if (calculation.LoanProductId !=
                dto.LoanProductId)
            {
                throw new Exception(
                    "Kredi hesaplaması seçilen ürünle eşleşmiyor.");
            }


            // ==========================================
            // LOAN APPLICATION
            // ==========================================

            var application =
                new LoanApplication
                {
                    // Id YOK.
                    // Database int ID'yi kendisi oluşturacak.

                    CustomerId =
                        customerId,

                    LoanProductId =
                        dto.LoanProductId,

                    LoanCalculationId =
                        dto.LoanCalculationId,

                    Status =
                        "Pending"
                };


            var result =
                await _unitOfWork
                    .GetWriteRepository<LoanApplication>()
                    .AddAsync(application);


            if (!result)
            {
                throw new Exception(
                    "Kredi başvurusu oluşturulamadı.");
            }


            await _unitOfWork.SaveAsync();


            // ==========================================
            // RESPONSE
            // ==========================================

            return new LoanApplicationDto
            {
                Id =
                    application.Id,

                CustomerId =
                    application.CustomerId,

                LoanProductId =
                    application.LoanProductId,

                LoanCalculationId =
                    application.LoanCalculationId,

                Status =
                    application.Status
            };
        }
    }
}