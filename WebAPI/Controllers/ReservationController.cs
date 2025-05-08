using ASPNETCore.Application.DTO;
using ASPNETCore.Application.Services;
using ASPNETCore.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace WebAPI.Controllers
{
    /// <summary>
    /// Контроллер для работы с бронированиями объектов недвижимости
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class ReservationController : ControllerBase
    {
        private readonly ReservationServices _reservationService;

        /// <summary>
        /// Инициализирует новый экземпляр <see cref="ReservationController"/>
        /// </summary>
        /// <param name="reservationService">Сервис для работы с бронированиями</param>
        public ReservationController(ReservationServices reservationService)
        {
            _reservationService = reservationService;
        }

        /// <summary>
        /// Получает список всех бронирований
        /// </summary>
        /// <returns>Коллекция бронирований</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ReservationDTO>>> GetReservations()
        {
            var reservations = await _reservationService.GetAllAsync();
            return Ok(reservations);
        }

        /// <summary>
        /// Получает бронирование по идентификатору
        /// </summary>
        /// <param name="id">Идентификатор бронирования</param>
        /// <returns>Данные бронирования</returns>
        /// <response code="200">Бронирование найдено</response>
        /// <response code="404">Бронирование не найдено</response>
        [HttpGet("{id}")]
        public async Task<ActionResult<ReservationDTO>> GetReservation(int id)
        {
            var reservation = await _reservationService.GetByIdAsync(id);
            if (reservation == null) return NotFound();
            return Ok(reservation);
        }

        /// <summary>
        /// Создает новое бронирование
        /// </summary>
        /// <param name="reservationDto">Данные для создания бронирования</param>
        /// <returns>Созданное бронирование</returns>
        /// <response code="201">Бронирование успешно создано</response>
        [HttpPost]
        public async Task<ActionResult<ReservationDTO>> CreateReservation(CreateReservationDTO reservationDto)
        {
            await _reservationService.AddAsync(reservationDto);
            return CreatedAtAction(nameof(GetReservation), new { id = reservationDto.Id }, reservationDto);
        }

        /// <summary>
        /// Обновляет существующее бронирование
        /// </summary>
        /// <param name="id">Идентификатор бронирования</param>
        /// <param name="reservationDto">Обновленные данные бронирования</param>
        /// <returns>Обновленное бронирование</returns>
        /// <response code="200">Бронирование успешно обновлено</response>
        /// <response code="400">Некорректные входные данные</response>
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

        /// <summary>
        /// Удаляет бронирование
        /// </summary>
        /// <param name="id">Идентификатор бронирования</param>
        /// <returns>Результат операции</returns>
        /// <response code="204">Бронирование успешно удалено</response>
        /// <response code="409">Ошибка при удалении бронирования</response>
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