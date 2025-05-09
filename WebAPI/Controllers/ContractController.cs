using ASPNETCore.Application.DTO;
using ASPNETCore.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    /// <summary>
    /// Контроллер для работы с договорами
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class ContractController : ControllerBase
    {
        private readonly ContractServices _contractService;
        private readonly ILogger<ContractController> _logger;

        /// <summary>
        /// Инициализирует новый экземпляр контроллера договоров
        /// </summary>
        /// <param name="contractService">Сервис для работы с договорами</param>
        public ContractController(ContractServices contractService, ILogger<ContractController> logger)
        {
            _logger = logger;
            _contractService = contractService;
        }

        /// <summary>
        /// Получает все договоры или выполняет поиск по дате подписания
        /// </summary>
        /// <param name="signDate">Дата подписания для поиска (опционально)</param>
        /// <returns>Список договоров</returns>
        /// <response code="200">Успешное выполнение запроса</response>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ContractDTO>>> GetContracts([FromQuery] DateTime? signDate)
        {
            var currUser = User.Identity.IsAuthenticated ? User.Identity.Name : "Неавторизованный пользователь";
            if (signDate == null)
                _logger.LogInformation($"{currUser} получает список договоров");
            else
                _logger.LogInformation($"{currUser} получает список договоров по дате заключения {signDate}");
            var contracts = signDate == null
                ? await _contractService.GetAllAsync()
                : await _contractService.SearchBySignDateAsync(signDate.Value);

            return Ok(contracts);
        }

        /// <summary>
        /// Получает договор по идентификатору
        /// </summary>
        /// <param name="id">Идентификатор договора</param>
        /// <returns>Данные договора</returns>
        /// <response code="200">Договор найден</response>
        /// <response code="404">Договор не найден</response>
        [HttpGet("{id}")]
        public async Task<ActionResult<ContractDTO>> GetContract(int id)
        {
            var currUser = User.Identity.IsAuthenticated ? User.Identity.Name : "Неавторизованный пользователь";
            _logger.LogInformation($"{currUser} получает договор с id {id}");
            var contract = await _contractService.GetByIdAsync(id);
            if (contract == null) return NotFound();
            return Ok(contract);
        }

        /// <summary>
        /// Создает новый договор
        /// </summary>
        /// <param name="contractDto">Данные для создания договора</param>
        /// <returns>Созданный договор</returns>
        /// <response code="201">Договор успешно создан</response>
        [HttpPost]
        public async Task<ActionResult<ContractDTO>> CreateContract(CreateContractDTO contractDto)
        {
            var currUser = User.Identity.IsAuthenticated ? User.Identity.Name : "Неавторизованный пользователь";
            _logger.LogInformation($"{currUser} создает договор");
            await _contractService.AddAsync(contractDto);
            return CreatedAtAction(nameof(GetContract), new { id = contractDto.Id }, contractDto);
        }

        /// <summary>
        /// Обновляет существующий договор
        /// </summary>
        /// <param name="id">Идентификатор договора</param>
        /// <param name="contractDto">Обновленные данные договора</param>
        /// <returns>Обновленный договор</returns>
        /// <response code="200">Договор успешно обновлен</response>
        /// <response code="400">Некорректные входные данные</response>
        [HttpPut("{id}")]
        public async Task<ActionResult<ContractDTO>> UpdateContract(int id, CreateContractDTO contractDto)
        {
            var currUser = User.Identity.IsAuthenticated ? User.Identity.Name : "Неавторизованный пользователь";
            _logger.LogInformation($"{currUser} обновляет договор с id {id}");
            if (!ModelState.IsValid) return BadRequest(ModelState);
            if (id != contractDto.Id) return BadRequest();
            await _contractService.UpdateAsync(contractDto);
            return CreatedAtAction(nameof(GetContract), new { id = contractDto.Id }, contractDto);
        }

        /// <summary>
        /// Удаляет договор
        /// </summary>
        /// <param name="id">Идентификатор договора</param>
        /// <returns>Результат операции</returns>
        /// <response code="204">Договор успешно удален</response>
        /// <response code="409">Ошибка при удалении договора</response>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteContract(int id)
        {
            var currUser = User.Identity.IsAuthenticated ? User.Identity.Name : "Неавторизованный пользователь";
            _logger.LogInformation($"{currUser} удаляет договор с id {id}");
            try
            {
                await _contractService.DeleteAsync(id);
                return NoContent();
            }
            catch (ApplicationException ex)
            {
                _logger.LogError($"При удалении договора возникла ошибка {ex.Message}");
                return Conflict(new { message = ex.Message });
            }
        }
    }
}