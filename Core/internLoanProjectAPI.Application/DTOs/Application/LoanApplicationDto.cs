using internLoanProject.Domain.Entities.Enums;

namespace internLoanProjectAPI.Application.DTOs.Application
{
    public class LoanApplicationDto
    {
        public int Id { get; set; }

        public int CustomerId { get; set; }

        public string CustomerName { get; set; } = null!;


        public int LoanProductId { get; set; }

        public string LoanProductName { get; set; } = null!;

        public string BankName { get; set; } = null!;

        public int LoanCalculationId { get; set; }

        public decimal Amount { get; set; }

        public int Term { get; set; }

        public decimal MonthlyInstallment { get; set; }


        public LoanApplicationStatus Status { get; set; }


        public DateTime ApplicationDate { get; set; }

        public DateTime? DecisionDate { get; set; }

        public string? DecisionNote { get; set; }
    }
}