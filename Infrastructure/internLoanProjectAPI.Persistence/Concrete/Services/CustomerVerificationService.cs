using internLoanProject.Domain.Entities;
using internLoanProjectAPI.Application.Abstractions.Services;
using internLoanProjectAPI.Application.Abstractions.UnitOfWorks;
using internLoanProjectAPI.Application.DTOs.Auth;
using internLoanProjectAPI.Persistence.Contexts;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace internLoanProjectAPI.Persistence.Concrete.Services
{
    public class CustomerVerificationService: ICustomerVerificationService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly internLoanProjectAPIDbContext _context;

        public CustomerVerificationService(IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor, internLoanProjectAPIDbContext context)
        {
            _unitOfWork = unitOfWork;
            _httpContextAccessor = httpContextAccessor;
            _context = context;
        }

        public async Task<CustomerVerificationStatusDto> GetMyStatusAsync()
        {
            var userIdClaim = _httpContextAccessor
                .HttpContext?
                .User
                .FindFirst(ClaimTypes.NameIdentifier);

            if (userIdClaim == null)
                throw new Exception("Kullanıcı kimliği bulunamadı.");

            if (!Guid.TryParse(userIdClaim.Value, out Guid userId))
                throw new Exception("Geçersiz kullanıcı kimliği.");

            var user = await _context.Users.FindAsync(userId);

            if (user == null)
                throw new Exception("Kullanıcı bulunamadı.");

            if (user.CustomerId == null)
                throw new Exception("Kullanıcıya bağlı müşteri kaydı bulunamadı.");

            var document = await _unitOfWork
                .GetReadRepository<CustomerVerificationDocument>()
                .GetSingleAsync(
                    x => x.CustomerId == user.CustomerId.Value,
                    false
                );

            if (document == null)
                throw new Exception("Doğrulama belgesi bulunamadı.");

            return new CustomerVerificationStatusDto
            {
                Status = document.Status,
                UploadedAt = document.UploadedAt,
                VerifiedAt = document.VerifiedAt,
                VerificationNote = document.VerificationNote
            };
        }
    }
}
