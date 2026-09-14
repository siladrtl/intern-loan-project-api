using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace internLoanProjectAPI.Application.DTOs.Application
{
    public class LoanApplicationEligibilityDto
    {
        public bool IsEligible { get; set; }

        public string Message { get; set; } = null!;

        public DateTime? NextApplicationDate { get; set; }
    }
}
