using BusinessService.Models;
using BusinessService.Services.Interfaces;
using BusinessService.SignalR;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace BusinessService.Services
{
    public class SubscriptionService : ISubscriptionService
    {
        private readonly IHubContext<ProgressHub> _hubContext;

        public SubscriptionService(IHubContext<ProgressHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task SubscribeUser(ShoppingCartViewModel model)
        {
            var steps = new Random().Next(3, 20);
            var increase = (int)100 / steps;
            var total = 0;

            //Simulate Work
            for (var i = 0; i < steps; i++)
            {
                Thread.Sleep(500);
                total += increase;

                if(i > 7)
                {
                    //Sample exception
                    throw new Exception("Business Service Exception.");
                }

                // Update Progress
                await _hubContext.Clients.User(model.UserName)
                            .SendAsync("updateProgressBar", new ProgressInfo {
                                Pct = total,
                                Message = string.Format("Progress Message: {0}", i)
                            });
            }
        }
    }

}
