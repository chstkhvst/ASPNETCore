using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASPNETCore.Application.DTO
{
    public class CreateReservationDTO
    {
        public int Id { get; set; }

        public int ObjectId { get; set; }

        public string UserId { get; set; }

        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public int ResStatusId { get; set; }
    }
}
