using internLoanProjectAPI.Application.DTOs.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace internLoanProjectAPI.Application.Abstractions.Services
{
    public interface IAdminCustomerVerificationService
    {
        Task<List<CustomerVerificationDto>> GetAllAsync();

        Task<CustomerVerificationDto> ApproveAsync(int verificationId, string? note);

        Task<CustomerVerificationDto> RejectAsync(int verificationId,string? note);

    }
}
