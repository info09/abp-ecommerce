using Ecommerce.Public.Catalog.ProductCategories;
using Ecommerce.Public.Catalog.Products;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Ecommerce.Public.Web.Pages.Product
{
    public class CategoryModel : PageModel
    {
        private readonly IProductCategoriesAppService _productCategoriesAppService;
        private readonly IProductsAppService _productsAppService;

        public CategoryModel(IProductCategoriesAppService productCategoriesAppService, IProductsAppService productsAppService)
        {
            _productCategoriesAppService = productCategoriesAppService;
            _productsAppService = productsAppService;
        }

        public ProductCategoryDto Category { set; get; }
        public List<ProductCategoryInListDto> Categories { set; get; }
        public PagedResult<ProductInListDto> ProductData { set; get; }

        public async Task OnGetAsync(string code, int page = 1)
        {
            Category = await _productCategoriesAppService.GetByCodeAsync(code);
            Categories = await _productCategoriesAppService.GetListAllAsync();
            ProductData = await _productsAppService.GetListFilterAsync(new ProductListFilter()
            {
                CategoryId = Category.Id,
                CurrentPage = page
            });
        }
    }
}
