import type { ProductAttributeType } from '../ecommerce/attributes/product-attribute-type.enum';
import type { EntityDto } from '@abp/ng.core';

export interface CreateUpdateProductAttributeDto {
  code?: string;
  productAttributeType: ProductAttributeType;
  label?: string;
  sortOrder: number;
  visibility: boolean;
  isActive: boolean;
  isRequired: boolean;
  isUnique: boolean;
  note?: string;
}

export interface ProductAttributeDto {
  code?: string;
  productAttributeType: ProductAttributeType;
  label?: string;
  sortOrder: number;
  visibility: boolean;
  isActive: boolean;
  isRequired: boolean;
  isUnique: boolean;
  note?: string;
  id?: string;
}

export interface ProductAttributeInListDto extends EntityDto<string> {
  code?: string;
  productAttributeType: ProductAttributeType;
  label?: string;
  sortOrder: number;
  visibility: boolean;
  isActive: boolean;
  isRequired: boolean;
  isUnique: boolean;
}
