using Ecommerce.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

namespace Ecommerce.Admin.ProductAttributes
{
    public class ProductAttributeAppService : CrudAppService<ProductAttribute, ProductAttributeDto, Guid, PagedResultRequestDto, CreateUpdateProductAttributeDto, CreateUpdateProductAttributeDto>, IProductAttributesAppService
    {
        public ProductAttributeAppService(IRepository<ProductAttribute, Guid> repository) : base(repository)
        {
        }

        public async Task DeleteMultiple(IEnumerable<Guid> ids)
        {
            await Repository.DeleteManyAsync(ids);
            await UnitOfWorkManager.Current.SaveChangesAsync();
        }

        public async Task<List<ProductAttributeInListDto>> GetAllAsync()
        {
            var query = await Repository.GetQueryableAsync();
            query = query.Where(i => i.IsActive);
            var data = await AsyncExecuter.ToListAsync(query);
            return ObjectMapper.Map<List<ProductAttribute>, List<ProductAttributeInListDto>>(data);
        }

        public async Task<PagedResultDto<ProductAttributeInListDto>> GetListFilterAsync(BaseListFilterDto input)
        {
            var query = await Repository.GetQueryableAsync();
            query = query.WhereIf(!string.IsNullOrEmpty(input.Keyword), i => i.Label.ToLower().Contains(input.Keyword.ToLower()));

            var totalCount = await AsyncExecuter.LongCountAsync(query);
            var data = await AsyncExecuter.ToListAsync(query.Skip(input.SkipCount).Take(input.MaxResultCount));

            return new PagedResultDto<ProductAttributeInListDto>(totalCount, ObjectMapper.Map<List<ProductAttribute>, List<ProductAttributeInListDto>>(data));
        }
    }
}
