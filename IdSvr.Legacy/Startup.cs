using System;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using IdentityManager.Configuration;
using IdentityManager.Core.Logging.LogProviders;
using IdentityServer3.Core.Configuration;
using IdentityServer3.Core.Logging;
using IdentityServer3.Core.Resources;
using Microsoft.Owin;
using Owin;
using Serilog;

[assembly: OwinStartup(typeof(IdSvr.Legacy.Startup))]

namespace IdSvr.Legacy
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            Log.Logger = new LoggerConfiguration()
                .WriteTo.File(@"c:\temp\idsvr.legacy.log")
                .MinimumLevel.Verbose()
                .CreateLogger();

            app.Map("/admin", adminApp =>
            {
                var factory = new IdentityManagerServiceFactory();
                factory.ConfigureSimpleIdentityManagerService("Identity");

                adminApp.UseIdentityManager(new IdentityManagerOptions()
                {
                    SecurityConfiguration = new LocalhostSecurityConfiguration() { RequireSsl = false },
                    Factory = factory
                });
            });

            var idSvrFactory = new IdentityServerServiceFactory()
                .UseInMemoryClients(Clients.Get())
                .UseInMemoryScopes(Scopes.Get());

            idSvrFactory.ConfigureUserService("Identity");

            var options = new IdentityServerOptions
            {
                Factory = idSvrFactory,
                SigningCertificate = LoadCertificate(),
                AuthenticationOptions = new AuthenticationOptions
                {
                    EnableSignOutPrompt = false,
                    EnablePostSignOutAutoRedirect = true,
                    PostSignOutAutoRedirectDelay = 1
                }
            };

            app.UseIdentityServer(options);
        }

        X509Certificate2 LoadCertificate()
        {
            return new X509Certificate2(@"C:\Development\FederatedIdentityServer\idsrv3test.pfx", "idsrv3test");
        }
    }
}
