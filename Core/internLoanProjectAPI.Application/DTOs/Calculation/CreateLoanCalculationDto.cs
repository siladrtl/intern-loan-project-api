using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace internLoanProjectAPI.Application.DTOs.Calculation
{
    public class CreateLoanCalculationDto
    {
        public Guid LoanProductId { get; set; }
        public int Term { get; set; }
        public decimal Amount { get; set; }
    }
}
