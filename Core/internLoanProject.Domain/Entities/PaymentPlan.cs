using internLoanProject.Domain.Entities.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace internLoanProject.Domain.Entities
{
    public class PaymentPlan: BaseEntity
    {
        public int LoanCalculationId { get; set; }

        public LoanCalculation LoanCalculation { get; set; }

        public int InstallmentNumber { get; set; }

        public DateTime DueDate { get; set; }

        public decimal InstallmentAmount { get; set; }

        public decimal PrincipalAmount { get; set; }

        public decimal InterestAmount { get; set; }

        public decimal KkdfAmount { get; set; }

        public decimal BsmvAmount { get; set; }

        public decimal RemainingPrincipal { get; set; }
    }
}
