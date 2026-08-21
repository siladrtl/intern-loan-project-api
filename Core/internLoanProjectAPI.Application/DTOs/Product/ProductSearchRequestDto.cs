using internLoanProject.Domain.Entities.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace internLoanProjectAPI.Application.DTOs.Product
{
    public class ProductSearchRequestDto
    {
        public int LoanTypeId { get; set; }

        public CustomerType? CustomerType { get; set; }

        public decimal Amount { get; set; }

        public int Term { get; set; }

        public List<int>? BankIds { get; set; }
    }
}
