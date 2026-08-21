using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace internLoanProjectAPI.Application.DTOs.Calculation
{
    public class LoanCalculationDto
    {
        public int Id { get; set; }

        public int LoanProductId { get; set; }

        public string LoanProductName { get; set; }

        public decimal Amount { get; set; }

        public int Term { get; set; }

        public decimal InterestRate { get; set; }

        public decimal MonthlyInstallment { get; set; }

        public decimal TotalInterest { get; set; }

        public decimal TotalKkdf { get; set; }

        public decimal TotalBsmv { get; set; }

        public decimal TotalPayment { get; set; }

        public List<PaymentPlanDto> PaymentPlans { get; set; }
    }
}
