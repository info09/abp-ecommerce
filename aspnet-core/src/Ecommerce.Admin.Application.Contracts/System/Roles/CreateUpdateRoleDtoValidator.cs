using FluentValidation;

namespace Ecommerce.Admin.System.Roles
{
    public class CreateUpdateRoleDtoValidator : AbstractValidator<CreateUpdateRoleDto>
    {
        public CreateUpdateRoleDtoValidator()
        {
            RuleFor(i => i.Name).NotEmpty();
            RuleFor(i => i.Description).NotEmpty();
        }
    }
}
