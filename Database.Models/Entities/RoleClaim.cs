using Microsoft.AspNetCore.Identity;

namespace Database.Models.Entities
{
    public class RoleClaim : IdentityRoleClaim<string>
    {
        public virtual Role Role { get; set; }
    }
}