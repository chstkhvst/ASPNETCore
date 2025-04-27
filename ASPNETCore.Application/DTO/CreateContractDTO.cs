using ASPNETCore.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASPNETCore.Application.DTO
{
    public class CreateContractDTO
    {
        public int Id { get; set; }

        [Column(TypeName = "date")]
        public DateTime SignDate { get; set; }

        public int ReservationId { get; set; }

        public string UserId { get; set; }

        public int Total { get; set; }

    }
}
