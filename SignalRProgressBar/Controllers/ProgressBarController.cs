using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BusinessService.Models;
using BusinessService.Services.Interfaces;
using BusinessService.SignalR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Options;
using SignalRProgressBar.Models;
using SignalRProgressBar.Services;

namespace SignalRProgressBar.Controllers
{
    [Authorize]
    public class ProgressBarController : Controller
    {
        private readonly StripeOptions _stripeOptions;
        private readonly ISubscriptionService _subscriptionService;
        private readonly IHubContext<ProgressHub> _hubContext;

        public ProgressBarController(IOptions<StripeOptions> stripeOptions,
                                    ISubscriptionService subscriptionService,                                
                                    IHubContext<ProgressHub> hubContext)
        {
            _stripeOptions = stripeOptions.Value;
            _subscriptionService = subscriptionService;
            _hubContext = hubContext;
        }

        [HttpGet]
        public IActionResult Success()
        {
            return View();
        }

        [HttpGet]
        public IActionResult CheckOut()
        {           

            var shoppingCartViewModel = BuildShoppingCartViewModel();

            return View(shoppingCartViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CheckOut([FromBody] CheckOutModel model)
        {
            //Progress Bar inititalization Message
            await _hubContext.Clients.User(User.Identity.Name).SendAsync("initProgressBar", new ProgressInfo { Pct = 0, Message = "Getting things ready" });

            var cartModel = BuildShoppingCartViewModel();

            //Whelp, turns out all I need is the stripeToken... CheckOutModel is overkill at this point.  :)
            cartModel.StripeToken = model.StripeToken;

            try
            {
                //To make the messages a little more readable.
                Thread.Sleep(400);
                //Call Business Layer to do some work.
                await _subscriptionService.SubscribeUser(cartModel);
            }
            catch (Exception ex)
            {
                //Progress Bar Exception Notification Message
                await _hubContext.Clients.User(User.Identity.Name).SendAsync("exception", new ProgressInfo { Message = ex.Message });

                //Short Circuit Method.
                return Ok();
            }

            //Progress Bar Complete Message
            await _hubContext.Clients.User(User.Identity.Name).SendAsync("completed", new ProgressInfo { Pct = 100, Message = "Process Completed Successfully!" });

            //Complete Task.
            return Ok();
        }

        private ShoppingCartViewModel BuildShoppingCartViewModel()
        {

            var shoppingCartViewModel = new ShoppingCartViewModel
            {
                //Items = await _shoppingCart.GetShoppingCartItemsAsync(),
                //ShoppingCartItemsTotal = ItemCount,
                //ShoppingCartTotal = TotalAmmount,
                //TotalRegistrationFees = TotalRegistrationFees,
                //TotalReoccuringFees = TotalReoccuringFees,
                UserName = User.Identity.Name,
                StripeKey = _stripeOptions.StripePublishableKey
                //FirstName = firstName,
                //LastName = lastName,
            };

            return shoppingCartViewModel;
        }

    }
}