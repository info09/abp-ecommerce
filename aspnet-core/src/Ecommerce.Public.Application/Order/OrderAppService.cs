using Ecommerce.Orders;
using Ecommerce.Products;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

namespace Ecommerce.Public.Order
{
    public class OrderAppService : CrudAppService<Orders.Order, OrderDto, Guid, PagedResultRequestDto, CreateOrderDto, CreateOrderDto>, IOrderAppService
    {
        private readonly OrderCodeGenerator _orderCodeGenerator;
        private IRepository<OrderItem> _orderItemRepository;
        private readonly IRepository<Product, Guid> _productRepository;
        public OrderAppService(IRepository<Orders.Order, Guid> repository, OrderCodeGenerator orderCodeGenerator, IRepository<OrderItem> orderItemRepository, IRepository<Product, Guid> productRepository) : base(repository)
        {
            _orderCodeGenerator = orderCodeGenerator;
            _orderItemRepository = orderItemRepository;
            _productRepository = productRepository;
        }

        public override async Task<OrderDto> CreateAsync(CreateOrderDto input)
        {
            var subTotal = input.Items.Sum(i => i.Quantity * i.Price);
            var orderId = Guid.NewGuid();
            var order = new Orders.Order(orderId)
            {
                Code = await _orderCodeGenerator.GenerateAsync(),
                CustomerAddress = input.CustomerAddress,
                CustomerName = input.CustomerName,
                CustomerPhoneNumber = input.CustomerPhoneNumber,
                ShippingFee = 0,
                CustomerUserId = input.CustomerUserId,
                Tax = 0,
                SubTotal = subTotal,
                GrandTotal = subTotal,
                Discount = 0,
                PaymentMethod = PaymentMethod.COD,
                Total = subTotal,
                OrderStatus = OrderStatus.New
            };

            var items = new List<OrderItem>();
            foreach (var orderItem in input.Items)
            {
                var product = await _productRepository.GetAsync(orderItem.ProductId);
                items.Add(new OrderItem()
                {
                    OrderId = orderId,
                    ProductId = product.Id,
                    Price = orderItem.Price,
                    Quantity = orderItem.Quantity,
                    SKU = product.SKU,
                });
            }

            await _orderItemRepository.InsertManyAsync(items);
            var result = await Repository.InsertAsync(order);

            return ObjectMapper.Map<Orders.Order, OrderDto>(result);
        }
    }
}
