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
        private readonly ILogger<ReservationController> _logger;

        /// <summary>
        /// Инициализирует новый экземпляр <see cref="ReservationController"/>
        /// </summary>
        /// <param name="reservationService">Сервис для работы с бронированиями</param>
        public ReservationController(ReservationServices reservationService, ILogger<ReservationController> logger)
        {
            _reservationService = reservationService;
            _logger = logger;
        }

        /// <summary>
        /// Получает список всех бронирований
        /// </summary>
        /// <returns>Коллекция бронирований</returns>

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ReservationDTO>>> GetReservations([FromQuery] string? phoneNumber)
        {
            var currUser = User.Identity.IsAuthenticated ? User.Identity.Name : "Неавторизованный пользователь";
            if (string.IsNullOrEmpty(phoneNumber)) 
                _logger.LogInformation($"{currUser} получает список всех бронирований");
            else
                _logger.LogInformation($"{currUser} получает список бронирований с номером {phoneNumber}");
            var reservations = string.IsNullOrEmpty(phoneNumber)
                ? await _reservationService.GetAllAsync()
                : await _reservationService.SearchByPhoneNumberAsync(phoneNumber);

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
            var currUser = User.Identity.IsAuthenticated ? User.Identity.Name : "Неавторизованный пользователь";
            _logger.LogInformation($"{currUser} получает бронирование с id {id}");
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
            var currUser = User.Identity.IsAuthenticated ? User.Identity.Name : "Неавторизованный пользователь";
            _logger.LogInformation($"{currUser} создает новую бронь");
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
            var currUser = User.Identity.IsAuthenticated ? User.Identity.Name : "Неавторизованный пользователь";
            _logger.LogInformation($"{currUser} обновляет бронь с id {id}");
            if (!ModelState.IsValid) return BadRequest(ModelState);
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
            var currUser = User.Identity.IsAuthenticated ? User.Identity.Name : "Неавторизованный пользователь";
            _logger.LogInformation($"{currUser} удаляет бронь с id {id}");
            try
            {
                await _reservationService.DeleteAsync(id);
                return NoContent();
            }
            catch (ApplicationException ex)
            {
                _logger.LogError($"Ошибка при удалении брони {ex.Message}");
                
                return Conflict(new { message = ex.Message });
            }
        }
    }
}