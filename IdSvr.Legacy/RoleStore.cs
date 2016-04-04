using Microsoft.AspNet.Identity.EntityFramework;

namespace IdSvr.Legacy
{
    public class RoleStore : RoleStore<Role>
    {
        public RoleStore(Context ctx)
            : base(ctx)
        {
        }
    }
}