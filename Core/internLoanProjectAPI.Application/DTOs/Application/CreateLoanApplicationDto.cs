using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace internLoanProjectAPI.Application.DTOs.Application
{
    public class CreateLoanApplicationDto
    {
        public int LoanProductId { get; set; }

        public int LoanCalculationId { get; set; }
    }
}
