using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace Ecommerce.Admin.Roles
{
    public interface IRoleAppService : ICrudAppService<RoleDto, Guid, PagedResultRequestDto, CreateUpdateRoleDto, CreateUpdateRoleDto>
    {
        Task<PagedResultDto<RoleInListDto>> GetListFilterAsync(BaseListFilterDto input);
        Task<List<RoleInListDto>> GetListAllAsync();
        Task DeleteMultiple(IEnumerable<Guid> ids);
    }
}
