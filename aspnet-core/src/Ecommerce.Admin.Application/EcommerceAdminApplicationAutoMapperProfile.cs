using AutoMapper;
using Ecommerce.Admin.ProductCategories;
using Ecommerce.Admin.Products;
using Ecommerce.ProductCategories;
using Ecommerce.Products;

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
    }
}
