using IdentityManager.AspNetIdentity;

namespace IdSvr.Legacy
{
    public class SimpleIdentityManagerService : AspNetIdentityManagerService<User, string, Role, string>
    {
        public SimpleIdentityManagerService(UserManager userMgr, RoleManager roleMgr)
            : base(userMgr, roleMgr)
        {
        }
    }
}