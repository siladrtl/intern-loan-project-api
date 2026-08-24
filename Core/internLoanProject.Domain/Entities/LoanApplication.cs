using internLoanProject.Domain.Entities.Common;
using internLoanProject.Domain.Entities.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace internLoanProject.Domain.Entities
{
    public class LoanApplication: BaseEntity
    {
        public int CustomerId { get; set; }

        public Customer Customer { get; set; }


        public int LoanProductId { get; set; }

        public LoanProduct LoanProduct { get; set; }


        public int LoanCalculationId { get; set; }

        public LoanCalculation LoanCalculation { get; set; }

        public LoanApplicationStatus Status { get; set; }

        public DateTime ApplicationDate { get; set; }


        public DateTime? DecisionDate { get; set; }

        public string? DecisionNote { get; set; }
    }
}
