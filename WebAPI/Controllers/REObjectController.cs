using ASPNETCore.Application.DTO;
using ASPNETCore.Application.Services;
using ASPNETCore.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class REObjectController : ControllerBase
    {
        private readonly REObjectServices _reObjectService;
        private readonly IWebHostEnvironment _env;
        public REObjectController(REObjectServices reObjectService, IWebHostEnvironment env)
        {
            _reObjectService = reObjectService;
            _env = env;
        }
        [HttpGet]
        [Authorize]
        public async Task<ActionResult<IEnumerable<REObjectDTO>>> GetREObjects([FromQuery] string? name)
        {
            var isAdmin = User.IsInRole("admin");
            var objects = string.IsNullOrEmpty(name)
                ? await _reObjectService.GetAllAsync(isAdmin)
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
        //[HttpPost]
        //public async Task<ActionResult<REObjectDTO>> CreateREObject(CreateREObjectDTO reObjectDto)
        //{
        //    await _reObjectService.AddAsync(reObjectDto);
        //    return CreatedAtAction(nameof(GetREObject), new { id = reObjectDto.Id }, reObjectDto);
        //}
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

        //[HttpPut("{id}")]
        //public async Task<ActionResult<REObjectDTO>> UpdateProject(int id, CreateREObjectDTO reobjectDto)
        //{
        //    if (!ModelState.IsValid) return BadRequest(ModelState);
        //    if (id != reobjectDto.Id) return BadRequest();
        //    await _reObjectService.UpdateAsync(reobjectDto);
        //    return CreatedAtAction(nameof(GetREObject), new { id = reobjectDto.Id }, reobjectDto);
        //}
        [HttpPut("{id}")]
        public async Task<ActionResult<REObjectDTO>> UpdateProject(
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
    [FromForm] IFormFileCollection? files, [FromForm] List<int>? imagesToDelete = null)
        {
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
