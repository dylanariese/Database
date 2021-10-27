using System.ComponentModel.DataAnnotations;

namespace Database.Models.Models
{
    public class SignOutViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}