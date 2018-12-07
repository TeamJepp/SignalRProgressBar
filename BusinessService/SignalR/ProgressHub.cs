using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace BusinessService.SignalR
{
    public class ProgressHub : Hub
    {
        public override async Task OnConnectedAsync()
        {
            await base.OnConnectedAsync();
            await Clients.Caller.SendAsync("connected");
            await Task.CompletedTask;
        }
    }
}
