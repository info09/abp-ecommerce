using Ecommerce.Admin.Permissions;
using Ecommerce.Attributes;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

namespace Ecommerce.Admin.Catalog.ProductAttributes
{
    [Authorize(EcommerceAdminPermissions.Attribute.Default, Policy = "AdminOnly")]
    public class ProductAttributeAppService : CrudAppService<ProductAttribute, ProductAttributeDto, Guid, PagedResultRequestDto, CreateUpdateProductAttributeDto, CreateUpdateProductAttributeDto>, IProductAttributesAppService
    {
        public ProductAttributeAppService(IRepository<ProductAttribute, Guid> repository) : base(repository)
        {
            GetPolicyName = EcommerceAdminPermissions.Attribute.Default;
            GetListPolicyName = EcommerceAdminPermissions.Attribute.Default;
            CreatePolicyName = EcommerceAdminPermissions.Attribute.Create;
            UpdatePolicyName = EcommerceAdminPermissions.Attribute.Update;
            DeletePolicyName = EcommerceAdminPermissions.Attribute.Delete;
        }

        [Authorize(EcommerceAdminPermissions.Attribute.Delete)]
        public async Task DeleteMultiple(IEnumerable<Guid> ids)
        {
            await Repository.DeleteManyAsync(ids);
            await UnitOfWorkManager.Current.SaveChangesAsync();
        }

        [Authorize(EcommerceAdminPermissions.Attribute.Default)]
        public async Task<List<ProductAttributeInListDto>> GetAllListAsync()
        {
            var query = await Repository.GetQueryableAsync();
            query = query.Where(i => i.IsActive);
            var data = await AsyncExecuter.ToListAsync(query);
            return ObjectMapper.Map<List<ProductAttribute>, List<ProductAttributeInListDto>>(data);
        }

        [Authorize(EcommerceAdminPermissions.Attribute.Default)]
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
