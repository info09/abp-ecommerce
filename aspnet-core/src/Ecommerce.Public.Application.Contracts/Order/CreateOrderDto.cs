using System;
using System.Collections.Generic;

namespace Ecommerce.Public.Order
{
    public class CreateOrderDto
    {
        public string CustomerName { get; set; }
        public string CustomerPhoneNumber { get; set; }
        public string CustomerAddress { get; set; }
        public Guid? CustomerUserId { get; set; }
        public List<OrderItemDto> Items { get; set; }
    }
}
