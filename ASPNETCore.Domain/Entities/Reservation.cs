﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ASPNETCore.Domain.Entities
{
    public class Reservation
    {
        //public Reservation()
        //{
        //    Contract = new HashSet<Contract>();
        //}

        [Key]
        public int Id { get; set; }

        public int ObjectId { get; set; }

        public string UserId { get; set; }

        [Column(TypeName = "date")]
        public DateTime? StartDate { get; set; }

        [Column(TypeName = "date")]
        public DateTime? EndDate { get; set; }

        public int ResStatusId { get; set; }

        //[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        //public virtual ICollection<Contract> Contract { get; set; }

        public virtual REObject Object { get; set; }

        public virtual ResStatus ResStatus { get; set; }

        public virtual User User { get; set; }
    }
}

