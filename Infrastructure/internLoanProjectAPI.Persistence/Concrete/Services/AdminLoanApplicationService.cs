using internLoanProject.Domain.Entities;
using internLoanProject.Domain.Entities.Enums;
using internLoanProjectAPI.Application.Abstractions.Messaging;
using internLoanProjectAPI.Application.Abstractions.Services;
using internLoanProjectAPI.Application.Abstractions.SignalR;
using internLoanProjectAPI.Application.Abstractions.UnitOfWorks;
using internLoanProjectAPI.Application.DTOs.Application;
using internLoanProjectAPI.Application.Messages;
using Microsoft.EntityFrameworkCore;


namespace internLoanProjectAPI.Persistence.Concrete.Services
{
    public class AdminLoanApplicationService
        : IAdminLoanApplicationService
    {
        private readonly IUnitOfWork _unitOfWork;

        private readonly INotificationService _notificationService;

        private readonly IMessagePublisher _messagePublisher;

        public AdminLoanApplicationService(IUnitOfWork unitOfWork, INotificationService notificationService, IMessagePublisher messagePublisher)
        {
            _unitOfWork = unitOfWork;
            _notificationService = notificationService;
            _messagePublisher = messagePublisher;
        }

        //Tum basvurulari getir
        public async Task<List<LoanApplicationDto>> GetAllAsync()
        {
            var applications =
                await _unitOfWork
                    .GetReadRepository<LoanApplication>()
                    .GetAll(false)

                    .Include(
                        x => x.Customer)

                    .Include(x => x.LoanProduct)
                    .ThenInclude(x => x.Bank)
                    .Include(x => x.LoanCalculation)
                    .OrderByDescending(x => x.ApplicationDate)
                    .ToListAsync();

            return applications
                .Select(application => MapToDto(application))
                .ToList();
        }

        //Basvuruyu Onayla
        public async Task<LoanApplicationDto> ApproveAsync(int applicationId, string? note)
        {
            var application = await GetApplicationWithDetailsAsync(applicationId);

            if (application == null)
            {
                throw new Exception("Kredi başvurusu bulunamadı.");
            }


            if (application.Status != LoanApplicationStatus.Pending)
            {
                throw new Exception("Sadece bekleyen başvurular onaylanabilir.");
            }


            application.Status = LoanApplicationStatus.Approved;
            application.DecisionDate = DateTime.UtcNow;
            application.DecisionNote = string.IsNullOrWhiteSpace(note) ? null : note.Trim();


            _unitOfWork.GetWriteRepository<LoanApplication>().Update(application);
            await _unitOfWork.SaveAsync();

            
            //SignalR
            await _notificationService
                .SendApplicationStatusChangedAsync(
                    application.CustomerId,
                    application.Id,
                    "Approved",
                    application.DecisionNote
                );

            // RabbitMQ - E-posta bildirimi
            await _messagePublisher.PublishAsync(
                new LoanApplicationEmailMessage
                {
                    ApplicationId = application.Id,

                    CustomerId = application.CustomerId,

                    Email = application.Customer.Email,

                    CustomerName = $"{application.Customer.FirstName} {application.Customer.LastName}",

                    Status = "Approved",

                    Subject = "Kredi Başvurunuz Sonuçlandı",

                    Message = "Kredi başvurunuz onaylanmıştır."
                },
                "email-notification-queue"
            );

            return MapToDto(application);
        }

        //Basvuruyu Reddet
        public async Task<LoanApplicationDto> RejectAsync(int applicationId, string? note)
        {
            var application = await GetApplicationWithDetailsAsync(applicationId);


            if (application == null)
            {
                throw new Exception("Kredi başvurusu bulunamadı.");
            }


            if (application.Status != LoanApplicationStatus.Pending)
            {
                throw new Exception("Sadece bekleyen başvurular reddedilebilir.");
            }


            application.Status = LoanApplicationStatus.Rejected;
            application.DecisionDate = DateTime.UtcNow;
            application.DecisionNote = string.IsNullOrWhiteSpace(note) ? null : note.Trim();


            _unitOfWork.GetWriteRepository<LoanApplication>().Update(application);
            await _unitOfWork.SaveAsync();


            //SignalR
            await _notificationService
                .SendApplicationStatusChangedAsync(
                    application.CustomerId,
                    application.Id,
                    "Rejected",
                    application.DecisionNote
                );

            // RabbitMQ - E-posta bildirimi
            await _messagePublisher.PublishAsync(
                new LoanApplicationEmailMessage
                {
                    ApplicationId = application.Id,

                    CustomerId = application.CustomerId,

                    Email = application.Customer.Email,

                    CustomerName =
                        $"{application.Customer.FirstName} {application.Customer.LastName}",

                    Status = "Rejected",

                    Subject = "Kredi Başvurunuz Sonuçlandı",

                    Message = "Kredi başvurunuz reddedilmiştir."
                },
                "email-notification-queue"
            );
            return MapToDto(application);
                
        }

        //Bavuru ile ilgili iliskili bilgiler
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
    
        //Map
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