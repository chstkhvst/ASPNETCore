using ASPNETCore.Application.DTO;
using ASPNETCore.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ASPNETCore.Domain;
using ASPNETCore.Domain.Entities;

namespace WebAPI.Controllers
{
    /// <summary>
    /// Контроллер для работы с объектами недвижимости
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class REObjectController : ControllerBase
    {
        private readonly REObjectServices _reObjectService;
        private readonly IWebHostEnvironment _env;
        private readonly ILogger<REObjectController> _logger;

        /// <summary>
        /// Инициализирует новый экземпляр контроллера
        /// </summary>
        /// <param name="reObjectService">Сервис для работы с объектами недвижимости</param>
        /// <param name="env">Окружение веб-хоста</param>
        public REObjectController(REObjectServices reObjectService, IWebHostEnvironment env, ILogger<REObjectController> logger)
        {
            _reObjectService = reObjectService;
            _env = env;
            _logger = logger;
        }

        /// <summary>
        /// Получает список объектов недвижимости
        /// </summary>
        /// <param name="name">Фильтр по названию улицы (опционально)</param>
        /// <returns>Список объектов недвижимости</returns>
        /// <response code="200">Успешное выполнение запроса</response>
        /// <response code="401">Требуется авторизация</response>

        [HttpGet]
        [Authorize]
        public async Task<ActionResult<PaginatedResponse<REObjectDTO>>> GetREObjects(
            [FromQuery] int page = 1,          // Текущая страница (по умолчанию 1)
            [FromQuery] int pageSize = 5)
        {
            var currUser = User.Identity.IsAuthenticated ? User.Identity.Name : "Неавторизованный пользователь";
            _logger.LogInformation($"{currUser} получает список всех объектов. Страница {page}");
            var isAdmin = User.IsInRole("admin");
            var paginatedResult = await _reObjectService.GetAllPaginatedAsync(isAdmin, page, pageSize);
            return Ok(paginatedResult);
        }

        /// <summary>
        /// Получает объект недвижимости по идентификатору
        /// </summary>
        /// <param name="id">Идентификатор объекта</param>
        /// <returns>Данные объекта недвижимости</returns>
        /// <response code="200">Объект найден</response>
        /// <response code="404">Объект не найден</response>
        [HttpGet("{id}")]
        public async Task<ActionResult<REObjectDTO>> GetREObject(int id)
        {
            var currUser = User.Identity.IsAuthenticated ? User.Identity.Name : "Неавторизованный пользователь";
            _logger.LogInformation($"{currUser} получает объект с ID {id}");
            var obj = await _reObjectService.GetByIdAsync(id);
            if (obj == null) return NotFound();
            return Ok(obj);
        }

        /// <summary>
        /// Создает новый объект недвижимости
        /// </summary>
        /// <param name="street">Улица</param>
        /// <param name="building">Номер дома</param>
        /// <param name="roomnum">Номер комнаты (опционально)</param>
        /// <param name="rooms">Количество комнат</param>
        /// <param name="floors">Этажность</param>
        /// <param name="square">Площадь</param>
        /// <param name="price">Цена</param>
        /// <param name="dealTypeId">Идентификатор типа сделки</param>
        /// <param name="typeId">Идентификатор типа объекта</param>
        /// <param name="statusId">Идентификатор статуса</param>
        /// <param name="files">Файлы изображений</param>
        /// <returns>Созданный объект</returns>
        /// <response code="201">Объект успешно создан</response>
        /// <response code="400">Ошибка в данных запроса</response>
        [HttpPost]
        public async Task<ActionResult<REObjectDTO>> CreateREObject(
            [FromForm] string street,
            [FromForm] int building,
            [FromForm] int? roomnum,
            [FromForm] int rooms,
            [FromForm] int floors,
            [FromForm] int square,
            [FromForm] int price,
            [FromForm] int dealTypeId,
            [FromForm] int typeId,
            [FromForm] int statusId,
            [FromForm] IFormFileCollection files)
        {
            var currUser = User.Identity.IsAuthenticated ? User.Identity.Name : "Неавторизованный пользователь";
            _logger.LogInformation($"{currUser} создает новый объект");
            try
            {
                var reObjectDto = new CreateREObjectDTO
                {
                    Street = street,
                    Building = building,
                    Roomnum = roomnum,
                    Rooms = rooms,
                    Floors = floors,
                    Square = square,
                    Price = price,
                    DealTypeId = dealTypeId,
                    TypeId = typeId,
                    StatusId = statusId
                };

                await _reObjectService.AddAsync(reObjectDto, files, _env);
                return CreatedAtAction(nameof(GetREObject), new { id = reObjectDto.Id }, reObjectDto);
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    Message = ex.Message,
                    Details = ex.Message,
                    StackTrace = ex.StackTrace
                });
            }
        }

        /// <summary>
        /// Обновляет существующий объект недвижимости
        /// </summary>
        /// <param name="id">Идентификатор объекта</param>
        /// <param name="street">Улица</param>
        /// <param name="building">Номер дома</param>
        /// <param name="roomnum">Номер комнаты (опционально)</param>
        /// <param name="rooms">Количество комнат</param>
        /// <param name="floors">Этажность</param>
        /// <param name="square">Площадь</param>
        /// <param name="price">Цена</param>
        /// <param name="dealTypeId">Идентификатор типа сделки</param>
        /// <param name="typeId">Идентификатор типа объекта</param>
        /// <param name="statusId">Идентификатор статуса</param>
        /// <param name="files">Новые файлы изображений (опционально)</param>
        /// <param name="imagesToDelete">Список идентификаторов изображений для удаления (опционально)</param>
        /// <returns>Обновленный объект</returns>
        /// <response code="200">Объект успешно обновлен</response>
        /// <response code="400">Ошибка в данных запроса</response>
        [HttpPut("{id}")]
        public async Task<ActionResult<REObjectDTO>> UpdateObject(
            int id,
            [FromForm] string street,
            [FromForm] int building,
            [FromForm] int? roomnum,
            [FromForm] int rooms,
            [FromForm] int floors,
            [FromForm] int square,
            [FromForm] int price,
            [FromForm] int dealTypeId,
            [FromForm] int typeId,
            [FromForm] int statusId,
            [FromForm] IFormFileCollection? files,
            [FromForm] List<int>? imagesToDelete = null)
        {
            var currUser = User.Identity.IsAuthenticated ? User.Identity.Name : "Неавторизованный пользователь";
            _logger.LogInformation($"{currUser} обновляет объект с ID {id}");
            try
            {
                var reobjectDto = new CreateREObjectDTO
                {
                    Id = id,
                    Street = street,
                    Building = building,
                    Roomnum = roomnum,
                    Rooms = rooms,
                    Floors = floors,
                    Square = square,
                    Price = price,
                    DealTypeId = dealTypeId,
                    TypeId = typeId,
                    StatusId = statusId
                };

                await _reObjectService.UpdateAsync(reobjectDto, files, _env, imagesToDelete);
                return CreatedAtAction(nameof(GetREObject), new { id = reobjectDto.Id }, reobjectDto);
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    Message = "Ошибка при обновлении объекта",
                    Details = ex.Message,
                    StackTrace = ex.StackTrace
                });
            }
        }

        /// <summary>
        /// Удаляет объект недвижимости
        /// </summary>
        /// <param name="id">Идентификатор объекта</param>
        /// <returns>Результат операции</returns>
        /// <response code="204">Объект успешно удален</response>
        /// <response code="409">Ошибка при удалении объекта</response>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteREObject(int id)
        {
            var currUser = User.Identity.IsAuthenticated ? User.Identity.Name : "Неавторизованный пользователь";
            _logger.LogInformation($"{currUser} удаляет объект с ID {id}");
            try
            {
                await _reObjectService.DeleteAsync(id);
                return NoContent();
            }
            catch (ApplicationException ex)
            {
                _logger.LogError($"Ошибка при удалении объекта {ex.Message}");
                return Conflict(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Получает отфильтрованный список объектов недвижимости
        /// </summary>
        /// <param name="typeId">Идентификатор типа объекта (опционально)</param>
        /// <param name="dealTypeId">Идентификатор типа сделки (опционально)</param>
        /// <param name="statusId">Идентификатор статуса (опционально)</param>
        /// <returns>Отфильтрованный список объектов</returns>
        /// <response code="200">Успешное выполнение запроса</response>
        /// <response code="401">Требуется авторизация</response>
        [HttpGet("filter")]
        [Authorize]
        public async Task<ActionResult<PaginatedResponse<REObjectDTO>>> GetFilteredObjects(
            [FromQuery] int? typeId,
            [FromQuery] int? dealTypeId,
            [FromQuery] int? statusId,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 5)
        {
            var currUser = User.Identity.IsAuthenticated ? User.Identity.Name : "Неавторизованный пользователь";
            _logger.LogInformation($"{currUser} получает список отфильтрованных объектов. Страница {page}");
            var isAdmin = User.IsInRole("admin");
            var filteredObjects = await _reObjectService.GetFilteredPaginatedAsync(
                typeId,
                dealTypeId,
                statusId,
                isAdmin,
                page,
                pageSize);

            return Ok(filteredObjects);
        }
    }
}