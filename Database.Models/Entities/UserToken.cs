using Microsoft.AspNetCore.Identity;

namespace Database.Models.Entities
{
    public class UserToken : IdentityUserToken<string>
    {
        public virtual User User { get; set; }
    }
}