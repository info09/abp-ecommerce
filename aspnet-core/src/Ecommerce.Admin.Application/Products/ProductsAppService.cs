using Ecommerce.Admin.Products.Attributes;
using Ecommerce.Attributes;
using Ecommerce.ProductCategories;
using Ecommerce.Products;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.BlobStoring;
using Volo.Abp.Domain.Repositories;

namespace Ecommerce.Admin.Products
{
    [Authorize]
    public class ProductsAppService : CrudAppService<Product, ProductDto, Guid, PagedResultRequestDto, CreateUpdateProductDto, CreateUpdateProductDto>, IProductsAppService
    {
        private readonly ProductManager _productManager;
        private readonly IRepository<ProductCategory> _productCategoryRepository;
        private readonly IBlobContainer<ProductThumbnailPictureContainer> _fileContainer;
        private readonly ProductCodeGenerator _productCodeGenerator;
        private readonly IRepository<ProductAttribute> _productAttributeRepository;
        private readonly IRepository<ProductAttributeDateTime> _productAttributeDateTimeRepository;
        private readonly IRepository<ProductAttributeInt> _productAttributeIntRepository;
        private readonly IRepository<ProductAttributeDecimal> _productAttributeDecimalRepository;
        private readonly IRepository<ProductAttributeVarchar> _productAttributeVarcharRepository;
        private readonly IRepository<ProductAttributeText> _productAttributeTextRepository;

        public ProductsAppService(IRepository<Product, Guid> repository, ProductManager productManager, IRepository<ProductCategory> productCategoryRepository, IBlobContainer<ProductThumbnailPictureContainer> thumbnailPictureContainer, ProductCodeGenerator productCodeGenerator, IRepository<ProductAttribute> productAttributeRepository, IRepository<ProductAttributeDateTime> productAttributeDateTimeRepository, IRepository<ProductAttributeInt> productAttributeIntRepository, IRepository<ProductAttributeDecimal> productAttributeDecimalRepository, IRepository<ProductAttributeVarchar> productAttributeVarcharRepository, IRepository<ProductAttributeText> productAttributeTextRepository) : base(repository)
        {
            _productManager = productManager;
            _productCategoryRepository = productCategoryRepository;
            _fileContainer = thumbnailPictureContainer;
            _productCodeGenerator = productCodeGenerator;
            _productAttributeRepository = productAttributeRepository;
            _productAttributeDateTimeRepository = productAttributeDateTimeRepository;
            _productAttributeIntRepository = productAttributeIntRepository;
            _productAttributeDecimalRepository = productAttributeDecimalRepository;
            _productAttributeVarcharRepository = productAttributeVarcharRepository;
            _productAttributeTextRepository = productAttributeTextRepository;
        }

        public override async Task<ProductDto> CreateAsync(CreateUpdateProductDto input)
        {
            var product = await _productManager.CreateAsync(input.ManufacturerId, input.Name, input.Code, input.Slug, input.ProductType, input.SKU, input.SortOrder, input.Visibility, input.IsActive, input.CategoryId, input.SeoMetaDescription, input.Description, input.SellPrice);

            if (input.ThumbnailPictureContent != null && input.ThumbnailPictureContent.Length > 0)
            {
                await SaveThumbnailImageAsync(input.ThumbnailPictureName, input.ThumbnailPictureContent);
                product.ThumbnailPicture = input.ThumbnailPictureName;
            }

            var result = await Repository.InsertAsync(product);

            return ObjectMapper.Map<Product, ProductDto>(result);
        }

        public override async Task<ProductDto> UpdateAsync(Guid id, CreateUpdateProductDto input)
        {
            var product = await Repository.GetAsync(id) ?? throw new BusinessException(EcommerceDomainErrorCodes.ProductIsNotExists);
            product.ManufacturerId = input.ManufacturerId;
            product.Name = input.Name;
            product.Code = input.Code;
            product.Slug = input.Slug;
            product.ProductType = input.ProductType;
            product.SKU = input.SKU;
            product.SortOrder = input.SortOrder;
            product.Visibility = input.Visibility;
            product.IsActive = input.IsActive;
            if (product.CategoryId != input.CategoryId)
            {
                product.CategoryId = input.CategoryId;
                var category = await _productCategoryRepository.GetAsync(x => x.Id == input.CategoryId);
                product.CategoryName = category.Name;
                product.CategorySlug = category.Slug;
            }
            product.SeoMetaDescription = input.SeoMetaDescription;
            product.Description = input.Description;
            if (input.ThumbnailPictureContent != null && input.ThumbnailPictureContent.Length > 0)
            {
                await SaveThumbnailImageAsync(input.ThumbnailPictureName, input.ThumbnailPictureContent);
                product.ThumbnailPicture = input.ThumbnailPictureName;
            }
            product.SellPrice = input.SellPrice;
            await Repository.UpdateAsync(product);
            return ObjectMapper.Map<Product, ProductDto>(product);


        }

