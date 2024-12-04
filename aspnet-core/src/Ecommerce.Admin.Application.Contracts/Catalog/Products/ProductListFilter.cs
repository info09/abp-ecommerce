using System;

namespace Ecommerce.Admin.Catalog.Products
{
    public class ProductListFilter : BaseListFilterDto
    {
        public Guid? CategoryId { get; set; }
    }
}
