using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace internLoanProjectAPI.SignalR.Hubs
{
    [Authorize]
    public class NotificationHub: Hub
    {
            public override async Task OnConnectedAsync()
            {
                var customerId = Context.User?
                        .FindFirst("CustomerId")?
                        .Value;

                if (!string.IsNullOrWhiteSpace(customerId))
                {
                    await Groups.AddToGroupAsync(Context.ConnectionId, $"customer-{customerId}");
                }
                await base.OnConnectedAsync();
            }


            public override async Task OnDisconnectedAsync(Exception? exception)
            {
                var customerId = Context.User?
                        .FindFirst("CustomerId")?
                        .Value;

                if (!string.IsNullOrWhiteSpace(customerId))
                {
                    await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"customer-{customerId}");
                }

                await base.OnDisconnectedAsync(exception);
            }
     }
}


