using Microsoft.AspNetCore.Identity;

namespace Database.Models.Entities
{
    public class UserClaim : IdentityUserClaim<string>
    {
        public virtual User User { get; set; }
    }
}