using internLoanProject.Domain.Entities.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace internLoanProject.Domain.Entities
{
    public class LoanCalculation: BaseEntity
    {
        public decimal Amount { get; set; }

        public int Term { get; set; }

        public decimal InterestRate { get; set; }

        public decimal MonthlyInstallment { get; set; }

        public decimal TotalPayment { get; set; }

        public decimal TotalInterest { get; set; }
        public Guid LoanProductId { get; set; }

        public LoanProduct LoanProduct { get; set; }

        public ICollection<PaymentPlan> PaymentPlans { get; set; }
    }
}
