﻿using Ecommerce.Localization;
using Volo.Abp.Application.Services;

namespace Ecommerce.Public;

/* Inherit your application services from this class.
 */
public abstract class EcommercePublicAppService : ApplicationService
{
    protected EcommercePublicAppService()
    {
        LocalizationResource = typeof(EcommerceResource);
    }
}