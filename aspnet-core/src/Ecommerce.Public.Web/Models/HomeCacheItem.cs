using Ecommerce.Public.Catalog.ProductCategories;
using Ecommerce.Public.Catalog.Products;
using System.Collections.Generic;

namespace Ecommerce.Public.Web.Models
{
    public class HomeCacheItem
    {
        public List<ProductCategoryInListDto> Categories { set; get; }
        public List<ProductInListDto> TopSellerProducts { set; get; }
    }
}
