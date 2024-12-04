using AutoMapper;
using Ecommerce.Admin.Manufacturers;
using Ecommerce.Admin.ProductAttributes;
using Ecommerce.Admin.ProductCategories;
using Ecommerce.Admin.Products;
using Ecommerce.Admin.Roles;
using Ecommerce.Attributes;
using Ecommerce.Manufacturers;
using Ecommerce.ProductCategories;
using Ecommerce.Products;
using Ecommerce.Roles;
using Volo.Abp.Identity;

namespace Ecommerce.Admin;

public class EcommerceAdminApplicationAutoMapperProfile : Profile
{
    public EcommerceAdminApplicationAutoMapperProfile()
    {
        /* You can configure your AutoMapper mapping configuration here.
         * Alternatively, you can split your mapping configurations
         * into multiple profile classes for a better organization. */

        CreateMap<ProductCategory, ProductCategoryDto>();
        CreateMap<ProductCategory, ProductCategoryInListDto>();
        CreateMap<CreateUpdateProductCategoryDto, ProductCategory>();

        //Product
        CreateMap<Product, ProductDto>();
        CreateMap<Product, ProductInListDto>();
        CreateMap<CreateUpdateProductDto, Product>();

        CreateMap<Manufacturer, ManufacturerDto>();
        CreateMap<Manufacturer, ManufacturerInListDto>();
        CreateMap<CreateUpdateManufacturerDto, Manufacturer>();

        //Product attribute
        CreateMap<ProductAttribute, ProductAttributeDto>();
        CreateMap<ProductAttribute, ProductAttributeInListDto>();
        CreateMap<CreateUpdateProductAttributeDto, ProductAttribute>();

        //Role
        CreateMap<IdentityRole, RoleDto>().ForMember(i => i.Description,
            map => map.MapFrom(x => x.ExtraProperties.ContainsKey(RoleConsts.DescriptionFieldName) ?
            x.ExtraProperties[RoleConsts.DescriptionFieldName] :
            null));
        CreateMap<CreateUpdateRoleDto, IdentityRole>();
    }
}
