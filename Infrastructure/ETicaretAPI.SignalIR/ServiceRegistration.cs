using ETicaretAPI.Application.Abstractions.Hubs;
using ETicaretAPI.SignalIR.HubServices;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretAPI.SignalIR
{
    public static class ServiceRegistration
    {
        public static void AddSignalRServices(this IServiceCollection collection)
        {
            collection.AddTransient<IProductHubService,ProductHubService>();
            collection.AddSignalR();
        }
    }
}
