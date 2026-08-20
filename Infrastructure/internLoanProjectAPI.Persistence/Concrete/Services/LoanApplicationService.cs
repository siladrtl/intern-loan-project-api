using internLoanProject.Domain.Entities;
using internLoanProjectAPI.Application.Abstractions.Services;
using internLoanProjectAPI.Application.Abstractions.UnitOfWorks;
using internLoanProjectAPI.Application.DTOs.Application;
using internLoanProjectAPI.Persistence.Contexts;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace internLoanProjectAPI.Persistence.Concrete.Services
{
    public class LoanApplicationService : ILoanApplicationService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly internLoanProjectAPIDbContext _context;



        public async Task<LoanApplicationDto> CreateAsync(
      CreateLoanApplicationDto dto)
        {
            //  JWT'den UserId'yi al
            var userIdClaim =
                _httpContextAccessor.HttpContext?
                    .User
                    .FindFirst(ClaimTypes.NameIdentifier);

            if (userIdClaim == null)
                throw new Exception("Kullanıcı kimliği bulunamadı.");

            if (!Guid.TryParse(userIdClaim.Value, out Guid userId))
                throw new Exception("Geçersiz kullanıcı kimliği.");

            // AppUser'ı bul
            var user = await _context.Users.FindAsync(userId);

            if (user == null)
                throw new Exception("Kullanıcı bulunamadı.");

            // CustomerId kontrolü
            if (user.CustomerId == null)
                throw new Exception(
                    "Kullanıcıya bağlı müşteri kaydı bulunamadı.");

            var customerId = user.CustomerId.Value;

            //LoanProduct kontrolü
            var loanProduct = await _unitOfWork
                .GetReadRepository<LoanProduct>()
                .GetSingleAsync(
                    x => x.Id == dto.LoanProductId &&
                         x.IsActive,
                    false);

            if (loanProduct == null)
                throw new Exception("Kredi ürünü bulunamadı.");

            // LoanCalculation kontrolü
            var calculation = await _unitOfWork
                .GetReadRepository<LoanCalculation>()
                .GetSingleAsync(
                    x => x.Id == dto.LoanCalculationId,
                    false);

            if (calculation == null)
                throw new Exception("Kredi hesaplaması bulunamadı.");

            // Hesaplama gerçekten seçilen ürüne ait mi?
            if (calculation.LoanProductId != dto.LoanProductId)
                throw new Exception(
                    "Kredi hesaplaması seçilen ürünle eşleşmiyor.");

            //  Başvuru oluştur
            var application = new LoanApplication
            {
                Id = Guid.NewGuid(),

                CustomerId = customerId,

                LoanProductId = dto.LoanProductId,

                LoanCalculationId = dto.LoanCalculationId,

                Status = "Pending"
            };

            var result = await _unitOfWork
                .GetWriteRepository<LoanApplication>()
                .AddAsync(application);

            if (!result)
                throw new Exception(
                    "Kredi başvurusu oluşturulamadı.");

            await _unitOfWork.SaveAsync();

            //  DTO döndür
            return new LoanApplicationDto
            {
                Id = application.Id,

                CustomerId = application.CustomerId,

                LoanProductId = application.LoanProductId,

                LoanCalculationId = application.LoanCalculationId,

                Status = application.Status
            };
        }
    }
}




  
    
