using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Helpers;
using IdentityModel.Client;
using LegacyPortal;
using Microsoft.AspNet.Identity;
using Microsoft.IdentityModel.Protocols;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OpenIdConnect;
using Owin;

[assembly: OwinStartup(typeof (Startup))]

namespace LegacyPortal
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            AntiForgeryConfig.UniqueClaimTypeIdentifier = "sub";
            JwtSecurityTokenHandler.InboundClaimTypeMap = new Dictionary<string, string>();

            ConfigureAuth(app);

            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = "Cookies"
            });

            app.UseOpenIdConnectAuthentication(new OpenIdConnectAuthenticationOptions
            {
                Authority = "https://idsvr.legacy",
                ClientId = "legacy-portal",
                RedirectUri = "https://portal.legacy/",
                PostLogoutRedirectUri = "https://portal.legacy/",
                ResponseType = "code id_token",
                SignInAsAuthenticationType = "Cookies",
                Scope = "openid email portalprofile",
                Notifications = new OpenIdConnectAuthenticationNotifications
                {
                    AuthorizationCodeReceived = async n =>
                    {
                        // use the code to get the access and refresh token
                        var tokenClient = new TokenClient(
                            "https://idsvr.legacy/connect/token",
                            "legacy-portal",
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

                        // NameIdentifier required for ASP.NET Identity (for Legacy Portal)
                        // Perhaps this needs to go into client-specific claims provider?
                        nid.AddClaim(new Claim(ClaimTypes.NameIdentifier, sub.Value));
                        
                        n.AuthenticationTicket = new AuthenticationTicket(nid, n.AuthenticationTicket.Properties);
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