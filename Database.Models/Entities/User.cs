using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace Database.Models.Entities
{
    public class User : IdentityUser
    {
        public string FirstName { get; set; }

        public string Prefix { get; set; }

        public string LastName { get; set; }

        public virtual ICollection<UserClaim> Claims { get; set; }

        public virtual ICollection<UserLogin> Logins { get; set; }

        public virtual ICollection<UserToken> Tokens { get; set; }

        public virtual ICollection<UserRole> UserRoles { get; set; }
    }
}