        public async Task DeleteMultipleAsync(IEnumerable<Guid> ids)
        {
            await Repository.DeleteManyAsync(ids);
            await UnitOfWorkManager.Current.SaveChangesAsync();
        }

        public async Task<List<ProductInListDto>> GetListAllAsync()
        {
            var query = await Repository.GetQueryableAsync();
            query = query.Where(i => i.IsActive);
            var data = await AsyncExecuter.ToListAsync(query);
            return ObjectMapper.Map<List<Product>, List<ProductInListDto>>(data);
        }

        public async Task<PagedResultDto<ProductInListDto>> GetListFilterAsync(ProductListFilter filter)
        {
            var query = await Repository.GetQueryableAsync();
            query = query.WhereIf(!string.IsNullOrEmpty(filter.Keyword), i => i.Name.ToLower().Contains(filter.Keyword.ToLower()));

            query = query.WhereIf(filter.CategoryId.HasValue, i => i.CategoryId == filter.CategoryId.Value);

            var totalCount = await AsyncExecuter.LongCountAsync(query);
            var data = await AsyncExecuter.ToListAsync(query.OrderByDescending(i => i.CreationTime).Skip(filter.SkipCount).Take(filter.MaxResultCount));

            return new PagedResultDto<ProductInListDto>(totalCount, ObjectMapper.Map<List<Product>, List<ProductInListDto>>(data));
        }

        private async Task SaveThumbnailImageAsync(string fileName, string base64)
        {
            Regex regex = new Regex(@"^[\w/\:.-]+;base64,");
            base64 = regex.Replace(base64, string.Empty);
            byte[] bytes = Convert.FromBase64String(base64);
            await _fileContainer.SaveAsync(fileName, bytes, overrideExisting: true);
        }

        public async Task<string> GetThumbnailImageAsync(string fileName)
        {
            if (string.IsNullOrEmpty(fileName)) return null;

            var thumbnaiContent = await _fileContainer.GetAllBytesAsync(fileName);

            if (thumbnaiContent == null) return null;

            var result = Convert.ToBase64String(thumbnaiContent);
            return result;
        }

        public async Task<string> GetSuggestNewCodeAsync()
        {
            return await _productCodeGenerator.GenerateAsync();
        }

