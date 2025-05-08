using ASPNETCore.Application.Services;
using ASPNETCore.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace WebAPI.Controllers
{
    /// <summary>
    /// Контроллер для работы со справочными данными системы
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class CatalogController : ControllerBase
    {
        private readonly CatalogService _catalogService;

        /// <summary>
        /// Инициализирует новый экземпляр контроллера каталогов
        /// </summary>
        /// <param name="catalogService">Сервис для работы со справочными данными</param>
        public CatalogController(CatalogService catalogService)
        {
            _catalogService = catalogService;
        }

        /// <summary>
        /// Получает все типы сделок
        /// </summary>
        /// <returns>Коллекция типов сделок</returns>
        /// <response code="200">Успешное выполнение запроса</response>
        [HttpGet("dealtypes")]
        public async Task<ActionResult<IEnumerable<DealType>>> GetDealTypes()
        {
            var dealTypes = await _catalogService.GetAllDealTypesAsync();
            return Ok(dealTypes);
        }

        /// <summary>
        /// Получает тип сделки по идентификатору
        /// </summary>
        /// <param name="id">Идентификатор типа сделки</param>
        /// <returns>Данные типа сделки</returns>
        /// <response code="200">Тип сделки найден</response>
        /// <response code="404">Тип сделки не найден</response>
        [HttpGet("dealtypes/{id}")]
        public async Task<ActionResult<DealType>> GetDealType(int id)
        {
            var dealType = await _catalogService.GetDealTypeByIdAsync(id);
            if (dealType == null) return NotFound();
            return Ok(dealType);
        }

        /// <summary>
        /// Получает все типы объектов недвижимости
        /// </summary>
        /// <returns>Коллекция типов объектов</returns>
        /// <response code="200">Успешное выполнение запроса</response>
        [HttpGet("objecttypes")]
        public async Task<ActionResult<IEnumerable<ObjectType>>> GetObjectTypes()
        {
            var objectTypes = await _catalogService.GetAllObjectTypesAsync();
            return Ok(objectTypes);
        }

        /// <summary>
        /// Получает тип объекта недвижимости по идентификатору
        /// </summary>
        /// <param name="id">Идентификатор типа объекта</param>
        /// <returns>Данные типа объекта</returns>
        /// <response code="200">Тип объекта найден</response>
        /// <response code="404">Тип объекта не найден</response>
        [HttpGet("objecttypes/{id}")]
        public async Task<ActionResult<ObjectType>> GetObjectType(int id)
        {
            var objectType = await _catalogService.GetObjectTypeByIdAsync(id);
            if (objectType == null) return NotFound();
            return Ok(objectType);
        }

        /// <summary>
        /// Получает все статусы объектов недвижимости
        /// </summary>
        /// <returns>Коллекция статусов объектов</returns>
        /// <response code="200">Успешное выполнение запроса</response>
        [HttpGet("statuses")]
        public async Task<ActionResult<IEnumerable<Status>>> GetStatuses()
        {
            var statuses = await _catalogService.GetAllStatusesAsync();
            return Ok(statuses);
        }

        /// <summary>
        /// Получает статус объекта недвижимости по идентификатору
        /// </summary>
        /// <param name="id">Идентификатор статуса</param>
        /// <returns>Данные статуса объекта</returns>
        /// <response code="200">Статус найден</response>
        /// <response code="404">Статус не найден</response>
        [HttpGet("statuses/{id}")]
        public async Task<ActionResult<Status>> GetStatus(int id)
        {
            var status = await _catalogService.GetStatusByIdAsync(id);
            if (status == null) return NotFound();
            return Ok(status);
        }

        /// <summary>
        /// Получает все статусы бронирований
        /// </summary>
        /// <returns>Коллекция статусов бронирований</returns>
        /// <response code="200">Успешное выполнение запроса</response>
        [HttpGet("resstatuses")]
        public async Task<ActionResult<IEnumerable<ResStatus>>> GetResStatuses()
        {
            var resStatuses = await _catalogService.GetAllResStatusesAsync();
            return Ok(resStatuses);
        }

        /// <summary>
        /// Получает статус бронирования по идентификатору
        /// </summary>
        /// <param name="id">Идентификатор статуса бронирования</param>
        /// <returns>Данные статуса бронирования</returns>
        /// <response code="200">Статус найден</response>
        /// <response code="404">Статус не найден</response>
        [HttpGet("resstatuses/{id}")]
        public async Task<ActionResult<ResStatus>> GetResStatus(int id)
        {
            var resStatus = await _catalogService.GetResStatusByIdAsync(id);
            if (resStatus == null) return NotFound();
            return Ok(resStatus);
        }
    }
}