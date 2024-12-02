import type { ProductType } from '../ecommerce/products/product-type.enum';
import type { EntityDto } from '@abp/ng.core';
import type { BaseListFilterDto } from '../models';

export interface CreateUpdateProductDto {
  manufacturerId?: string;
  name?: string;
  code?: string;
  slug?: string;
  productType: ProductType;
  sku?: string;
  sortOrder: number;
  visiblity: boolean;
  isActive: boolean;
  sellPrice: number;
  categoryId?: string;
  seoMetaDescription?: string;
  description?: string;
  thumbnailPicture?: string;
}

export interface ProductDto {
  manufacturerId?: string;
  id?: string;
  name?: string;
  code?: string;
  slug?: string;
  productType: ProductType;
  sku?: string;
  sortOrder: number;
  visibility: boolean;
  isActive: boolean;
  categoryId?: string;
  seoMetaDescription?: string;
  description?: string;
  thumbnailPicture?: string;
  sellPrice: number;
}

export interface ProductInListDto extends EntityDto<string> {
  manufacturerId?: string;
  name?: string;
  code?: string;
  slug?: string;
  productType: ProductType;
  sku?: string;
  sortOrder: number;
  visiblity: boolean;
  isActive: boolean;
  categoryId?: string;
  thumbnailPicture?: string;
}

export interface ProductListFilter extends BaseListFilterDto {
  categoryId?: string;
}
