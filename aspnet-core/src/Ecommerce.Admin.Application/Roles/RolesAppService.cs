﻿using Ecommerce.Roles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Identity;

namespace Ecommerce.Admin.Roles
{
    public class RolesAppService : CrudAppService<IdentityRole, RoleDto, Guid, PagedResultRequestDto, CreateUpdateRoleDto, CreateUpdateRoleDto>, IRoleAppService
    {
        public RolesAppService(IRepository<IdentityRole, Guid> repository) : base(repository)
        {
        }

        public async Task DeleteMultiple(IEnumerable<Guid> ids)
        {
            await Repository.DeleteManyAsync(ids);
            await UnitOfWorkManager.Current.SaveChangesAsync();
        }

        public async Task<List<RoleInListDto>> GetListAllAsync()
        {
            var query = await Repository.GetQueryableAsync();
            var data = await AsyncExecuter.ToListAsync(query);
            return ObjectMapper.Map<List<IdentityRole>, List<RoleInListDto>>(data);
        }

        public async Task<PagedResultDto<RoleInListDto>> GetListFilterAsync(BaseListFilterDto input)
        {
            var query = await Repository.GetQueryableAsync();
            query = query.WhereIf(!string.IsNullOrWhiteSpace(input.Keyword), x => x.Name.Contains(input.Keyword));
            var totalCount = await AsyncExecuter.LongCountAsync(query);
            var data = await AsyncExecuter.ToListAsync(query.Skip(input.SkipCount).Take(input.MaxResultCount));
            return new PagedResultDto<RoleInListDto>(totalCount, ObjectMapper.Map<List<IdentityRole>, List<RoleInListDto>>(data));
        }

        public override async Task<RoleDto> CreateAsync(CreateUpdateRoleDto input)
        {
            var query = await Repository.GetQueryableAsync();
            var isNameExisted = query.Any(i => i.Name == input.Name);
            if (isNameExisted)
                throw new BusinessException(EcommerceDomainErrorCodes.RoleNameAlreadyExists).WithData("Name", input.Name);

            var role = new IdentityRole(Guid.NewGuid(), input.Name);
            role.ExtraProperties[RoleConsts.DescriptionFieldName] = input.Description;
            var data = await Repository.InsertAsync(role);
            await UnitOfWorkManager.Current.SaveChangesAsync();
            return ObjectMapper.Map<IdentityRole, RoleDto>(data);
        }

        public override async Task<RoleDto> UpdateAsync(Guid id, CreateUpdateRoleDto input)
        {
            var role = await Repository.GetAsync(id) ?? throw new EntityNotFoundException(typeof(IdentityRole), id);

            var query = await Repository.GetQueryableAsync();
            var isNameExisted = query.Any(i => i.Name == input.Name);
            if (isNameExisted)
                throw new BusinessException(EcommerceDomainErrorCodes.RoleNameAlreadyExists).WithData("Name", input.Name);

            role.ExtraProperties[RoleConsts.DescriptionFieldName] = input.Description;
            var data = await Repository.UpdateAsync(role);
            await UnitOfWorkManager.Current.SaveChangesAsync();
            return ObjectMapper.Map<IdentityRole, RoleDto>(data);
        }
    }
}
