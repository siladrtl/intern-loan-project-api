using internLoanProject.Domain.Entities;
using internLoanProject.Domain.Entities.Enums;
using internLoanProjectAPI.Application.Abstractions.Services;
using internLoanProjectAPI.Application.Abstractions.UnitOfWorks;
using internLoanProjectAPI.Application.DTOs.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace internLoanProjectAPI.Persistence.Concrete.Services
{
    public class AdminCustomerVerificationService: IAdminCustomerVerificationService
    {
        private readonly IUnitOfWork _unitOfWork;

        public AdminCustomerVerificationService(
            IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<CustomerVerificationDto>> GetAllAsync()
        {
            var documents = _unitOfWork
                .GetReadRepository<CustomerVerificationDocument>()
                .GetAll(false)
                .ToList();

            var result = new List<CustomerVerificationDto>();

            foreach (var document in documents)
            {
                var customer = await _unitOfWork
                    .GetReadRepository<Customer>()
                    .GetSingleAsync(
                        x => x.Id == document.CustomerId,
                        false
                    );

                if (customer == null)
                    continue;

                result.Add(new CustomerVerificationDto
                {
                    Id = document.Id,
                    CustomerId = customer.Id,

                    CustomerName =
                        $"{customer.FirstName} {customer.LastName}",

                    CustomerType = customer.CustomerType,

                    OriginalFileName =
                        document.OriginalFileName,

                    ContentType =
                        document.ContentType,

                    FileSize =
                        document.FileSize,

                    Status =
                        document.Status,

                    UploadedAt =
                        document.UploadedAt,

                    VerifiedAt =
                        document.VerifiedAt,

                    VerificationNote =
                        document.VerificationNote
                });
            }

            return result;
        }

        public async Task<CustomerVerificationDto> ApproveAsync(
            int verificationId,
            string? note)
        {
            var document = await _unitOfWork
                .GetReadRepository<CustomerVerificationDocument>()
                .GetSingleAsync(
                    x => x.Id == verificationId,
                    true
                );

            if (document == null)
            {
                throw new Exception(
                    "Doğrulama belgesi bulunamadı."
                );
            }

            if (document.Status != VerificationStatus.Pending)
            {
                throw new Exception(
                    "Bu belge daha önce işleme alınmış."
                );
            }

            document.Status =
                VerificationStatus.Approved;

            document.VerifiedAt =
                DateTime.Now;

            document.VerificationNote =
                note;

            _unitOfWork
                .GetWriteRepository<CustomerVerificationDocument>()
                .Update(document);

            await _unitOfWork.SaveAsync();

            return await CreateDtoAsync(document);
        }

        public async Task<CustomerVerificationDto> RejectAsync(
            int verificationId,
            string? note)
        {
            var document = await _unitOfWork
                .GetReadRepository<CustomerVerificationDocument>()
                .GetSingleAsync(
                    x => x.Id == verificationId,
                    true
                );

            if (document == null)
            {
                throw new Exception(
                    "Doğrulama belgesi bulunamadı."
                );
            }

            if (document.Status != VerificationStatus.Pending)
            {
                throw new Exception(
                    "Bu belge daha önce işleme alınmış."
                );
            }

            document.Status =
                VerificationStatus.Rejected;

            document.VerifiedAt =
                DateTime.Now;

            document.VerificationNote =
                note;

            _unitOfWork
                .GetWriteRepository<CustomerVerificationDocument>()
                .Update(document);

            await _unitOfWork.SaveAsync();

            return await CreateDtoAsync(document);
        }

        private async Task<CustomerVerificationDto> CreateDtoAsync(
            CustomerVerificationDocument document)
        {
            var customer = await _unitOfWork
                .GetReadRepository<Customer>()
                .GetSingleAsync(
                    x => x.Id == document.CustomerId,
                    false
                );

            if (customer == null)
            {
                throw new Exception(
                    "Belgeye ait müşteri bulunamadı."
                );
            }

            return new CustomerVerificationDto
            {
                Id = document.Id,
                CustomerId = customer.Id,

                CustomerName =
                    $"{customer.FirstName} {customer.LastName}",

                CustomerType =
                    customer.CustomerType,

                OriginalFileName =
                    document.OriginalFileName,

                ContentType =
                    document.ContentType,

                FileSize =
                    document.FileSize,

                Status =
                    document.Status,

                UploadedAt =
                    document.UploadedAt,

                VerifiedAt =
                    document.VerifiedAt,

                VerificationNote =
                    document.VerificationNote
            };
        }
    }
}




    
       
