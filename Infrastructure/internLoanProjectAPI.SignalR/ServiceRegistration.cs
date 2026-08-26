using internLoanProjectAPI.Application.Abstractions.SignalR;
using internLoanProjectAPI.SignalR.HubServices;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace internLoanProjectAPI.SignalR
{
    public static class ServiceRegistration
    {
        public static void AddSignalRServices(this IServiceCollection collection)
        { 
            collection.AddTransient<INotificationService, NotificationService>();
            collection.AddSignalRCore();
        }
    }
}
