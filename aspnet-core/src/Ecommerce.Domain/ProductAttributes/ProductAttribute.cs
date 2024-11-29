using System;
using Volo.Abp.Domain.Entities.Auditing;

namespace Ecommerce.Attributes
{
    public class ProductAttribute : CreationAuditedAggregateRoot<Guid>
    {
        public string Code { get; set; }
        public ProductAttributeType ProductAttributeType { get; set; }
        public string Label { get; set; }
        public string SortOrder { get; set; }
        public bool Visibility { get; set; }
        public bool IsActive { get; set; }
        public bool IsRequired { get; set; }
        public bool IsUnique { get; set; }
        public string Note { get; set; }
    }
}
