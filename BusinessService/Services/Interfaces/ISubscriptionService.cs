using BusinessService.Models;
using System.Threading.Tasks;

namespace BusinessService.Services.Interfaces
{

    public interface ISubscriptionService
    {
        Task SubscribeUser(ShoppingCartViewModel model);

    }
}
