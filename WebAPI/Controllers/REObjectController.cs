using ASPNETCore.Application.DTO;
using ASPNETCore.Application.Services;
using ASPNETCore.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class REObjectController : ControllerBase
    {
        private readonly REObjectServices _reObjectService;

        public REObjectController(REObjectServices reObjectService)
        {
            _reObjectService = reObjectService;
        }

        //получение всех объектов или поиск по названию
        [HttpGet]
        public async Task<ActionResult<IEnumerable<REObjectDTO>>> GetREObjects([FromQuery] string? name)
        {
            var objects = string.IsNullOrEmpty(name) //если строка не пустая, то ищем по имени, иначе выводим весь список объектов
                ? await _reObjectService.GetAllAsync()
                : await _reObjectService.SearchByNameAsync(name);

            return Ok(objects);
        }

        //получение объекта по ID
        [HttpGet("{id}")]
        public async Task<ActionResult<REObjectDTO>> GetREObject(int id)
        {
            var obj = await _reObjectService.GetByIdAsync(id);
            if (obj == null) return NotFound();
            return Ok(obj);
        }

        //создание нового объекта
        [HttpPost]
        public async Task<ActionResult<REObjectDTO>> CreateREObject(CreateREObjectDTO reObjectDto)
        {
            await _reObjectService.AddAsync(reObjectDto);
            return CreatedAtAction(nameof(GetREObject), new { id = reObjectDto.Id }, reObjectDto);
        }

        ////обновление объекта по ID
        //[HttpPut("{id}")]
        //public async Task<IActionResult> UpdateREObject(int id, CreateREObjectDTO reObjectDto)
        //{
        //    if (id != reObjectDto.Id) return BadRequest();
        //    await _reObjectService.UpdateAsync(reObjectDto);
        //    return NoContent();
        //}
        [HttpPut("{id}")]
        // Указывает, что данный метод обрабатывает HTTP PUT-запросы для обновления.
        public async Task<ActionResult<REObjectDTO>> UpdateProject(int id, CreateREObjectDTO reobjectDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            if (id != reobjectDto.Id) return BadRequest();
            await _reObjectService.UpdateAsync(reobjectDto);
            return CreatedAtAction(nameof(GetREObject), new { id = reobjectDto.Id }, reobjectDto);
        }

        //удаление объекта по ID
        //[Authorize(Roles = "admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteREObject(int id)
        {
            try
            {
                await _reObjectService.DeleteAsync(id);
                return NoContent();
                // Возвращает HTTP-ответ 204 (No Content) после успешного удаления.
            }
            catch (ApplicationException ex)
            {
                return Conflict(new { message = ex.Message });
                // В случае ошибки возвращает HTTP-ответ 409 (Conflict) с описанием ошибки.
            }
        }

    }
}
