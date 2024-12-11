﻿using Ecommerce.Admin.Permissions;
using Ecommerce.ProductCategories;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

namespace Ecommerce.Admin.Catalog.ProductCategories
{
    [Authorize(EcommerceAdminPermissions.ProductCategory.Default, Policy = "AdminOnly")]
    public class ProductCategoriesAppService : CrudAppService<ProductCategory, ProductCategoryDto, Guid, PagedResultRequestDto, CreateUpdateProductCategoryDto, CreateUpdateProductCategoryDto>, IProductCategoriesAppService
    {
        public ProductCategoriesAppService(IRepository<ProductCategory, Guid> repository) : base(repository)
        {
            GetPolicyName = EcommerceAdminPermissions.ProductCategory.Default;
            GetListPolicyName = EcommerceAdminPermissions.ProductCategory.Default;
            CreatePolicyName = EcommerceAdminPermissions.ProductCategory.Create;
            UpdatePolicyName = EcommerceAdminPermissions.ProductCategory.Update;
            DeletePolicyName = EcommerceAdminPermissions.ProductCategory.Delete;
        }

        [Authorize(EcommerceAdminPermissions.ProductCategory.Delete)]
        public async Task DeleteMultipleAsync(IEnumerable<Guid> ids)
        {
            await Repository.DeleteManyAsync(ids);
            await UnitOfWorkManager.Current.SaveChangesAsync();
        }

        [Authorize(EcommerceAdminPermissions.ProductCategory.Default)]
        public async Task<List<ProductCategoryInListDto>> GetListAllAsync()
        {
            var query = await Repository.GetQueryableAsync();
            query = query.Where(i => i.IsActive);
            var data = await AsyncExecuter.ToListAsync(query);
            return ObjectMapper.Map<List<ProductCategory>, List<ProductCategoryInListDto>>(data);
        }

        [Authorize(EcommerceAdminPermissions.ProductCategory.Default)]
        public async Task<PagedResultDto<ProductCategoryInListDto>> GetListFilterAsync(BaseListFilterDto input)
        {
            var query = await Repository.GetQueryableAsync();
            query = query.WhereIf(!string.IsNullOrEmpty(input.Keyword), i => i.Name.ToLower().Contains(input.Keyword.ToLower()));

            var totalCount = await AsyncExecuter.LongCountAsync(query);
            var data = await AsyncExecuter.ToListAsync(query.Skip(input.SkipCount).Take(input.MaxResultCount));

            return new PagedResultDto<ProductCategoryInListDto>(totalCount, ObjectMapper.Map<List<ProductCategory>, List<ProductCategoryInListDto>>(data));
        }
    }
}
