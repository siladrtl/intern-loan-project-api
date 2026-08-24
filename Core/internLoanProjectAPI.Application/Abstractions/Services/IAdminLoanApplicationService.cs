using internLoanProjectAPI.Application.DTOs.Application;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace internLoanProjectAPI.Application.Abstractions.Services
{
    public interface IAdminLoanApplicationService
    {
        Task<List<LoanApplicationDto>> GetAllAsync();

        Task<LoanApplicationDto> ApproveAsync(int applicationId, string? note);

        Task<LoanApplicationDto> RejectAsync(int applicationId, string? note);
    }
}
