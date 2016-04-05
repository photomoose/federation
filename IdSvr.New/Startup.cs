using System.Collections.Generic;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using IdentityServer3.Core.Configuration;
using IdentityServer3.Core.Extensions;
using IdentityServer3.Core.Services;
using Microsoft.IdentityModel.Protocols;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Google;
using Microsoft.Owin.Security.OpenIdConnect;
using Owin;
using Serilog;
using AuthenticationOptions = IdentityServer3.Core.Configuration.AuthenticationOptions;

namespace IdSvr.New
{
    class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            Log.Logger = new LoggerConfiguration()
                .WriteTo.File(@"c:\temp\idsvr.new.log")
                .MinimumLevel.Verbose()
                .CreateLogger();

            app.Map("/core", coreApp =>
            {
                var factory = new IdentityServerServiceFactory()
                    .UseInMemoryClients(Clients.Get())
                    .UseInMemoryScopes(Scopes.Get())
                    .UseInMemoryUsers(Users.Get());
                factory.UserService = new Registration<IUserService, UserService>();

                var options = new IdentityServerOptions
                {
                    Factory = factory,
                    SigningCertificate = LoadCertificate(),
                    AuthenticationOptions = new AuthenticationOptions
                    {
                        IdentityProviders = ConfigureIdentityProviders,
                        EnableLocalLogin = false,
                        EnableSignOutPrompt = false,
                        EnablePostSignOutAutoRedirect = true
                    }
                };

                coreApp.UseIdentityServer(options);

                coreApp.Map("/signoutcallback", cleanup =>
                {
                    cleanup.Run(async ctx =>
                    {
                        var state = ctx.Request.Cookies["state"];
                        await ctx.Environment.RenderLoggedOutViewAsync(state);
                    });
                });
            });


        }

        private void ConfigureIdentityProviders(IAppBuilder app, string signInAsType)
        {
            var option = new OpenIdConnectAuthenticationOptions
            {
                Authority = "https://idsvr.legacy/",
                ClientId = "new-idsvr",
                Scope = "openid",
                RedirectUri = "https://idsvr.new/core/",
                PostLogoutRedirectUri = "https://idsvr.new/core/signoutcallback/",
                ResponseType = "id_token",
                SignInAsAuthenticationType = signInAsType,
                Caption = "Legacy Reports Portal",
                AuthenticationType = "LegacyIdSvr",
                Notifications = new OpenIdConnectAuthenticationNotifications
                {
                    SecurityTokenValidated = async n =>
                    {
                        var id = n.AuthenticationTicket.Identity;

                        var nid = new ClaimsIdentity(id.AuthenticationType);

                        nid.AddClaims(id.Claims);

                        // Add id_token to allow post logout redirect
                        nid.AddClaim(new Claim("id_token", n.ProtocolMessage.IdToken));

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

                            var signOutMessageId = n.OwinContext.Environment.GetSignOutMessageId();
                            if (signOutMessageId != null)
                            {
                                n.OwinContext.Response.Cookies.Append("state", signOutMessageId);
                            }

                        }

                        return Task.FromResult(0);
                    }
                }
            };

            app.UseOpenIdConnectAuthentication(option);

            //app.UseGoogleAuthentication(new GoogleOAuth2AuthenticationOptions
            //{
            //    ClientId = "485477875434-rh9frqr45ae9hlgo2f1ctre5g1encr63.apps.googleusercontent.com",
            //    ClientSecret = "rMzlAt80YFsz5i9cFUlJNZEJ",
            //    SignInAsAuthenticationType = signInAsType
            //});
        }

        X509Certificate2 LoadCertificate()
        {
            return new X509Certificate2(@"C:\Development\FederatedIdentityServer\idsrv3test.pfx", "idsrv3test");
        }
    }
}