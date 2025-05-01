using ASPNETCore.Application.DTO;
using ASPNETCore.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ContractController : ControllerBase
    {
        private readonly ContractServices _contractService;

        public ContractController(ContractServices contractService)
        {
            _contractService = contractService;
        }

        // Получение всех контрактов или поиск по дате подписания
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ContractDTO>>> GetContracts([FromQuery] DateTime? signDate)
        {
            var contracts = signDate == null
                ? await _contractService.GetAllAsync()
                : await _contractService.SearchBySignDateAsync(signDate.Value);

            return Ok(contracts);
        }

        // Получение контракта по ID
        [HttpGet("{id}")]
        public async Task<ActionResult<ContractDTO>> GetContract(int id)
        {
            var contract = await _contractService.GetByIdAsync(id);
            if (contract == null) return NotFound();
            return Ok(contract);
        }

        //// Создание нового контракта
        [HttpPost]
        public async Task<ActionResult<ContractDTO>> CreateContract(CreateContractDTO contractDto)
        {
            await _contractService.AddAsync(contractDto);
            return CreatedAtAction(nameof(GetContract), new { id = contractDto.Id }, contractDto);
        }

        // Обновление контракта по ID
        [HttpPut("{id}")]
        public async Task<ActionResult<ContractDTO>> UpdateContract(int id, CreateContractDTO contractDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            if (id != contractDto.Id) return BadRequest();
            await _contractService.UpdateAsync(contractDto);
            return CreatedAtAction(nameof(GetContract), new { id = contractDto.Id }, contractDto);
        }

        // Удаление контракта по ID
        //[Authorize(Roles = "admin")]
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