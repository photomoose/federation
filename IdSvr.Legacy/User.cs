using Microsoft.AspNet.Identity.EntityFramework;

namespace IdSvr.Legacy
{
    public class User : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}