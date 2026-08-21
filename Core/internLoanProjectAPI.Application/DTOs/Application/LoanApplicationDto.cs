using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace internLoanProjectAPI.Application.DTOs.Application
{
    public class LoanApplicationDto
    {
        public int Id { get; set; }

        public int CustomerId { get; set; }

        public int LoanProductId { get; set; }

        public int LoanCalculationId { get; set; }

        public string Status { get; set; }

    }
}



