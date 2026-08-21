using internLoanProject.Domain.Entities.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace internLoanProject.Domain.Entities
{
    public class LoanType: BaseEntity
    {
        public string Name { get; set; }
        public decimal KkdfRate { get; set; }

        public decimal BsmvRate { get; set; }
        public ICollection<LoanProduct> LoanProducts { get; set; }
    }
}
