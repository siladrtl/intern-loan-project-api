using internLoanProjectAPI.Application.Abstractions.SignalR;
using internLoanProjectAPI.SignalR.Hubs;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace internLoanProjectAPI.SignalR.HubServices
{
    public class NotificationService: INotificationService
    {
        private readonly IHubContext<NotificationHub> _hubContext;

        public NotificationService(IHubContext<NotificationHub> hubContext)
        {
            _hubContext = hubContext;
        }
        public async Task SendApplicationStatusChangedAsync(int customerId, int applicationId, string status, string? message)
        {
            await _hubContext
                .Clients
                .Group($"customer-{customerId}")
                .SendAsync(
                    "ApplicationStatusChanged",
                    new
                    {
                        applicationId,
                        status,
                        message
                    }
                );
        }
    }
}
