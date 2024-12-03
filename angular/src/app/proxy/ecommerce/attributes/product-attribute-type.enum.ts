import { mapEnumToOptions } from '@abp/ng.core';

export enum ProductAttributeType {
  Int = 1,
  Varchar = 2,
  Text = 3,
  Decimal = 4,
  Date = 5,
}

export const productAttributeTypeOptions = mapEnumToOptions(ProductAttributeType);
