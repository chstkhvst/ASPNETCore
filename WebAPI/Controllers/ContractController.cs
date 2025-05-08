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

        /// <summary>
        /// Инициализирует новый экземпляр контроллера договоров
        /// </summary>
        /// <param name="contractService">Сервис для работы с договорами</param>
        public ContractController(ContractServices contractService)
        {
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
            try
            {
                await _contractService.DeleteAsync(id);
                return NoContent();
            }
            catch (ApplicationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
        }
    }
}