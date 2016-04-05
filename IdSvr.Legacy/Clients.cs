using System.Collections.Generic;
using IdentityServer3.Core.Models;

namespace IdSvr.Legacy
{
    public static class Clients
    {
        public static List<Client> Get()
        {
            return new List<Client>
            {
                new Client
                {
                    Enabled = true,
                    ClientName = "New Identity Server",
                    ClientId = "new-idsvr",
                    Flow = Flows.Implicit,
                    RedirectUris = new List<string>
                    {
                        "https://idsvr.new/core/"
                    },
                    PostLogoutRedirectUris = new List<string>
                    {
                        "https://idsvr.new/core/signoutcallback/"
                    },
                    AllowAccessToAllScopes = true,
                    RequireConsent = false
                },
                new Client
                {
                    Enabled = true,
                    ClientName = "Legacy Portal",
                    ClientId = "legacy-portal",
                    ClientSecrets = new List<Secret>
                    {
                        new Secret("secret".Sha256())
                    },
                    Flow = Flows.Hybrid,
                    RedirectUris = new List<string>
                    {
                        "https://portal.legacy/"
                    },
                    PostLogoutRedirectUris = new List<string>
                    {
                        "https://portal.legacy/"
                    },
                    AllowedScopes = new List<string>
                    {
                        "openid",
                        "email",
                        "portalprofile"
                    },
                    RequireConsent = false,                    
                }
            };
        }
    }
}