using internLoanProject.Domain.Entities.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace internLoanProjectAPI.Application.DTOs.Product
{
    public class UpdateLoanProductRequestDto
    {
        public string Name { get; set; }

        public decimal InterestRate { get; set; }

        public decimal MinAmount { get; set; }

        public decimal MaxAmount { get; set; }

        public int MinTerm { get; set; }

        public int MaxTerm { get; set; }

        public int BankId { get; set; }

        public int LoanTypeId { get; set; }

        public CustomerType CustomerType { get; set; }

        public bool IsActive { get; set; }
    }
}
