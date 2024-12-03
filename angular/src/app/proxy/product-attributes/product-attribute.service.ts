import type { CreateUpdateProductAttributeDto, ProductAttributeDto, ProductAttributeInListDto } from './models';
import { RestService } from '@abp/ng.core';
import type { PagedResultDto, PagedResultRequestDto } from '@abp/ng.core';
import { Injectable } from '@angular/core';
import type { BaseListFilterDto } from '../models';

@Injectable({
  providedIn: 'root',
})
export class ProductAttributeService {
  apiName = 'Default';
  

  create = (input: CreateUpdateProductAttributeDto) =>
    this.restService.request<any, ProductAttributeDto>({
      method: 'POST',
      url: '/api/app/product-attribute',
      body: input,
    },
    { apiName: this.apiName });
  

  delete = (id: string) =>
    this.restService.request<any, void>({
      method: 'DELETE',
      url: `/api/app/product-attribute/${id}`,
    },
    { apiName: this.apiName });
  

  deleteMultipleByIds = (ids: string[]) =>
    this.restService.request<any, void>({
      method: 'DELETE',
      url: '/api/app/product-attribute/multiple',
      params: { ids },
    },
    { apiName: this.apiName });
  

  get = (id: string) =>
    this.restService.request<any, ProductAttributeDto>({
      method: 'GET',
      url: `/api/app/product-attribute/${id}`,
    },
    { apiName: this.apiName });
  

  getAllList = () =>
    this.restService.request<any, ProductAttributeInListDto[]>({
      method: 'GET',
      url: '/api/app/product-attribute/list',
    },
    { apiName: this.apiName });
  

  getList = (input: PagedResultRequestDto) =>
    this.restService.request<any, PagedResultDto<ProductAttributeDto>>({
      method: 'GET',
      url: '/api/app/product-attribute',
      params: { maxResultCount: input.maxResultCount, skipCount: input.skipCount },
    },
    { apiName: this.apiName });
  

  getListFilter = (input: BaseListFilterDto) =>
    this.restService.request<any, PagedResultDto<ProductAttributeInListDto>>({
      method: 'GET',
      url: '/api/app/product-attribute/filter',
      params: { skipCount: input.skipCount, maxResultCount: input.maxResultCount, keyword: input.keyword },
    },
    { apiName: this.apiName });
  

  update = (id: string, input: CreateUpdateProductAttributeDto) =>
    this.restService.request<any, ProductAttributeDto>({
      method: 'PUT',
      url: `/api/app/product-attribute/${id}`,
      body: input,
    },
    { apiName: this.apiName });

  constructor(private restService: RestService) {}
}
