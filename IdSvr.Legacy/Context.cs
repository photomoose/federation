using Microsoft.AspNet.Identity.EntityFramework;

namespace IdSvr.Legacy
{
    public class Context : IdentityDbContext<User, Role, string, IdentityUserLogin, IdentityUserRole, IdentityUserClaim>
    {
        public Context(string connString)
            : base(connString)
        {
        }
    }
}