using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace internLoanProjectAPI.Application.Messages
{
    public class LoanApplicationEmailMessage
    {      
        public int ApplicationId { get; set; }

        public int CustomerId { get; set; }

        public string Email { get; set; } = null!;

        public string CustomerName { get; set; } = null!;

        public string Status { get; set; } = null!;

        public string Subject { get; set; } = null!;

        public string Message { get; set; } = null!;
    }
}

