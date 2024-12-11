using Volo.Abp.Application.Dtos;

namespace Ecommerce.Public
{
    public class BaseListFilterDto : PagedResultRequestDto
    {
        public string Keyword { get; set; }
    }
}
