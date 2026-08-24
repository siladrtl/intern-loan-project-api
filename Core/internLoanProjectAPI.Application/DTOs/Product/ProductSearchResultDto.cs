using internLoanProject.Domain.Entities.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace internLoanProjectAPI.Application.DTOs.Product
{
    public class ProductSearchResultDto
    {
        public int LoanProductId { get; set; }

        public int BankId { get; set; }

        public string LoanProductName { get; set; }

        public string BankName { get; set; }

        public string LoanTypeName { get; set; }

        public CustomerType CustomerType { get; set; }

        public decimal Amount { get; set; }

        public int Term { get; set; }

        public decimal InterestRate { get; set; }

        public decimal MonthlyInstallment { get; set; }

        public decimal TotalPayment { get; set; }
    }
}
