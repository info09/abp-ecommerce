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

namespace Ecommerce.Admin.System.Users
{
    public class UserAppService : CrudAppService<IdentityUser, UserDto, Guid, PagedResultRequestDto, CreateUserDto, UpdateUserDto>, IUserAppService
    {
        private readonly IdentityUserManager _identityUserManager;
        public UserAppService(IRepository<IdentityUser, Guid> repository, IdentityUserManager identityUserManager) : base(repository)
        {
            _identityUserManager = identityUserManager;
        }

        public async Task DeleteMultipleAsync(IEnumerable<Guid> ids)
        {
            await Repository.DeleteManyAsync(ids);
            await UnitOfWorkManager.Current.SaveChangesAsync();
        }

        public async Task<List<UserInListDto>> GetListAllAsync(string filterKeyword)
        {
            var query = await Repository.GetQueryableAsync();
            if (!string.IsNullOrEmpty(filterKeyword))
            {
                query = query.Where(o => o.Name.ToLower().Contains(filterKeyword)
              || o.Email.ToLower().Contains(filterKeyword)
              || o.PhoneNumber.ToLower().Contains(filterKeyword));
            }
            var data = await AsyncExecuter.ToListAsync(query);
            return ObjectMapper.Map<List<IdentityUser>, List<UserInListDto>>(data);
        }

        public async Task<PagedResultDto<UserInListDto>> GetListWithFilterAsync(BaseListFilterDto input)
        {
            var query = await Repository.GetQueryableAsync();
            if (!string.IsNullOrEmpty(input.Keyword))
            {
                query = query.Where(i => i.Name.ToLower().Contains(input.Keyword.ToLower()) ||
                                                    i.Email.ToLower().Contains(input.Keyword.ToLower()) ||
                                                    i.PhoneNumber.ToLower().Contains(input.Keyword.ToLower()));
            }

            query = query.OrderByDescending(i => i.CreationTime);

            var totalCount = await AsyncExecuter.LongCountAsync(query);
            var data = await AsyncExecuter.ToListAsync(query);

            return new PagedResultDto<UserInListDto>(totalCount, ObjectMapper.Map<List<IdentityUser>, List<UserInListDto>>(data));
        }

        public override async Task<UserDto> CreateAsync(CreateUserDto input)
        {
            var query = await Repository.GetQueryableAsync();
            var isNameExisted = query.Any(i => i.Name == input.Name);
            if (isNameExisted)
                throw new UserFriendlyException("Tài khoản đã tồn tại");

            var isEmailExisted = query.Any(email => email.Email == input.Email);
            if (isEmailExisted)
                throw new UserFriendlyException("Email đã tồn tại");

            var userId = Guid.NewGuid();
            var user = new IdentityUser(userId, input.UserName, input.Email);
            user.Name = input.Name;
            user.Surname = input.Surname;
            user.SetPhoneNumber(input.PhoneNumber, true);
            var result = await _identityUserManager.CreateAsync(user, input.Password);
            if (result.Succeeded)
            {
                return ObjectMapper.Map<IdentityUser, UserDto>(user);
            }
            else
            {
                List<Microsoft.AspNetCore.Identity.IdentityError> errorList = result.Errors.ToList();
                string errors = "";
                foreach (var error in errorList)
                {
                    errors += error.Description.ToString();
                }
                throw new UserFriendlyException(errors);
            }
        }

        public async override Task<UserDto> UpdateAsync(Guid id, UpdateUserDto input)
        {
            var user = await _identityUserManager.FindByIdAsync(id.ToString()) ?? throw new EntityNotFoundException(typeof(IdentityUser), id);
            user.Name = input.Name;
            user.SetPhoneNumber(input.PhoneNumber, true);
            user.Surname = input.Surname;
            var result = await _identityUserManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                return ObjectMapper.Map<IdentityUser, UserDto>(user);
            }
            else
            {
                List<Microsoft.AspNetCore.Identity.IdentityError> errorList = result.Errors.ToList();
                string errors = "";
                foreach (var error in errorList)
                {
                    errors += error.Description.ToString();
                }
                throw new UserFriendlyException(errors);
            }
        }

        public override async Task<UserDto> GetAsync(Guid id)
        {
            var user = await _identityUserManager.FindByIdAsync(id.ToString()) ?? throw new EntityNotFoundException(typeof(IdentityUser), id);

            var userDto = ObjectMapper.Map<IdentityUser, UserDto>(user);
            var roles = await _identityUserManager.GetRolesAsync(user);
            userDto.Roles = roles;
            return userDto;
        }

        public async Task AssignRolesAsync(Guid userId, string[] roleNames)
        {
            var user = await _identityUserManager.FindByIdAsync(userId.ToString()) ?? throw new EntityNotFoundException(typeof(IdentityUser), userId);

            var currentRoles = await _identityUserManager.GetRolesAsync(user);
            var removedResult = await _identityUserManager.RemoveFromRolesAsync(user, currentRoles);
            var addedResult = await _identityUserManager.AddToRolesAsync(user, currentRoles);

            if (!addedResult.Succeeded || !removedResult.Succeeded)
            {
                List<Microsoft.AspNetCore.Identity.IdentityError> addedErrorList = addedResult.Errors.ToList();
                List<Microsoft.AspNetCore.Identity.IdentityError> removedErrorList = removedResult.Errors.ToList();
                var errorList = new List<Microsoft.AspNetCore.Identity.IdentityError>();
                errorList.AddRange(addedErrorList);
                errorList.AddRange(removedErrorList);
                string errors = "";
                foreach (var error in errorList)
                {
                    errors = errors + error.Description.ToString();
                }
                throw new UserFriendlyException(errors);
            }
        }
    }
}
