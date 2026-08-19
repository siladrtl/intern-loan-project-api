using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace internLoanProjectAPI.Application.DTOs.Product
{
    public class ProductSearchRequestDto
    {
        public Guid LoanTypeId { get; set; }
        public Guid CustomerTypeId { get; set; }
        public decimal Amount { get; set; }
        public int Term { get; set; }
        public List<Guid>? BankIds { get; set; } // opsiyonel banka filtresi
    }
}
