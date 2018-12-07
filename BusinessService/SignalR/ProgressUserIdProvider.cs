using Microsoft.AspNetCore.SignalR;

namespace BusinessService.SignalR
{
    public class ProgressUserIdProvider : IUserIdProvider
    {
        public virtual string GetUserId(HubConnectionContext connection)
        {
            return connection.User?.Identity.Name;
        }
    }
}
