using ETicaretAPI.Application.Abstractions.Hubs;
using ETicaretAPI.SignalIR.Hubs;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretAPI.SignalIR.HubServices
{
    public class OrderHubService : IOrderHubService
    {
        private readonly IHubContext<OrderHub> _hubContext;

        public OrderHubService(IHubContext<OrderHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task OrderAddedMessageAsync(string message)
        {
           await _hubContext.Clients.All.SendAsync(ReceiveFunctionNames.OrderAddedMessage, message);
        }
    }
}