        public async Task<ProductAttributeValueDto> AddProductAttributeAsync(AddUpdateProductAttributeDto input)
        {
            var product = await Repository.GetAsync(input.ProductId) ?? throw new BusinessException(EcommerceDomainErrorCodes.ProductIsNotExists);

            var attribute = await _productAttributeRepository.GetAsync(i => i.Id == input.AttributeId) ?? throw new BusinessException(EcommerceDomainErrorCodes.ProductAttributeIdIsNotExists);

            var newAttributeId = Guid.NewGuid();
            switch (attribute.ProductAttributeType)
            {
                case ProductAttributeType.Int:
                    if (input.IntValue.HasValue)
                        throw new BusinessException(EcommerceDomainErrorCodes.ProductAttributeValueIsNotValid);

                    var productAttributeInt = new ProductAttributeInt(newAttributeId, input.AttributeId, input.ProductId, input.IntValue.Value);
                    await _productAttributeIntRepository.InsertAsync(productAttributeInt);
                    break;
                case ProductAttributeType.Varchar:
                    if (input.VarcharValue == null)
                        throw new BusinessException(EcommerceDomainErrorCodes.ProductAttributeValueIsNotValid);

                    var productAttributeVarchar = new ProductAttributeVarchar(newAttributeId, input.AttributeId, input.ProductId, input.VarcharValue);
                    await _productAttributeVarcharRepository.InsertAsync(productAttributeVarchar);
                    break;
                case ProductAttributeType.Text:
                    if (input.TextValue == null)
                        throw new BusinessException(EcommerceDomainErrorCodes.ProductAttributeValueIsNotValid);

                    var productAttributeText = new ProductAttributeText(newAttributeId, input.AttributeId, input.ProductId, input.TextValue);
                    await _productAttributeTextRepository.InsertAsync(productAttributeText);
                    break;
                case ProductAttributeType.Decimal:
                    if (input.DecimalValue.HasValue)
                        throw new BusinessException(EcommerceDomainErrorCodes.ProductAttributeValueIsNotValid);

                    var productAttributeDecimal = new ProductAttributeDecimal(newAttributeId, input.AttributeId, input.ProductId, input.DecimalValue.Value);
                    await _productAttributeDecimalRepository.InsertAsync(productAttributeDecimal);
                    break;
                case ProductAttributeType.Date:
                    if (input.DateTimeValue == null)
                        throw new BusinessException(EcommerceDomainErrorCodes.ProductAttributeValueIsNotValid);

                    var productAttributeDateTime = new ProductAttributeDateTime(newAttributeId, input.AttributeId, input.ProductId, input.DateTimeValue);
                    await _productAttributeDateTimeRepository.InsertAsync(productAttributeDateTime);
                    break;
                default:
                    break;
            }

            await UnitOfWorkManager.Current.SaveChangesAsync();
            return new ProductAttributeValueDto()
            {
                AttributeId = input.AttributeId,
                Code = attribute.Code,
                ProductAttributeType = attribute.ProductAttributeType,
                DateTimeValue = input.DateTimeValue,
                IntValue = input.IntValue,
                DecimalValue = input.DecimalValue,
                TextValue = input.TextValue,
                Id = newAttributeId,
                Label = attribute.Label,
                ProductId = input.ProductId,
                VarcharValue = input.VarcharValue,
            };
        }

        public async Task<ProductAttributeValueDto> UpdateProductAttributeAsync(Guid id, AddUpdateProductAttributeDto input)
        {
            var product = await Repository.GetAsync(input.ProductId) ?? throw new BusinessException(EcommerceDomainErrorCodes.ProductIsNotExists);
            var attribute = await _productAttributeRepository.GetAsync(x => x.Id == input.AttributeId) ?? throw new BusinessException(EcommerceDomainErrorCodes.ProductAttributeIdIsNotExists);
            switch (attribute.ProductAttributeType)
            {
                case ProductAttributeType.Date:
                    if (input.DateTimeValue == null)
                        throw new BusinessException(EcommerceDomainErrorCodes.ProductAttributeValueIsNotValid);

                    var productAttributeDateTime = await _productAttributeDateTimeRepository.GetAsync(x => x.Id == id) ?? throw new BusinessException(EcommerceDomainErrorCodes.ProductAttributeIdIsNotExists);

                    productAttributeDateTime.Value = input.DateTimeValue.Value;
                    await _productAttributeDateTimeRepository.UpdateAsync(productAttributeDateTime);
                    break;
                case ProductAttributeType.Int:
                    if (input.IntValue == null)
                        throw new BusinessException(EcommerceDomainErrorCodes.ProductAttributeValueIsNotValid);

                    var productAttributeInt = await _productAttributeIntRepository.GetAsync(x => x.Id == id) ?? throw new BusinessException(EcommerceDomainErrorCodes.ProductAttributeIdIsNotExists);

                    productAttributeInt.Value = input.IntValue.Value;
                    await _productAttributeIntRepository.UpdateAsync(productAttributeInt);
                    break;
                case ProductAttributeType.Decimal:
                    if (input.DecimalValue == null)
                        throw new BusinessException(EcommerceDomainErrorCodes.ProductAttributeValueIsNotValid);

                    var productAttributeDecimal = await _productAttributeDecimalRepository.GetAsync(x => x.Id == id) ?? throw new BusinessException(EcommerceDomainErrorCodes.ProductAttributeIdIsNotExists);

                    productAttributeDecimal.Value = input.DecimalValue.Value;
                    await _productAttributeDecimalRepository.UpdateAsync(productAttributeDecimal);
                    break;
                case ProductAttributeType.Varchar:
                    if (input.VarcharValue == null)
                        throw new BusinessException(EcommerceDomainErrorCodes.ProductAttributeValueIsNotValid);

                    var productAttributeVarchar = await _productAttributeVarcharRepository.GetAsync(x => x.Id == id) ?? throw new BusinessException(EcommerceDomainErrorCodes.ProductAttributeIdIsNotExists);

                    productAttributeVarchar.Value = input.VarcharValue;
                    await _productAttributeVarcharRepository.UpdateAsync(productAttributeVarchar);
                    break;
                case ProductAttributeType.Text:
                    if (input.TextValue == null)
                        throw new BusinessException(EcommerceDomainErrorCodes.ProductAttributeValueIsNotValid);

                    var productAttributeText = await _productAttributeTextRepository.GetAsync(x => x.Id == id) ?? throw new BusinessException(EcommerceDomainErrorCodes.ProductAttributeIdIsNotExists);

                    productAttributeText.Value = input.TextValue;
                    await _productAttributeTextRepository.UpdateAsync(productAttributeText);
                    break;
            }
            await UnitOfWorkManager.Current.SaveChangesAsync();
            return new ProductAttributeValueDto()
            {
                AttributeId = input.AttributeId,
                Code = attribute.Code,
                ProductAttributeType = attribute.ProductAttributeType,
                DateTimeValue = input.DateTimeValue,
                DecimalValue = input.DecimalValue,
                Id = id,
                IntValue = input.IntValue,
                Label = attribute.Label,
                ProductId = input.ProductId,
                TextValue = input.TextValue
            };
        }

