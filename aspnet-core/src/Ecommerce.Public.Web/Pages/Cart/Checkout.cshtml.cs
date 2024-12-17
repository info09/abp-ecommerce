using Ecommerce.Orders.Events;
using Ecommerce.Public.Order;
using Ecommerce.Public.Web.Extensions;
using Ecommerce.Public.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;
using Volo.Abp.EventBus.Local;

namespace Ecommerce.Public.Web.Pages.Cart
{
    public class CheckoutModel : PageModel
    {
        private readonly IOrderAppService _orderAppService;
        private readonly ILocalEventBus _localEventBus;

        public CheckoutModel(IOrderAppService orderAppService, ILocalEventBus localEventBus)
        {
            _orderAppService = orderAppService;
            _localEventBus = localEventBus;
        }

        public List<CartItem> CartItems { get; set; }
        public bool? CreateStatus { get; set; }

        [BindProperty]
        public OrderDto Order { get; set; }



        public void OnGet()
        {
            CartItems = GetCartItems();
        }

        public async Task OnPostAsync()
        {
            if (!ModelState.IsValid) { }
            var cartItems = new List<OrderItemDto>();

            foreach (var item in GetCartItems())
            {
                cartItems.Add(new OrderItemDto()
                {
                    Price = item.Product.SellPrice,
                    ProductId = item.Product.Id,
                    Quantity = item.Quantity
                });
            }

            Guid? userId = User.Identity.IsAuthenticated ? User.GetUserId() : null;
            var order = await _orderAppService.CreateAsync(new CreateOrderDto()
            {
                CustomerAddress = Order.CustomerAddress,
                CustomerName = Order.CustomerName,
                CustomerPhoneNumber = Order.CustomerPhoneNumber,
                Items = cartItems,
                CustomerUserId = userId
            });
            CartItems = GetCartItems();
            HttpContext.Session.Remove(EcommerceConsts.Cart);
            if (order != null)
            {
                if (User.Identity.IsAuthenticated)
                {
                    var email = User.GetSpecificClaim(ClaimTypes.Email);
                    await _localEventBus.PublishAsync(new NewOrderCreatedEvent()
                    {
                        CustomerEmail = email,
                        Message = "Create Order Success"
                    });
                }
                CreateStatus = true;
            }
            else
                CreateStatus = false;
        }

        private List<CartItem> GetCartItems()
        {
            var cart = HttpContext.Session.GetString(EcommerceConsts.Cart);
            var productCarts = new Dictionary<string, CartItem>();
            if (cart != null)
            {
                productCarts = JsonSerializer.Deserialize<Dictionary<string, CartItem>>(cart);
            }
            return productCarts.Values.ToList();
        }
    }
}
