using internLoanProject.Domain.Entities.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace internLoanProject.Domain.Entities
{
    public class LoanProduct: BaseEntity
    {
        public decimal InterestRate { get; set; }

        public decimal MinAmount { get; set; }

        public decimal MaxAmount { get; set; }

        public int MinTerm { get; set; }

        public int MaxTerm { get; set; }
        public Guid LoanTypeId { get; set; }

        public LoanType LoanType { get; set; }

        public ICollection<LoanCalculation> LoanCalculations { get; set; }

    }
}
