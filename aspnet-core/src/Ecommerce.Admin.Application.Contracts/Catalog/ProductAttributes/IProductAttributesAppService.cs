using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace Ecommerce.Admin.Catalog.ProductAttributes
{
    public interface IProductAttributesAppService : ICrudAppService<ProductAttributeDto, Guid, PagedResultRequestDto, CreateUpdateProductAttributeDto, CreateUpdateProductAttributeDto>
    {
        Task<PagedResultDto<ProductAttributeInListDto>> GetListFilterAsync(BaseListFilterDto input);
        Task<List<ProductAttributeInListDto>> GetAllListAsync();
        Task DeleteMultiple(IEnumerable<Guid> ids);
    }
}
