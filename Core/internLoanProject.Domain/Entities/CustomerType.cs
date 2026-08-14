using internLoanProject.Domain.Entities.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace internLoanProject.Domain.Entities
{
    public class CustomerType: BaseEntity
    {
        public string Name { get; set; }

        public ICollection<Customer> Customers { get; set; }

        public ICollection<LoanProduct> LoanProducts { get; set; }
    }
}
