using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace ASPNETCore.Domain.Entities
{
    public class User : IdentityUser
    {

        [Required]
        [StringLength(50)]
        public string FullName { get; set; }

        public int RoleId { get; set; }

        [StringLength(10)]
        public string? Passport { get; set; }

        [StringLength(11)]
        public string? Phone { get; set; }
    }
}
