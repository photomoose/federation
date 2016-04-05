using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using IdentityServer3.Core.Services;
using IdentityServer3.Core.Services.Default;

namespace IdSvr.New
{
    internal class LegacyIdSvrClaimsFilter : ClaimsFilterBase
    {
        public LegacyIdSvrClaimsFilter() : base("LegacyIdSvr")
        {
        }

        protected override IEnumerable<Claim> TransformClaims(IEnumerable<Claim> claims)
        {
            return claims.Where(x => x.Type != "id_token");
        }
    }
}