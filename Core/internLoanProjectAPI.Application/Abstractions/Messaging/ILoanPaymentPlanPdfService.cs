using internLoanProject.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace internLoanProjectAPI.Application.Abstractions.Messaging
{
    public interface ILoanPaymentPlanPdfService
    {
        byte[] Create(LoanApplication application);
    }
}
