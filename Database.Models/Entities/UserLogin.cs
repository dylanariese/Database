using Microsoft.AspNetCore.Identity;

namespace Database.Models.Entities
{
    public class UserLogin : IdentityUserLogin<string>
    {
        public virtual User User { get; set; }
    }
}