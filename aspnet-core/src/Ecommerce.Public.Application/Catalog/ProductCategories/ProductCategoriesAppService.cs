using Ecommerce.ProductCategories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

namespace Ecommerce.Public.Catalog.ProductCategories
{
    public class ProductCategoriesAppService : ReadOnlyAppService<ProductCategory, ProductCategoryDto, Guid, PagedResultRequestDto>, IProductCategoriesAppService
    {
        private readonly IRepository<ProductCategory, Guid> _productCategoryRepository;
        public ProductCategoriesAppService(IRepository<ProductCategory, Guid> repository, IRepository<ProductCategory, Guid> productCategoryRepository) : base(repository)
        {
            _productCategoryRepository = productCategoryRepository;
        }

        public async Task<ProductCategoryDto> GetByCodeAsync(string code)
        {
            var category = await _productCategoryRepository.GetAsync(i => i.Code == code);
            return ObjectMapper.Map<ProductCategory, ProductCategoryDto>(category);
        }

        public async Task<List<ProductCategoryInListDto>> GetListAllAsync()
        {
            var query = await Repository.GetQueryableAsync();
            query = query.Where(i => i.IsActive);
            var data = await AsyncExecuter.ToListAsync(query);
            return ObjectMapper.Map<List<ProductCategory>, List<ProductCategoryInListDto>>(data);
        }

        public async Task<PagedResult<ProductCategoryInListDto>> GetListFilterAsync(BaseListFilterDto input)
        {
            var query = await Repository.GetQueryableAsync();
            query = query.WhereIf(!string.IsNullOrEmpty(input.Keyword), i => i.Name.ToLower().Contains(input.Keyword.ToLower()));

            var totalCount = await AsyncExecuter.LongCountAsync(query);
            var data = await AsyncExecuter.ToListAsync(query.Skip((input.CurrentPage - 1) * input.PageSize).Take(input.PageSize));

            return new PagedResult<ProductCategoryInListDto>(ObjectMapper.Map<List<ProductCategory>, List<ProductCategoryInListDto>>(data), totalCount, input.CurrentPage, input.PageSize);
        }
    }
}
