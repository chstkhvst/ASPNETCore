using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

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

        //[StringLength(11)]
        //public string? Phone { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        [JsonIgnore]
        public virtual ICollection<Contract> Contract { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        [JsonIgnore]
        public virtual ICollection<Reservation> Reservation { get; set; }
    }
}
