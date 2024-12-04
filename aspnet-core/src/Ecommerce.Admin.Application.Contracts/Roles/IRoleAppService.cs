using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.PermissionManagement;

namespace Ecommerce.Admin.Roles
{
    public interface IRoleAppService : ICrudAppService<RoleDto, Guid, PagedResultRequestDto, CreateUpdateRoleDto, CreateUpdateRoleDto>
    {
        Task<PagedResultDto<RoleInListDto>> GetListFilterAsync(BaseListFilterDto input);
        Task<List<RoleInListDto>> GetListAllAsync();
        Task DeleteMultiple(IEnumerable<Guid> ids);
        Task<GetPermissionListResultDto> GetPermissionsAsync(string providerName, string providerKey);
        Task UpdatePermissionsAsync(string providerName, string providerKey, UpdatePermissionsDto input);
    }
}
