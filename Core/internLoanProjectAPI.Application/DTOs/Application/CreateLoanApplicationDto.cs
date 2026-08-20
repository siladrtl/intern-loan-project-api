using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace internLoanProjectAPI.Application.DTOs.Application
{
    public class CreateLoanApplicationDto
    {
        public Guid LoanProductId { get; set; }
        public Guid LoanCalculationId { get; set; }
    }
}
