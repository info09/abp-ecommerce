import type { CreateUserDto, UpdateUserDto, UserDto, UserInListDto } from './models';
import { RestService } from '@abp/ng.core';
import type { PagedResultDto, PagedResultRequestDto } from '@abp/ng.core';
import { Injectable } from '@angular/core';
import type { BaseListFilterDto } from '../../models';

@Injectable({
  providedIn: 'root',
})
export class UserService {
  apiName = 'Default';
  

  create = (input: CreateUserDto) =>
    this.restService.request<any, UserDto>({
      method: 'POST',
      url: '/api/app/user',
      body: input,
    },
    { apiName: this.apiName });
  

  delete = (id: string) =>
    this.restService.request<any, void>({
      method: 'DELETE',
      url: `/api/app/user/${id}`,
    },
    { apiName: this.apiName });
  

  deleteMultiple = (ids: string[]) =>
    this.restService.request<any, void>({
      method: 'DELETE',
      url: '/api/app/user/multiple',
      params: { ids },
    },
    { apiName: this.apiName });
  

  get = (id: string) =>
    this.restService.request<any, UserDto>({
      method: 'GET',
      url: `/api/app/user/${id}`,
    },
    { apiName: this.apiName });
  

  getList = (input: PagedResultRequestDto) =>
    this.restService.request<any, PagedResultDto<UserDto>>({
      method: 'GET',
      url: '/api/app/user',
      params: { maxResultCount: input.maxResultCount, skipCount: input.skipCount },
    },
    { apiName: this.apiName });
  

  getListAll = (filterKeyword: string) =>
    this.restService.request<any, UserInListDto[]>({
      method: 'GET',
      url: '/api/app/user/all',
      params: { filterKeyword },
    },
    { apiName: this.apiName });
  

  getListWithFilter = (input: BaseListFilterDto) =>
    this.restService.request<any, PagedResultDto<UserInListDto>>({
      method: 'GET',
      url: '/api/app/user/with-filter',
      params: { keyword: input.keyword, skipCount: input.skipCount, maxResultCount: input.maxResultCount },
    },
    { apiName: this.apiName });
  

  update = (id: string, input: UpdateUserDto) =>
    this.restService.request<any, UserDto>({
      method: 'PUT',
      url: `/api/app/user/${id}`,
      body: input,
    },
    { apiName: this.apiName });

  constructor(private restService: RestService) {}
}