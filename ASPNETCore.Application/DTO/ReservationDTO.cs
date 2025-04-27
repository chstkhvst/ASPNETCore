using ASPNETCore.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASPNETCore.Application.DTO
{
    public class ReservationDTO
    {
        public int Id { get; set; }

        public int ObjectId { get; set; }

        public string UserId { get; set; }

        [Column(TypeName = "date")]
        public DateTime? StartDate { get; set; }

        [Column(TypeName = "date")]
        public DateTime? EndDate { get; set; }

        public int ResStatusId { get; set; }

        
        //public virtual ICollection<Contract> Contract { get; set; }

        public virtual REObject Object { get; set; }

        public virtual ResStatus ResStatus { get; set; }

        public virtual User User { get; set; }
    }
}