        public async Task RemoveProductAttributeAsync(Guid attributeId, Guid id)
        {
            var attribute = await _productAttributeRepository.GetAsync(x => x.Id == attributeId) ?? throw new BusinessException(EcommerceDomainErrorCodes.ProductAttributeIdIsNotExists);
            switch (attribute.ProductAttributeType)
            {
                case ProductAttributeType.Date:
                    var productAttributeDateTime = await _productAttributeDateTimeRepository.GetAsync(x => x.Id == id) ?? throw new BusinessException(EcommerceDomainErrorCodes.ProductAttributeIdIsNotExists);

                    await _productAttributeDateTimeRepository.DeleteAsync(productAttributeDateTime);
                    break;
                case ProductAttributeType.Int:

                    var productAttributeInt = await _productAttributeIntRepository.GetAsync(x => x.Id == id) ?? throw new BusinessException(EcommerceDomainErrorCodes.ProductAttributeIdIsNotExists);

                    await _productAttributeIntRepository.DeleteAsync(productAttributeInt);
                    break;
                case ProductAttributeType.Decimal:
                    var productAttributeDecimal = await _productAttributeDecimalRepository.GetAsync(x => x.Id == id) ?? throw new BusinessException(EcommerceDomainErrorCodes.ProductAttributeIdIsNotExists);

                    await _productAttributeDecimalRepository.DeleteAsync(productAttributeDecimal);
                    break;
                case ProductAttributeType.Varchar:
                    var productAttributeVarchar = await _productAttributeVarcharRepository.GetAsync(x => x.Id == id) ?? throw new BusinessException(EcommerceDomainErrorCodes.ProductAttributeIdIsNotExists);

                    await _productAttributeVarcharRepository.DeleteAsync(productAttributeVarchar);
                    break;
                case ProductAttributeType.Text:
                    var productAttributeText = await _productAttributeTextRepository.GetAsync(x => x.Id == id) ?? throw new BusinessException(EcommerceDomainErrorCodes.ProductAttributeIdIsNotExists);

                    await _productAttributeTextRepository.DeleteAsync(productAttributeText);
                    break;
            }
            await UnitOfWorkManager.Current.SaveChangesAsync();
        }

