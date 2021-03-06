﻿using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
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
                ResponseType = "code id_token",
                SignInAsAuthenticationType = "Cookies",
                Scope = "openid email portalprofile",
                Notifications = new OpenIdConnectAuthenticationNotifications
                {
                    AuthorizationCodeReceived = async n =>
                    {
                        var tokenClient = new TokenClient(
                            "https://idsvr.new/core/connect/token",
                            "new-portal",
                            "secret");

                        var tokenResponse = await tokenClient.RequestAuthorizationCodeAsync(
                                    n.Code, n.RedirectUri);

                        var userInfoClient = new UserInfoClient(
                            new Uri("https://idsvr.new/core/connect/userinfo"),
                            tokenResponse.AccessToken);

                        var userInfo = await userInfoClient.GetAsync();

                        // Oh dear, we have no claims because IdSvr.New has no user store!
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
