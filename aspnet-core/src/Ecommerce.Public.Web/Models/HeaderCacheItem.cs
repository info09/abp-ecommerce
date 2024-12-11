using Ecommerce.Public.Catalog.ProductCategories;
using System.Collections.Generic;

namespace Ecommerce.Public.Web.Models
{
    public class HeaderCacheItem
    {
        public List<ProductCategoryInListDto> Categories { set; get; }
    }
}
