using ASPNETCore.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASPNETCore.Application.DTO
{
    public class REObjectDTO
    {
        public int Id { get; set; }

        public int Rooms { get; set; }

        public int Floors { get; set; }

        public int Square { get; set; }

        public int TypeId { get; set; }

        public int DealTypeId { get; set; }

        public string Street { get; set; }

        public int Building { get; set; }

        public int? Roomnum { get; set; }

        public int Price { get; set; }
        public int StatusId { get; set; }
        public virtual DealType DealType { get; set; }

        public virtual Status Status { get; set; }

        public virtual ObjectType ObjectType { get; set; }
        public virtual ICollection<ObjectImagesDTO> ObjectImages { get; set; } = new List<ObjectImagesDTO>();
    }
}
