using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASPNETCore.Domain.Entities
{
    public class REObject
    {
        [Key]
        public int Id { get; set; }

        public int Rooms { get; set; } //количество комнат

        public int Floors { get; set; } //кол-во этажей

        public int Square { get; set; } //площадь

        public int TypeId { get; set; } //тип объекта

        public int DealTypeId { get; set; } //тип сделки

        [Required]
        [StringLength(50)]
        public string Street { get; set; } //улица, длина до 50 символов

        public int Building { get; set; } //номер здания

        public int? Roomnum { get; set; } //номер квартиры

        public int Price { get; set; } //стоимость

        public int StatusId { get; set; } // статус
        public string? ImagePath { get; set; }

        public virtual DealType DealType { get; set; }

        public virtual Status Status { get; set; }

        public virtual ObjectType ObjectType { get; set; }
        //public virtual ICollection<Reservation> Reservation { get; set; }
    }
}
