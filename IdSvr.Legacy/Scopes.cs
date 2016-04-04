using System.Collections.Generic;
using IdentityServer3.Core.Models;

namespace IdSvr.Legacy
{
    public static class Scopes
    {
        public static List<Scope> Get()
        {
            return new List<Scope>(StandardScopes.All)
                {
                    new Scope
                    {
                        Name = "api"
                    },
                    new Scope
                    {
                        Name = "portal",
                        Type = ScopeType.Identity,

                        Claims = new List<ScopeClaim>
                        {
                            new ScopeClaim("age"),
                            new ScopeClaim("role"),
                            new ScopeClaim("name")
                        },
                        
                    }
                };
        }
    }
}