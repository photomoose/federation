using Microsoft.AspNet.Identity;

namespace IdSvr.Legacy
{
    public class RoleManager : RoleManager<Role>
    {
        public RoleManager(RoleStore store)
            : base(store)
        {
        }
    }
}