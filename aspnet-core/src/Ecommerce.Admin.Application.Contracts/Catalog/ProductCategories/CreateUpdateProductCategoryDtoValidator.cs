using FluentValidation;

namespace Ecommerce.Admin.Catalog.ProductCategories
{
    public class CreateUpdateProductCategoryDtoValidator : AbstractValidator<CreateUpdateProductCategoryDto>
    {
        public CreateUpdateProductCategoryDtoValidator()
        {
            RuleFor(i => i.Name).NotEmpty().MaximumLength(50);
            RuleFor(i => i.Code).NotEmpty().MaximumLength(50);
            RuleFor(i => i.Slug).NotEmpty().MaximumLength(50);
            RuleFor(x => x.CoverPicture).MaximumLength(250);
            RuleFor(x => x.SeoMetaDescription).MaximumLength(250);
        }
    }
}
