using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Helpers;
using IdentityModel.Client;
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
            AntiForgeryConfig.UniqueClaimTypeIdentifier = "sub";
            JwtSecurityTokenHandler.InboundClaimTypeMap = new Dictionary<string, string>();

            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = "Cookies"
            });

            app.UseOpenIdConnectAuthentication(new OpenIdConnectAuthenticationOptions
            {
                Authority = "https://idsvr.legacy/",
                ClientId = "new-portal",
                RedirectUri = "https://portal.new/",
                PostLogoutRedirectUri = "https://portal.new/",
                ResponseType = "code id_token",
                SignInAsAuthenticationType = "Cookies",
                Scope = "openid email portalprofile",
                Notifications = new OpenIdConnectAuthenticationNotifications
                {
                    AuthorizationCodeReceived = async n =>
                    {
                        var tokenClient = new TokenClient(
                            "https://idsvr.legacy/connect/token",
                            "new-portal",
                            "secret");

                        var tokenResponse = await tokenClient.RequestAuthorizationCodeAsync(
                                    n.Code, n.RedirectUri);

                        var userInfoClient = new UserInfoClient(
                            new Uri("https://idsvr.legacy/connect/userinfo"),
                            tokenResponse.AccessToken);

                        var userInfo = await userInfoClient.GetAsync();

                        var id = n.AuthenticationTicket.Identity;
                        var sub = id.FindFirst("sub");
                        var sid = id.FindFirst("sid");

                        // Create new identity with claims
                        var nid = new ClaimsIdentity(n.AuthenticationTicket.Identity.AuthenticationType);
                        nid.AddClaims(userInfo.GetClaimsIdentity().Claims);
                        nid.AddClaim(new Claim("access_token", tokenResponse.AccessToken));
                        nid.AddClaim(sub);
                        nid.AddClaim(sid);

                        // id_token is required for post logout redirect
                        nid.AddClaim(new Claim("id_token", n.ProtocolMessage.IdToken));

                        n.AuthenticationTicket = new AuthenticationTicket(nid, n.AuthenticationTicket.Properties);
                    },
                    //SecurityTokenValidated = n =>
                    //{
                    //    var id = n.AuthenticationTicket.Identity;

                    //    var nid = new ClaimsIdentity(id.AuthenticationType);

                    //    //nid.AddClaims(id.Claims);
                    //    //nid.AddClaim(id.FindFirst("preferred_username"));
                    //    nid.AddClaim(new Claim("id_token", n.ProtocolMessage.IdToken));
                        
                    //    var idProvider = id.FindFirst("http://schemas.microsoft.com/identity/claims/identityprovider");
                    //    nid.AddClaim(new Claim("http://schemas.microsoft.com/accesscontrolservice/2010/07/claims/identityprovider", idProvider.Value));

                    //    n.AuthenticationTicket = new AuthenticationTicket(
                    //        nid,
                    //        n.AuthenticationTicket.Properties);

                    //    return Task.FromResult(0);
                    //},
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
