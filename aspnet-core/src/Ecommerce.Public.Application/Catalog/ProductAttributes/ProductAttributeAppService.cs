using Ecommerce.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

namespace Ecommerce.Public.Catalog.ProductAttributes
{
    public class ProductAttributeAppService : ReadOnlyAppService<ProductAttribute, ProductAttributeDto, Guid, PagedResultRequestDto>, IProductAttributesAppService
    {
        public ProductAttributeAppService(IRepository<ProductAttribute, Guid> repository) : base(repository)
        {
        }

        public async Task<List<ProductAttributeInListDto>> GetAllListAsync()
        {
            var query = await Repository.GetQueryableAsync();
            query = query.Where(i => i.IsActive);
            var data = await AsyncExecuter.ToListAsync(query);
            return ObjectMapper.Map<List<ProductAttribute>, List<ProductAttributeInListDto>>(data);
        }

        public async Task<PagedResult<ProductAttributeInListDto>> GetListFilterAsync(BaseListFilterDto input)
        {
            var query = await Repository.GetQueryableAsync();
            query = query.WhereIf(!string.IsNullOrEmpty(input.Keyword), i => i.Label.ToLower().Contains(input.Keyword.ToLower()));

            var totalCount = await AsyncExecuter.LongCountAsync(query);
            var data = await AsyncExecuter.ToListAsync(query.Skip((input.CurrentPage - 1) * input.PageSize).Take(input.PageSize));

            return new PagedResult<ProductAttributeInListDto>(ObjectMapper.Map<List<ProductAttribute>, List<ProductAttributeInListDto>>(data), totalCount, input.CurrentPage, input.PageSize);
        }
    }
}
