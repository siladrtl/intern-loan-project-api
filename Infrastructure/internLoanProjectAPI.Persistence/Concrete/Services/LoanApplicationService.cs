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
    public class LoanApplicationService
        : ILoanApplicationService
    {
        private readonly IUnitOfWork _unitOfWork;

        private readonly IHttpContextAccessor _httpContextAccessor;

        private readonly internLoanProjectAPIDbContext _context;

        public LoanApplicationService(IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor, internLoanProjectAPIDbContext context)
        {
            _unitOfWork = unitOfWork;
            _httpContextAccessor = httpContextAccessor;
            _context = context;
        }

        //Basvuru Olustur
        public async Task<LoanApplicationDto> CreateAsync(CreateLoanApplicationDto dto)
        {
          
            var userIdClaim = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier);


            if (userIdClaim == null)
            {
                throw new Exception("Kullanıcı kimliği bulunamadı.");
            }


            if (!Guid.TryParse(userIdClaim.Value, out Guid userId))
            {
                throw new Exception("Geçersiz kullanıcı kimliği.");
            }


            //AppUser Bul 
            var user = await _context.Users.FindAsync(userId);

            if (user == null)
            {
                throw new Exception("Kullanıcı bulunamadı.");
            }


            if (user.CustomerId == null)
            {
                throw new Exception("Kullanıcıya bağlı müşteri kaydı bulunamadı.");
            }


            var customerId = user.CustomerId.Value;
            var customer = await _unitOfWork
                    .GetReadRepository<Customer>()
                    .GetSingleAsync( x => x.Id == customerId, false);


            if (customer == null)
            {
                throw new Exception(
                    "Müşteri kaydı bulunamadı.");
            }


            // kredi urunu + banka
            var loanProduct = await _unitOfWork
                    .GetReadRepository<LoanProduct>()
                    .GetAll(false)
                    .Include(x => x.Bank)
                    .FirstOrDefaultAsync(
                        x =>
                            x.Id ==
                            dto.LoanProductId
                            &&
                            x.IsActive
                    );


            if (loanProduct == null)
            {
                throw new Exception("Aktif kredi ürünü bulunamadı.");
            }

            //Musteri tipi uygun mu?
            if (loanProduct.CustomerType != customer.CustomerType)
            {
                throw new Exception("Seçilen kredi ürünü müşteri tipinize uygun değildir.");

            }

            // Kredi hesaplamasini bul
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
                    "Kredi hesaplaması bulunamadı.");
            }

            if (calculation.LoanProductId != dto.LoanProductId)
            {
                throw new Exception("Kredi hesaplaması seçilen ürünle eşleşmiyor.");
            }


            // Kredi basvurusu olustur
            var application =
                new LoanApplication
                { 
                    CustomerId = customerId,
                    LoanProductId = dto.LoanProductId,
                    LoanCalculationId = dto.LoanCalculationId,
                    Status = LoanApplicationStatus.Pending,
                    ApplicationDate = DateTime.UtcNow,
                    DecisionDate = null,
                    DecisionNote = null
                };

            // Database'e ekle
            var result = await _unitOfWork.GetWriteRepository<LoanApplication>().AddAsync(application);
                 
            if (!result)
            {
                throw new Exception("Kredi başvurusu oluşturulamadı.");
                    
            }
            await _unitOfWork.SaveAsync();
                


            //Response
            return new LoanApplicationDto
            {
                Id = application.Id,
                CustomerId = application.CustomerId,
                CustomerName = $"{customer.FirstName} " + $"{customer.LastName}",
                LoanProductId = application.LoanProductId,
                LoanProductName = loanProduct.Name,
                BankName = loanProduct.Bank.Name,
                LoanCalculationId = application.LoanCalculationId,
                Amount = calculation.Amount,
                Term = calculation.Term,
                MonthlyInstallment = calculation.MonthlyInstallment,
                Status = application.Status,
                ApplicationDate = application.ApplicationDate,
                DecisionDate = application.DecisionDate,
                DecisionNote = application.DecisionNote

            };
        }
     
        //Musterinin kendi basvurularini getir
        public async Task<List<LoanApplicationDto>>GetMyApplicationsAsync()
        {
          
            //JWT User Id
            var userIdClaim = _httpContextAccessor
                    .HttpContext?
                    .User
                    .FindFirst(
                        ClaimTypes.NameIdentifier);


            if (userIdClaim == null)
            {
                throw new Exception("Kullanıcı kimliği bulunamadı.");
            }


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

            var user = await _context.Users
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


            var customerId = user.CustomerId.Value;
            var applications = await _unitOfWork
                    .GetReadRepository<LoanApplication>().GetAll(false)

                    .Where( x => x.CustomerId == customerId)
                    .Include(x => x.Customer)
                    .Include( x => x.LoanProduct)
                    .ThenInclude(x => x.Bank)
                    .Include(x => x.LoanCalculation)
                    .OrderByDescending(x => x.ApplicationDate)
                    .ToListAsync();

            return applications
                .Select(application => MapToDto(application))
                .ToList();
        }
        private static LoanApplicationDto MapToDto(LoanApplication application)
        {
            return new LoanApplicationDto
            {
                Id = application.Id,
                CustomerId = application.CustomerId,
                CustomerName = $"{application.Customer.FirstName} " + $"{application.Customer.LastName}",
                LoanProductId = application.LoanProductId,
                LoanProductName = application.LoanProduct.Name,
                BankName = application.LoanProduct.Bank.Name,
                LoanCalculationId = application.LoanCalculationId,
                Amount = application.LoanCalculation.Amount,
                Term = application.LoanCalculation.Term,
                MonthlyInstallment = application.LoanCalculation.MonthlyInstallment,
                Status = application.Status,
                ApplicationDate = application.ApplicationDate,
                DecisionDate = application.DecisionDate,
                DecisionNote = application.DecisionNote

            };
        }
    }
}