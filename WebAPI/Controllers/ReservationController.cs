using ASPNETCore.Application.DTO;
using ASPNETCore.Application.Services;
using ASPNETCore.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReservationController : ControllerBase
    {
        private readonly ReservationServices _reservationService;

        public ReservationController(ReservationServices reservationService)
        {
            _reservationService = reservationService;
        }

        // Получение всех броней
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ReservationDTO>>> GetReservations()
        {
            var reservations = await _reservationService.GetAllAsync();
            return Ok(reservations);
        }

        // Получение брони по ID
        [HttpGet("{id}")]
        public async Task<ActionResult<ReservationDTO>> GetReservation(int id)
        {
            var reservation = await _reservationService.GetByIdAsync(id);
            if (reservation == null) return NotFound();
            return Ok(reservation);
        }

        // Создание новой брони
        [HttpPost]
        public async Task<ActionResult<ReservationDTO>> CreateReservation(CreateReservationDTO reservationDto)
        {
            await _reservationService.AddAsync(reservationDto);
            return CreatedAtAction(nameof(GetReservation), new { id = reservationDto.Id }, reservationDto);
        }

        // Обновление брони по ID
        [HttpPut("{id}")]
        public async Task<ActionResult<ReservationDTO>> UpdateReservation(int id, CreateReservationDTO reservationDto)
        {
            Console.WriteLine("Зашли в функцию");
            if (!ModelState.IsValid) return BadRequest(ModelState);
            Console.WriteLine("Зашли в проверку");
            if (id != reservationDto.Id) return BadRequest();
            await _reservationService.UpdateAsync(reservationDto);
            return CreatedAtAction(nameof(GetReservation), new { id = reservationDto.Id }, reservationDto);
        }

        // Удаление брони по ID
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteReservation(int id)
        {
            try
            {
                await _reservationService.DeleteAsync(id);
                return NoContent();
            }
            catch (ApplicationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
        }
    }
}
