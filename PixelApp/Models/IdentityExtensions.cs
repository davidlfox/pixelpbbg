using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Web;

namespace PixelApp.Models
{
    public static class IdentityExtensions
    {
        public static bool IsImpersonating(this IPrincipal principal)
        {
            if (principal == null)
            {
                return false;
            }

            var claimsPrincipal = principal as ClaimsPrincipal;
            if (claimsPrincipal == null)
            {
                return false;
            }

            return claimsPrincipal.HasClaim("UserImpersonation", "true");
        }

        public static String GetOriginalUsername(this IPrincipal principal)
        {
            if (principal == null)
            {
                return String.Empty;
            }

            var claimsPrincipal = principal as ClaimsPrincipal;
            if (claimsPrincipal == null)
            {
                return String.Empty;
            }

            if (!claimsPrincipal.IsImpersonating())
            {
                return String.Empty;
            }

            var originalUsernameClaim = claimsPrincipal.Claims.SingleOrDefault(c => c.Type == "OriginalUsername");

            if (originalUsernameClaim == null)
            {
                return String.Empty;
            }

            return originalUsernameClaim.Value;
        }
    }
}