using AutoMapper;
using Ecommerce.Attributes;
using Ecommerce.Manufacturers;
using Ecommerce.ProductCategories;
using Ecommerce.Products;
using Ecommerce.Public.Catalog.Manufacturers;
using Ecommerce.Public.Catalog.ProductAttributes;
using Ecommerce.Public.Catalog.ProductCategories;
using Ecommerce.Public.Catalog.Products;

namespace Ecommerce.Public;

public class EcommercePublicApplicationAutoMapperProfile : Profile
{
    public EcommercePublicApplicationAutoMapperProfile()
    {
        /* You can configure your AutoMapper mapping configuration here.
         * Alternatively, you can split your mapping configurations
         * into multiple profile classes for a better organization. */
        //Product Category
        CreateMap<ProductCategory, ProductCategoryDto>();
        CreateMap<ProductCategory, ProductCategoryInListDto>();
        //Product
        CreateMap<Product, ProductDto>();
        CreateMap<Product, ProductInListDto>();
        CreateMap<Manufacturer, ManufacturerDto>();
        CreateMap<Manufacturer, ManufacturerInListDto>();
        //Product attribute
        CreateMap<ProductAttribute, ProductAttributeDto>();
        CreateMap<ProductAttribute, ProductAttributeInListDto>();
    }
}
