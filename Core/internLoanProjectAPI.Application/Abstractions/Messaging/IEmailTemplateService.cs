using internLoanProject.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace internLoanProjectAPI.Application.Abstractions.Messaging
{
    public interface IEmailTemplateService
    {
        string CreateApprovedLoanApplicationEmail(LoanApplication application);

        string CreateRejectedLoanApplicationEmail(LoanApplication application);
       
    }
}
