using Volo.Abp.DependencyInjection;
using Volo.Abp.Ui.Branding;

namespace Ecommerce.Public.Web;

[Dependency(ReplaceServices = true)]
public class EcommercePublicBrandingProvider : DefaultBrandingProvider
{
    public override string AppName => "Public";
}
