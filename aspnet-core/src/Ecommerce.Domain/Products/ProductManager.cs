using Ecommerce.ProductCategories;
using System;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;

namespace Ecommerce.Products
{
    public class ProductManager : DomainService
    {
        private readonly IRepository<Product, Guid> _productRepository;
        private readonly IRepository<ProductCategory, Guid> _productCategoryRepository;

        public ProductManager(IRepository<Product, Guid> productRepository, IRepository<ProductCategory, Guid> productCategoryRepository)
        {
            _productRepository = productRepository;
            _productCategoryRepository = productCategoryRepository;
        }

        public async Task<Product> CreateAsync(Guid manufacturerId, string name, string code, string slug, ProductType productType, string sku, int sortOrder, bool visibility, bool isActive, Guid categoryId, string seoMetaDescription, string description, double sellPrice)
        {
            if (await _productRepository.AnyAsync(i => i.Name == name))
                throw new UserFriendlyException("Tên sản phẩm đã tồn tại", EcommerceDomainErrorCodes.ProductNameAlreadyExists);
            if (await _productRepository.AnyAsync(i => i.Code == code))
                throw new UserFriendlyException("Mã sản phẩm đã tồn tại", EcommerceDomainErrorCodes.ProductCodeAlreadyExists);
            if (await _productRepository.AnyAsync(i => i.SKU == sku))
                throw new UserFriendlyException("SKU sản phẩm đã tồn tại", EcommerceDomainErrorCodes.ProductSKUAlreadyExists);

            var category = await _productCategoryRepository.GetAsync(categoryId);
            return new Product(Guid.NewGuid(), manufacturerId, name, code, slug, productType, sku, sortOrder, visibility, isActive, categoryId, seoMetaDescription, description, null, sellPrice, category.Name, category.Slug);
        }
    }
}
