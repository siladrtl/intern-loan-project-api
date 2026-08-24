using internLoanProject.Domain.Entities;
using internLoanProject.Domain.Entities.Enums;
using internLoanProject.Domain.Entities.Identity;
using internLoanProjectAPI.Application.Abstractions.Services;
using internLoanProjectAPI.Application.Abstractions.UnitOfWorks;
using internLoanProjectAPI.Application.DTOs.Application;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace internLoanProjectAPI.Persistence.Concrete.Services
{
    public class AdminLoanApplicationService: IAdminLoanApplicationService
    {
        private readonly IUnitOfWork _unitOfWork;


        public AdminLoanApplicationService(
            IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }


        // ==========================================
        // TÜM BAŞVURULARI GETİR
        // ==========================================

        public async Task<List<LoanApplicationDto>> GetAllAsync()
        {
            var applications =
                await _unitOfWork
                    .GetReadRepository<LoanApplication>()
                    .GetAll(false)
                    .OrderByDescending(
                        x => x.ApplicationDate
                    )
                    .ToListAsync();


            return applications
                .Select(
                    application =>
                        MapToDto(application)
                )
                .ToList();
        }


        // ==========================================
        // BAŞVURUYU ONAYLA
        // ==========================================

        public async Task<LoanApplicationDto> ApproveAsync(
            int applicationId,
            string? note)
        {
            var application =
                await _unitOfWork
                    .GetReadRepository<LoanApplication>()
                    .GetSingleAsync(
                        x => x.Id == applicationId
                    );


            if (application == null)
            {
                throw new Exception(
                    "Kredi başvurusu bulunamadı."
                );
            }


            if (
                application.Status !=
                LoanApplicationStatus.Pending
            )
            {
                throw new Exception(
                    "Sadece bekleyen başvurular onaylanabilir."
                );
            }


            application.Status =
                LoanApplicationStatus.Approved;

            application.DecisionDate =
                DateTime.UtcNow;

            application.DecisionNote =
                string.IsNullOrWhiteSpace(note)
                    ? null
                    : note.Trim();


            _unitOfWork
                .GetWriteRepository<LoanApplication>()
                .Update(application);


            await _unitOfWork
                .SaveAsync();


            return MapToDto(
                application
            );
        }


        // ==========================================
        // BAŞVURUYU REDDET
        // ==========================================

        public async Task<LoanApplicationDto> RejectAsync(
            int applicationId,
            string? note)
        {
            var application =
                await _unitOfWork
                    .GetReadRepository<LoanApplication>()
                    .GetSingleAsync(
                        x => x.Id == applicationId
                    );


            if (application == null)
            {
                throw new Exception(
                    "Kredi başvurusu bulunamadı."
                );
            }


            if (
                application.Status !=
                LoanApplicationStatus.Pending
            )
            {
                throw new Exception(
                    "Sadece bekleyen başvurular reddedilebilir."
                );
            }


            application.Status =
                LoanApplicationStatus.Rejected;

            application.DecisionDate =
                DateTime.UtcNow;

            application.DecisionNote =
                string.IsNullOrWhiteSpace(note)
                    ? null
                    : note.Trim();


            _unitOfWork
                .GetWriteRepository<LoanApplication>()
                .Update(application);


            await _unitOfWork
                .SaveAsync();


            return MapToDto(
                application
            );
        }


        // ==========================================
        // ENTITY -> DTO
        // ==========================================

        private static LoanApplicationDto MapToDto(
            LoanApplication application)
        {
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
    }
}
