using internLoanProject.Domain.Entities;
using internLoanProject.Domain.Entities.Enums;
using internLoanProjectAPI.Application.Abstractions.Services;
using internLoanProjectAPI.Application.Abstractions.UnitOfWorks;
using internLoanProjectAPI.Application.DTOs.Application;
using internLoanProjectAPI.Persistence.Contexts;

using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
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

            _httpContextAccessor =
                httpContextAccessor;

            _context =
                context;
        }

        // BAŞVURU OLUŞTUR
        // ==========================================

        public async Task<LoanApplicationDto> CreateAsync(
            CreateLoanApplicationDto dto)
        {
            // ======================================
            // JWT'DEN APP USER ID AL
            // ======================================

            var userIdClaim =
                _httpContextAccessor
                    .HttpContext?
                    .User
                    .FindFirst(
                        ClaimTypes.NameIdentifier
                    );


            if (userIdClaim == null)
            {
                throw new Exception(
                    "Kullanıcı kimliği bulunamadı."
                );
            }


            // AppUser Id Guid olduğu için
            // claim içerisindeki değeri Guid'e çeviriyoruz.

            if (
                !Guid.TryParse(
                    userIdClaim.Value,
                    out Guid userId
                )
            )
            {
                throw new Exception(
                    "Geçersiz kullanıcı kimliği."
                );
            }


            // ======================================
            // APP USER BUL
            // ======================================

            var user =
                await _context.Users
                    .FindAsync(
                        userId
                    );


            if (user == null)
            {
                throw new Exception(
                    "Kullanıcı bulunamadı."
                );
            }


            if (user.CustomerId == null)
            {
                throw new Exception(
                    "Kullanıcıya bağlı müşteri kaydı bulunamadı."
                );
            }


            var customerId =
                user.CustomerId.Value;


            // ======================================
            // CUSTOMER BUL
            // ======================================

            var customer =
                await _unitOfWork
                    .GetReadRepository<Customer>()
                    .GetSingleAsync(
                        x =>
                            x.Id ==
                            customerId,
                        false
                    );


            if (customer == null)
            {
                throw new Exception(
                    "Müşteri kaydı bulunamadı."
                );
            }


            // ======================================
            // KREDİ ÜRÜNÜNÜ BUL
            // ======================================

            var loanProduct =
                await _unitOfWork
                    .GetReadRepository<LoanProduct>()
                    .GetSingleAsync(
                        x =>
                            x.Id ==
                            dto.LoanProductId
                            &&
                            x.IsActive,
                        false
                    );


            if (loanProduct == null)
            {
                throw new Exception(
                    "Aktif kredi ürünü bulunamadı."
                );
            }


            // ======================================
            // MÜŞTERİ TİPİ UYGUN MU?
            // ======================================

            if (
                loanProduct.CustomerType !=
                customer.CustomerType
            )
            {
                throw new Exception(
                    "Seçilen kredi ürünü müşteri tipinize uygun değildir."
                );
            }


            // ======================================
            // KREDİ HESAPLAMASINI BUL
            // ======================================

            var calculation =
                await _unitOfWork
                    .GetReadRepository<LoanCalculation>()
                    .GetSingleAsync(
                        x =>
                            x.Id ==
                            dto.LoanCalculationId,
                        false
                    );


            if (calculation == null)
            {
                throw new Exception(
                    "Kredi hesaplaması bulunamadı."
                );
            }


            // ======================================
            // HESAPLAMA BU ÜRÜNE Mİ AİT?
            // ======================================

            if (
                calculation.LoanProductId !=
                dto.LoanProductId
            )
            {
                throw new Exception(
                    "Kredi hesaplaması seçilen ürünle eşleşmiyor."
                );
            }


            // ======================================
            // LOAN APPLICATION OLUŞTUR
            // ======================================

            var application =
                new LoanApplication
                {
                    CustomerId =
                        customerId,

                    LoanProductId =
                        dto.LoanProductId,

                    LoanCalculationId =
                        dto.LoanCalculationId,


                    // Yeni oluşturulan bütün başvurular
                    // ilk olarak Pending durumundadır.
                    Status =
                        LoanApplicationStatus.Pending,


                    // Başvurunun oluşturulduğu tarih
                    ApplicationDate =
                        DateTime.UtcNow,


                    // Admin henüz karar vermedi.
                    DecisionDate =
                        null,


                    // Admin henüz açıklama girmedi.
                    DecisionNote =
                        null
                };


            // ======================================
            // DATABASE'E EKLE
            // ======================================

            var result =
                await _unitOfWork
                    .GetWriteRepository<LoanApplication>()
                    .AddAsync(
                        application
                    );


            if (!result)
            {
                throw new Exception(
                    "Kredi başvurusu oluşturulamadı."
                );
            }


            await _unitOfWork
                .SaveAsync();


            // ======================================
            // RESPONSE
            // ======================================

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
                    application.Status,

                ApplicationDate =
                    application.ApplicationDate,

                DecisionDate =
                    application.DecisionDate,

                DecisionNote =
                    application.DecisionNote
            };

        }
        public async Task<List<LoanApplicationDto>> GetMyApplicationsAsync()
        {
            // ==========================================
            // JWT'DEN USER ID
            // ==========================================

            var userIdClaim =
                _httpContextAccessor
                    .HttpContext?
                    .User
                    .FindFirst(ClaimTypes.NameIdentifier);


            if (userIdClaim == null)
            {
                throw new Exception(
                    "Kullanıcı kimliği bulunamadı."
                );
            }


            if (!Guid.TryParse(
                userIdClaim.Value,
                out Guid userId))
            {
                throw new Exception(
                    "Geçersiz kullanıcı kimliği."
                );
            }


            var user =
                await _context.Users
                    .FindAsync(userId);


            if (user == null)
            {
                throw new Exception(
                    "Kullanıcı bulunamadı."
                );
            }


            if (user.CustomerId == null)
            {
                throw new Exception(
                    "Kullanıcıya bağlı müşteri kaydı bulunamadı."
                );
            }


            var customerId =
                user.CustomerId.Value;



            var applications =
                await _unitOfWork
                    .GetReadRepository<LoanApplication>()
                    .GetWhere(
                        x => x.CustomerId == customerId,
                        false
                    )
                    .OrderByDescending(
                        x => x.ApplicationDate
                    )
                    .ToListAsync();


      

            var result =
                applications
                    .Select(application =>
                        new LoanApplicationDto
                        {
                            Id =  application.Id,

                            CustomerId = application.CustomerId,

                            LoanProductId =
                                application.LoanProductId,

                            LoanCalculationId =
                                application.LoanCalculationId,

                            Status =
                                application.Status,

                            ApplicationDate =
                                application.ApplicationDate,

                            DecisionDate =
                                application.DecisionDate,

                            DecisionNote =
                                application.DecisionNote
                        }
                    )
                    .ToList();


            return result;
        }
    }
}