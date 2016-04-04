using Microsoft.AspNet.Identity;

namespace IdSvr.Legacy
{
    public class UserManager : UserManager<User, string>
    {
        public UserManager(UserStore store)
            : base(store)
        {
            this.ClaimsIdentityFactory = new ClaimsFactory();
        }
    }
}