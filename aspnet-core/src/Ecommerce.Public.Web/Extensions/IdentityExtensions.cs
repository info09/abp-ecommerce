using System;
using System.Linq;
using System.Security.Claims;

namespace Ecommerce.Public.Web.Extensions
{
    public static class IdentityExtensions
    {
        public static string GetSpecificClaim(this ClaimsPrincipal claimsPrincipal, string claimType)
        {
            var claim = ((ClaimsIdentity)claimsPrincipal.Identity)?.Claims.FirstOrDefault(i => i.Type == claimType);
            return (claim != null) ? claim.Value : string.Empty;
        }

        public static Guid GetUserId(this ClaimsPrincipal claimsPrincipal)
        {
            var subjectId = claimsPrincipal.GetSpecificClaim(ClaimTypes.NameIdentifier);
            return Guid.Parse(subjectId);
        }
    }
}
