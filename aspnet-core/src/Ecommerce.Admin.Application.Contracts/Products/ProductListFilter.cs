using System;

namespace Ecommerce.Admin.Products
{
    public class ProductListFilter : BaseListFilterDto
    {
        public Guid? CategoryId { get; set; }
    }
}
