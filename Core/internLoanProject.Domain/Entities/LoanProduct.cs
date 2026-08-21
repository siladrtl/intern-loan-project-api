using internLoanProject.Domain.Entities.Common;
using internLoanProject.Domain.Entities.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace internLoanProject.Domain.Entities
{
    public class LoanProduct: BaseEntity
    {
        public string Name { get; set; }

        public decimal InterestRate { get; set; }

        public decimal MinAmount { get; set; }

        public decimal MaxAmount { get; set; }

        public int MinTerm { get; set; }

        public int MaxTerm { get; set; }

        public bool IsActive { get; set; }

        public int BankId { get; set; }

        public Bank Bank { get; set; }

        public int LoanTypeId { get; set; }

        public LoanType LoanType { get; set; }

        public CustomerType CustomerType { get; set; }

        public ICollection<LoanCalculation> LoanCalculations { get; set; }

        public ICollection<LoanApplication> LoanApplications { get; set; }
    }

}