        public async Task<List<ProductAttributeValueDto>> GetListProductAttributeAllAsync(Guid productId)
        {
            var attributeQuery = await _productAttributeRepository.GetQueryableAsync();

            var attributeDateTimeQuery = await _productAttributeDateTimeRepository.GetQueryableAsync();
            var attributeDecimalQuery = await _productAttributeDecimalRepository.GetQueryableAsync();
            var attributeIntQuery = await _productAttributeIntRepository.GetQueryableAsync();
            var attributeVarcharQuery = await _productAttributeVarcharRepository.GetQueryableAsync();
            var attributeTextQuery = await _productAttributeTextRepository.GetQueryableAsync();

            var query = from a in attributeQuery
                        join adate in attributeDateTimeQuery on a.Id equals adate.AttributeId into aDateTimeTabke
                        from adate in aDateTimeTabke.DefaultIfEmpty()
                        join adecimal in attributeDecimalQuery on a.Id equals adecimal.AttributeId into aDecimalTable
                        from adecimal in aDecimalTable.DefaultIfEmpty()
                        join aint in attributeIntQuery on a.Id equals aint.AttributeId into aIntTable
                        from aint in aIntTable.DefaultIfEmpty()
                        join aVarchar in attributeVarcharQuery on a.Id equals aVarchar.AttributeId into aVarcharTable
                        from aVarchar in aVarcharTable.DefaultIfEmpty()
                        join aText in attributeVarcharQuery on a.Id equals aText.AttributeId into aTextTable
                        from aText in aTextTable.DefaultIfEmpty()
                        where (adate != null || adate.ProductId == productId)
                        && (adecimal != null || adecimal.ProductId == productId)
                         && (aint != null || aint.ProductId == productId)
                          && (aVarchar != null || aVarchar.ProductId == productId)
                           && (aText != null || aText.ProductId == productId)
                        select new ProductAttributeValueDto()
                        {
                            Label = a.Label,
                            AttributeId = a.Id,
                            ProductAttributeType = a.ProductAttributeType,
                            Code = a.Code,
                            ProductId = productId,
                            DateTimeValue = adate.Value,
                            DecimalValue = adecimal.Value,
                            IntValue = aint.Value,
                            TextValue = aText.Value,
                            VarcharValue = aVarchar.Value,
                            DecimalId = adecimal.Id,
                            IntId = aint.Id,
                            TextId = aText.Id,
                            VarcharId = aVarchar.Id,
                        };
            return await AsyncExecuter.ToListAsync(query);
        }

        public async Task<PagedResultDto<ProductAttributeValueDto>> GetListProductAttributesAsync(ProductAttributeListFilterDto input)
        {
            var attributeQuery = await _productAttributeRepository.GetQueryableAsync();

            var attributeDateTimeQuery = await _productAttributeDateTimeRepository.GetQueryableAsync();
            var attributeDecimalQuery = await _productAttributeDecimalRepository.GetQueryableAsync();
            var attributeIntQuery = await _productAttributeIntRepository.GetQueryableAsync();
            var attributeVarcharQuery = await _productAttributeVarcharRepository.GetQueryableAsync();
            var attributeTextQuery = await _productAttributeTextRepository.GetQueryableAsync();

            var query = from a in attributeQuery
                        join adate in attributeDateTimeQuery on a.Id equals adate.AttributeId into aDateTimeTabke
                        from adate in aDateTimeTabke.DefaultIfEmpty()
                        join adecimal in attributeDecimalQuery on a.Id equals adecimal.AttributeId into aDecimalTable
                        from adecimal in aDecimalTable.DefaultIfEmpty()
                        join aint in attributeIntQuery on a.Id equals aint.AttributeId into aIntTable
                        from aint in aIntTable.DefaultIfEmpty()
                        join aVarchar in attributeVarcharQuery on a.Id equals aVarchar.AttributeId into aVarcharTable
                        from aVarchar in aVarcharTable.DefaultIfEmpty()
                        join aText in attributeVarcharQuery on a.Id equals aText.AttributeId into aTextTable
                        from aText in aTextTable.DefaultIfEmpty()
                        where (adate != null || adate.ProductId == input.ProductId)
                        && (adecimal != null || adecimal.ProductId == input.ProductId)
                         && (aint != null || aint.ProductId == input.ProductId)
                          && (aVarchar != null || aVarchar.ProductId == input.ProductId)
                           && (aText != null || aText.ProductId == input.ProductId)
                        select new ProductAttributeValueDto()
                        {
                            Label = a.Label,
                            AttributeId = a.Id,
                            ProductAttributeType = a.ProductAttributeType,
                            Code = a.Code,
                            ProductId = input.ProductId,
                            DateTimeValue = adate.Value,
                            DecimalValue = adecimal.Value,
                            IntValue = aint.Value,
                            TextValue = aText.Value,
                            VarcharValue = aVarchar.Value,
                            DecimalId = adecimal.Id,
                            IntId = aint.Id,
                            TextId = aText.Id,
                            VarcharId = aVarchar.Id,
                        };
            var totalCount = await AsyncExecuter.LongCountAsync(query);
            var data = await AsyncExecuter.ToListAsync(
                query.OrderByDescending(x => x.Label)
                .Skip(input.SkipCount)
                .Take(input.MaxResultCount)
                );
            return new PagedResultDto<ProductAttributeValueDto>(totalCount, data);
        }
    }
}
