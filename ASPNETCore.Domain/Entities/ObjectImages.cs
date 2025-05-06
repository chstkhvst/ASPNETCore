using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASPNETCore.Domain.Entities
{
    public class ObjectImages
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string ImagePath { get; set; }

        [Required]
        public int ObjectId { get; set; }
        public virtual REObject Object { get; set; }
    }
}
