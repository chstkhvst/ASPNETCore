using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ASPNETCore.Domain.Entities
{
    public class Contract
    {
        [Key]
        public int Id { get; set; }

        [Column(TypeName = "date")]
        public DateTime SignDate { get; set; }

        public int ReservationId { get; set; }

        public string UserId { get; set; }

        public int Total { get; set; }

        public virtual Reservation Reservation { get; set; }

        public virtual User User { get; set; }
    }
}
