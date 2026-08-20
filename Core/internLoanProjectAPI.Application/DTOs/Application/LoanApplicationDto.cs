using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace internLoanProjectAPI.Application.DTOs.Application
{
    public class LoanApplicationDto
    {
        public Guid Id { get; set; }

        public Guid CustomerId { get; set; }

        public Guid LoanProductId { get; set; }

        public Guid LoanCalculationId { get; set; }

        public string Status { get; set; }

    }
}



