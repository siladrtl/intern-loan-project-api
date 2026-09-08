using internLoanProject.Domain.Entities;
using internLoanProject.Domain.Entities.Enums;
using internLoanProject.Domain.Entities.Identity;
using internLoanProjectAPI.Application.Abstractions.Services;
using internLoanProjectAPI.Application.Abstractions.UnitOfWorks;
using internLoanProjectAPI.Application.DTOs.Auth;
using internLoanProjectAPI.Persistence.Contexts;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace internLoanProjectAPI.Persistence.Concrete.Services
{
    public class AdminCustomerVerificationService : IAdminCustomerVerificationService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<AppUser> _userManager;
        private readonly internLoanProjectAPIDbContext _context;

        public AdminCustomerVerificationService(
            IUnitOfWork unitOfWork,
            UserManager<AppUser> userManager,
            internLoanProjectAPIDbContext context)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _context = context;
        }

        public async Task<List<CustomerVerificationDto>> GetAllAsync()
        {
            return await _unitOfWork
                .GetReadRepository<CustomerVerificationDocument>()
                .GetAll(false)
                .Include(x => x.CustomerRegistration)
                .Select(d => MapToDto(d, d.CustomerRegistration))
                .ToListAsync();
        }

        public async Task<CustomerVerificationDto> ApproveAsync(int verificationId, string? note)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync();

            try
            { 
                var (document, registration) = await GetPendingVerificationAsync(verificationId);

                var existingCustomer = await _unitOfWork
                    .GetReadRepository<Customer>()
                    .GetSingleAsync(
                        x => x.NationalId == registration.NationalId || x.Email == registration.Email,
                        false);

                if (existingCustomer != null)
                {
                    throw new InvalidOperationException("Bu kişiye ait müşteri kaydı zaten mevcut.");
                }

                var user = await _userManager.FindByEmailAsync(registration.Email);
                if (user == null)
                {
                    throw new KeyNotFoundException("Kullanıcı hesabı bulunamadı.");
                }

                var customer = new Customer
                {
                    FirstName = registration.FirstName,
                    LastName = registration.LastName,
                    BirthDate = registration.BirthDate,
                    NationalId = registration.NationalId,
                    Email = registration.Email,
                    PhoneNumber = registration.PhoneNumber,
                    City = registration.City,
                    District = registration.District,
                    CustomerType = registration.CustomerType,
                    CustomerSince = DateTime.Now
                };

                var customerResult = await _unitOfWork
                    .GetWriteRepository<Customer>()
                    .AddAsync(customer);

                if (!customerResult)
                {
                    throw new InvalidOperationException("Müşteri kaydı oluşturulamadı.");
                }

                await _unitOfWork.SaveAsync();

                user.CustomerId = customer.Id;
                var userUpdateResult = await _userManager.UpdateAsync(user);

                if (!userUpdateResult.Succeeded)
                {
                    var errors = string.Join(", ", userUpdateResult.Errors.Select(x => x.Description));
                    throw new InvalidOperationException($"Kullanıcı müşteri hesabına bağlanamadı. {errors}");
                }

                var isCustomer = await _userManager.IsInRoleAsync(user, "Customer");
                if (!isCustomer)
                {
                    var roleResult = await _userManager.AddToRoleAsync(user, "Customer");
                    if (!roleResult.Succeeded)
                    {
                        var errors = string.Join(", ", roleResult.Errors.Select(x => x.Description));
                        throw new InvalidOperationException($"Customer rolü atanamadı. {errors}");
                    }
                }

                ApplyDecision(document, registration, VerificationStatus.Approved, note);

                _unitOfWork.GetWriteRepository<CustomerVerificationDocument>().Update(document);
                _unitOfWork.GetWriteRepository<CustomerRegistration>().Update(registration);

                await _unitOfWork.SaveAsync();
                await transaction.CommitAsync();

                return MapToDto(document, registration);
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<CustomerVerificationDto> RejectAsync(int verificationId, string? note)
        {
            var (document, registration) = await GetPendingVerificationAsync(verificationId);

            ApplyDecision(document, registration, VerificationStatus.Rejected, note);

            _unitOfWork.GetWriteRepository<CustomerVerificationDocument>().Update(document);
            _unitOfWork.GetWriteRepository<CustomerRegistration>().Update(registration);

            await _unitOfWork.SaveAsync();

            return MapToDto(document, registration);
        }

        public async Task<CustomerVerificationDocument> GetDocumentAsync(int verificationId)
        {
            var document = await _unitOfWork
                .GetReadRepository<CustomerVerificationDocument>()
                .GetSingleAsync(x => x.Id == verificationId, false);

            if (document == null)
            {
                throw new KeyNotFoundException("Doğrulama belgesi bulunamadı.");
            }

            return document;
        }

        private async Task<(CustomerVerificationDocument document, CustomerRegistration registration)> GetPendingVerificationAsync(int verificationId)
        {
            var document = await _unitOfWork
                .GetReadRepository<CustomerVerificationDocument>()
                .GetSingleAsync(x => x.Id == verificationId, true);

            if (document == null)
            {
                throw new KeyNotFoundException("Doğrulama belgesi bulunamadı.");
            }

            if (document.Status != VerificationStatus.Pending)
            {
                throw new InvalidOperationException("Bu belge daha önce işleme alınmış.");
            }

            var registration = await _unitOfWork
                .GetReadRepository<CustomerRegistration>()
                .GetSingleAsync(x => x.Id == document.CustomerRegistrationId, true);

            if (registration == null)
            {
                throw new KeyNotFoundException("Müşteri olma başvurusu bulunamadı.");
            }

            if (registration.Status != VerificationStatus.Pending)
            {
                throw new InvalidOperationException("Bu müşteri başvurusu daha önce işleme alınmış.");
            }

            return (document, registration);
        }

        private static void ApplyDecision(
            CustomerVerificationDocument document,
            CustomerRegistration registration,
            VerificationStatus status,
            string? note)
        {
            var now = DateTime.Now;

            document.Status = status;
            document.VerifiedAt = now;
            document.VerificationNote = note;

            registration.Status = status;
            registration.VerifiedAt = now;
            registration.VerificationNote = note;
        }

        private static CustomerVerificationDto MapToDto(
            CustomerVerificationDocument document,
            CustomerRegistration registration)
        {
            return new CustomerVerificationDto
            {
                Id = document.Id,
                CustomerRegistrationId = registration.Id,
                CustomerName = $"{registration.FirstName} {registration.LastName}",
                CustomerType = registration.CustomerType,
                OriginalFileName = document.OriginalFileName,
                ContentType = document.ContentType,
                FileSize = document.FileSize,
                Status = document.Status,
                UploadedAt = document.UploadedAt,
                VerifiedAt = document.VerifiedAt,
                VerificationNote = document.VerificationNote
            };
        }
    }
}