using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Protocols;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OpenIdConnect;
using Owin;

[assembly: OwinStartup(typeof(NewPortal.Startup))]

namespace NewPortal
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = "Cookies"
            });

            app.UseOpenIdConnectAuthentication(new OpenIdConnectAuthenticationOptions
            {
                Authority = "https://idsvr.new/core/",
                ClientId = "new-portal",
                RedirectUri = "https://portal.new/",
                PostLogoutRedirectUri = "https://portal.new/",
                ResponseType = "id_token",
                SignInAsAuthenticationType = "Cookies",
                Scope = "openid profile email portal",
                Notifications = new OpenIdConnectAuthenticationNotifications
                {
                    SecurityTokenValidated = async n =>
                    {
                        var id = n.AuthenticationTicket.Identity;

                        var nid = new ClaimsIdentity(
                            id.AuthenticationType,
                            ClaimTypes.GivenName,
                            ClaimTypes.Role);

                        nid.AddClaims(id.Claims);
                        nid.AddClaim(new Claim("id_token", n.ProtocolMessage.IdToken));
                        
                        var idProvider = id.FindFirst("http://schemas.microsoft.com/identity/claims/identityprovider");
                        nid.AddClaim(new Claim("http://schemas.microsoft.com/accesscontrolservice/2010/07/claims/identityprovider", idProvider.Value));

                        n.AuthenticationTicket = new AuthenticationTicket(
                            nid,
                            n.AuthenticationTicket.Properties);
                    },
                    RedirectToIdentityProvider = n =>
                    {
                        // if signing out, add the id_token_hint
                        if (n.ProtocolMessage.RequestType == OpenIdConnectRequestType.LogoutRequest)
                        {
                            var idTokenHint = n.OwinContext.Authentication.User.FindFirst("id_token");

                            if (idTokenHint != null)
                            {
                                n.ProtocolMessage.IdTokenHint = idTokenHint.Value;
                            }

                        }

                        return Task.FromResult(0);
                    }
                }
            });
        }
    }
}
