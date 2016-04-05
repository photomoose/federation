using System.Collections.Generic;
using IdentityServer3.Core.Models;

namespace IdSvr.New
{
    internal static class Scopes
    {
        public static List<Scope> Get()
        {
            return new List<Scope>(StandardScopes.All)
            {
                new Scope
                {
                    Name = "portal",
                    DisplayName = "Some description about the Portal scope.",
                    Type = ScopeType.Identity,
                    Claims = new List<ScopeClaim>
                    {
                        new ScopeClaim("age"),
                        new ScopeClaim("role"),
                        new ScopeClaim("preferred_username")
                    }
                }
            };
        }
    }
}