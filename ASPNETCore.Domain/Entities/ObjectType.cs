using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ASPNETCore.Domain.Entities
{
    public class ObjectType
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string TypeName { get; set; }

        [JsonIgnore]
        public virtual ICollection<REObject> Object { get; set; }
    }
    //public partial class ObjectType
    //{
    //    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
    //    public ObjectType()
    //    {
    //        Object = new HashSet<REObject>();
    //    }

    //    public int Id { get; set; }

    //    [Required]
    //    [StringLength(50)]
    //    public string TypeName { get; set; }

    //    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
    //    public virtual ICollection<REObject> Object { get; set; }
    //}
}
