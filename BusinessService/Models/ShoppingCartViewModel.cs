using System;
using System.Collections.Generic;
using System.Text;

namespace BusinessService.Models
{
    public class ShoppingCartViewModel
    {
        //simplifying the example by commenting out the Items.
        //public IList<ShoppingCartItem> Items { get; set; }
        public decimal ShoppingCartTotal { get; set; }
        public int ShoppingCartItemsTotal { get; set; }
        public decimal TotalRegistrationFees { get; set; }
        public decimal TotalReoccuringFees { get; set; }
        public string StripeToken { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        //For this example, the Username is the only interesting thing to see here. :)
        public string UserName { get; set; }

        public bool IsStripeCustomer { get; set; }
        public string ShowSource
        {
            get
            {
                if (IsStripeCustomer)
                    return "show";
                else
                    return "hidden";
            }
        }
    }
}
