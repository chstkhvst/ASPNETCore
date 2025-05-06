using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASPNETCore.Application.DTO
{
    public class CreateREObjectDTO
    {
        public int Id { get; set; }

        [Required]
        public int Rooms { get; set; }

        [Required]
        public int Floors { get; set; }

        public int Square { get; set; }
        
        [Required]
        public int TypeId { get; set; }

        [Required]
        public int DealTypeId { get; set; }

        [Required]
        [StringLength(100)] 
        public string Street { get; set; }
        
        [Required]
        public int Building { get; set; }
        
        public int? Roomnum { get; set; }

        [Required]
        public int Price { get; set; }

        [Required]
        public int StatusId { get; set; }
        public IFormFileCollection? Files { get; set; }
        public virtual ICollection<ObjectImagesDTO> ObjectImages { get; set; }
    }
}
