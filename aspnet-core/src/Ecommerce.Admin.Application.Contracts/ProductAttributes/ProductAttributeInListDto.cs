﻿using Ecommerce.Attributes;
using System;
using Volo.Abp.Application.Dtos;

namespace Ecommerce.Admin.ProductAttributes
{
    public class ProductAttributeInListDto : EntityDto<Guid>
    {
        public string Code { get; set; }
        public ProductAttributeType ProductAttributeType { get; set; }
        public string Label { get; set; }
        public int SortOrder { get; set; }
        public bool Visibility { get; set; }
        public bool IsActive { get; set; }
        public bool IsRequired { get; set; }
        public bool IsUnique { get; set; }
    }
}