using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace internLoanProjectAPI.Application.Messages
{
    public class LoanApplicationSmsMessage
    {
        public int ApplicationId { get; set; }

        public int CustomerId { get; set; }

        public string PhoneNumber { get; set; } = null!;

        public string Status { get; set; } = null!;

        public string Message { get; set; } = null!;
    }
}
