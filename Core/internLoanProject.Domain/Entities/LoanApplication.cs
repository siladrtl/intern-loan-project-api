using internLoanProject.Domain.Entities.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace internLoanProject.Domain.Entities
{
    public class LoanApplication: BaseEntity
    {
        public Guid CustomerId { get; set; }

        public Customer Customer { get; set; }

        public Guid LoanProductId { get; set; }

        public LoanProduct LoanProduct { get; set; }

        public Guid LoanCalculationId { get; set; }

        public LoanCalculation LoanCalculation { get; set; }

        public string Status { get; set; }
    }
}
