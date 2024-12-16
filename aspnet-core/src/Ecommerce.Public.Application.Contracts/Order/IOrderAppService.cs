using System;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace Ecommerce.Public.Order
{
    public interface IOrderAppService : ICrudAppService<OrderDto, Guid, PagedResultRequestDto, CreateOrderDto, CreateOrderDto>
    {
    }
}
