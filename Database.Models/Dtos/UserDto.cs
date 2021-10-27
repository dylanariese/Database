using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Database.Models.Dtos
{
    public class UserDto
    {
        public string Id { get; set; }

        public string Username { get; set; }

        public string Email { get; set; }

        [Required]
        public string FirstName { get; set; }

        public string Prefix { get; set; }

        [Required]
        public string LastName { get; set; }

        public string Token { get; set; }

        public DateTime? ValidTo { get; set; }

        public List<RoleDto> Roles { get; set; } = new List<RoleDto>();
    }
}