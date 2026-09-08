using internLoanProject.Domain.Entities;
using internLoanProject.Domain.Entities.Enums;
using internLoanProjectAPI.Application.Abstractions.Messaging;
using internLoanProjectAPI.Application.Abstractions.Services;
using internLoanProjectAPI.Application.Abstractions.SignalR;
using internLoanProjectAPI.Application.Abstractions.UnitOfWorks;
using internLoanProjectAPI.Application.DTOs.Application;
using internLoanProjectAPI.Application.Messages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace internLoanProjectAPI.Persistence.Concrete.Services
{
    public class AdminLoanApplicationService : IAdminLoanApplicationService
    {
        private const string EmailQueueName = "email-notification-queue";
        private readonly IUnitOfWork _unitOfWork;
        private readonly INotificationService _notificationService;
        private readonly IMessagePublisher _messagePublisher;
        private readonly ILogger<AdminLoanApplicationService> _logger;

        public AdminLoanApplicationService(
            IUnitOfWork unitOfWork,
            INotificationService notificationService,
            IMessagePublisher messagePublisher,
            ILogger<AdminLoanApplicationService> logger)
        {
            _unitOfWork = unitOfWork;
            _notificationService = notificationService;
            _messagePublisher = messagePublisher;
            _logger = logger;
        }

        public async Task<List<LoanApplicationDto>> GetAllAsync()
        {
            return await _unitOfWork
                .GetReadRepository<LoanApplication>()
                .GetAll(false)
                .Include(x => x.Customer)
                .Include(x => x.LoanProduct)
                    .ThenInclude(x => x.Bank)
                .Include(x => x.LoanCalculation)
                .OrderByDescending(x => x.ApplicationDate)
                .Select(application => MapToDto(application))
                .ToListAsync();
        }

        public async Task<LoanApplicationDto> ApproveAsync(int applicationId, string? note)
        {
            return await ProcessDecisionAsync(
                applicationId,
                LoanApplicationStatus.Approved,
                "Approved",
                note,
                "Kredi başvurusu onaylandı.");
        }

        public async Task<LoanApplicationDto> RejectAsync(int applicationId, string? note)
        {
            return await ProcessDecisionAsync(
                applicationId,
                LoanApplicationStatus.Rejected,
                "Rejected",
                note,
                "Kredi başvurusu reddedildi.");
        }

        private async Task<LoanApplicationDto> ProcessDecisionAsync(
            int applicationId,
            LoanApplicationStatus newStatus,
            string statusName,
            string? note,
            string logMessage)
        {
            var application = await GetApplicationWithDetailsAsync(applicationId);

            if (application == null)
            {
                throw new KeyNotFoundException("Kredi başvurusu bulunamadı.");
            }

            if (application.Status != LoanApplicationStatus.Pending)
            {
                throw new InvalidOperationException($"Sadece bekleyen başvurular {statusName.ToLower()} durumuna alınabilir.");
            }

            application.Status = newStatus;
            application.DecisionDate = DateTime.Now;
            application.DecisionNote = string.IsNullOrWhiteSpace(note) ? null : note.Trim();

            _unitOfWork.GetWriteRepository<LoanApplication>().Update(application);
            await _unitOfWork.SaveAsync();

            _logger.LogInformation("{Message} ApplicationId: {ApplicationId}, CustomerId: {CustomerId}", logMessage, application.Id, application.CustomerId);

            await _notificationService.SendApplicationStatusChangedAsync(
                application.CustomerId,
                application.Id,
                statusName,
                application.DecisionNote);

            await _messagePublisher.PublishAsync(
                new LoanApplicationEmailMessage
                {
                    ApplicationId = application.Id
                },
                EmailQueueName);

            return MapToDto(application);
        }

        private async Task<LoanApplication?> GetApplicationWithDetailsAsync(int applicationId)
        {
            return await _unitOfWork
                .GetReadRepository<LoanApplication>()
                .GetAll()
                .Include(x => x.Customer)
                .Include(x => x.LoanProduct)
                    .ThenInclude(x => x.Bank)
                .Include(x => x.LoanCalculation)
                .FirstOrDefaultAsync(x => x.Id == applicationId);
        }

        private static LoanApplicationDto MapToDto(LoanApplication application)
        {
            return new LoanApplicationDto
            {
                Id = application.Id,
                CustomerId = application.CustomerId,
                CustomerName = application.Customer != null
                    ? $"{application.Customer.FirstName} {application.Customer.LastName}"
                    : string.Empty,
                LoanProductId = application.LoanProductId,
                LoanProductName = application.LoanProduct?.Name ?? string.Empty,
                BankName = application.LoanProduct?.Bank?.Name ?? string.Empty,
                LoanCalculationId = application.LoanCalculationId,
                Amount = application.LoanCalculation?.Amount ?? 0,
                Term = application.LoanCalculation?.Term ?? 0,
                MonthlyInstallment = application.LoanCalculation?.MonthlyInstallment ?? 0,
                Status = application.Status,
                ApplicationDate = application.ApplicationDate,
                DecisionDate = application.DecisionDate,
                DecisionNote = application.DecisionNote
            };
        }
    }
}