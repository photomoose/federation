using IdSvr.Legacy.App_Packages.IdentityServer3.AspNetIdentity;

namespace IdSvr.Legacy
{
    public class UserService : AspNetIdentityUserService<User, string>
    {
        public UserService(UserManager userMgr)
            : base(userMgr)
        {
        }
    }
}