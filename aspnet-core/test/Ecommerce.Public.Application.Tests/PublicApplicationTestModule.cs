using Volo.Abp.Modularity;

namespace Ecommerce.Public;

[DependsOn(
    typeof(EcommercePublicApplicationModule),
    typeof(EcommerceDomainTestModule)
    )]
public class PublicApplicationTestModule : AbpModule
{

}
