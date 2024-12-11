using System;

namespace Ecommerce.Public.Catalog.Products
{
    public class ProductListFilter : BaseListFilterDto
    {
        public Guid? CategoryId { get; set; }
    }
}
