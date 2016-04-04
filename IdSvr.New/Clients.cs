using System.Collections.Generic;
using IdentityServer3.Core.Models;

namespace IdSvr.New
{
    internal static class Clients
    {
        public static List<Client> Get()
        {
            return new List<Client>
            {
                new Client
                {
                    Enabled = true,
                    ClientName = "New Portal",
                    ClientId = "new-portal",
                    Flow = Flows.Implicit,
                    RedirectUris = new List<string>
                    {
                        "https://portal.new/"
                    },
                    PostLogoutRedirectUris = new List<string>
                    {
                        "https://portal.new/"
                    },
                    AllowAccessToAllScopes = true,
                    RequireConsent = false
                }
            };
        }
    }
}