using System.Linq;
using System.Threading.Tasks;
using IdentityServer3.Core;
using IdentityServer3.Core.Models;
using IdentityServer3.Core.Services.Default;

namespace IdSvr.New
{
    internal class UserService : UserServiceBase
    {
        public override Task AuthenticateExternalAsync(ExternalAuthenticationContext context)
        {
            var userId = context.ExternalIdentity.ProviderId;
            var name = context.ExternalIdentity.Claims.FirstOrDefault(x => x.Type == Constants.ClaimTypes.Name);

            var displayName = name == null ? context.ExternalIdentity.ProviderId : name.Value;
            var claims = context.ExternalIdentity.Claims;

            context.AuthenticateResult = new AuthenticateResult(userId, displayName, identityProvider: context.ExternalIdentity.Provider, claims: claims);

            return Task.FromResult(0);
        }

        public override Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            var claims = context.Subject.Claims;

            context.IssuedClaims = claims;

            return Task.FromResult(0);
        }

        public override Task AuthenticateLocalAsync(LocalAuthenticationContext context)
        {
            return base.AuthenticateLocalAsync(context);
        }
    }
}