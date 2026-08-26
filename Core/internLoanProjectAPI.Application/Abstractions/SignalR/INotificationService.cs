using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace internLoanProjectAPI.Application.Abstractions.SignalR
{
    public interface INotificationService
    {
        Task SendApplicationStatusChangedAsync(int customerId, int applicationId, string status, string? message);
    }
}
