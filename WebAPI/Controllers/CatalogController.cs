using ASPNETCore.Application.Services;
using ASPNETCore.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CatalogController : ControllerBase
    {
        private readonly CatalogService _catalogService;

        public CatalogController(CatalogService catalogService)
        {
            _catalogService = catalogService;
        }

        // GET api/catalog/dealtypes
        [HttpGet("dealtypes")]
        public async Task<ActionResult<IEnumerable<DealType>>> GetDealTypes()
        {
            var dealTypes = await _catalogService.GetAllDealTypesAsync();
            return Ok(dealTypes);
        }

        // GET api/catalog/dealtypes/{id}
        [HttpGet("dealtypes/{id}")]
        public async Task<ActionResult<DealType>> GetDealType(int id)
        {
            var dealType = await _catalogService.GetDealTypeByIdAsync(id);
            if (dealType == null) return NotFound();
            return Ok(dealType);
        }

        // GET api/catalog/objecttypes
        [HttpGet("objecttypes")]
        public async Task<ActionResult<IEnumerable<ObjectType>>> GetObjectTypes()
        {
            var objectTypes = await _catalogService.GetAllObjectTypesAsync();
            return Ok(objectTypes);
        }

        // GET api/catalog/objecttypes/{id}
        [HttpGet("objecttypes/{id}")]
        public async Task<ActionResult<ObjectType>> GetObjectType(int id)
        {
            var objectType = await _catalogService.GetObjectTypeByIdAsync(id);
            if (objectType == null) return NotFound();
            return Ok(objectType);
        }

        // GET api/catalog/statuses
        [HttpGet("statuses")]
        public async Task<ActionResult<IEnumerable<Status>>> GetStatuses()
        {
            var statuses = await _catalogService.GetAllStatusesAsync();
            return Ok(statuses);
        }

        // GET api/catalog/statuses/{id}
        [HttpGet("statuses/{id}")]
        public async Task<ActionResult<Status>> GetStatus(int id)
        {
            var status = await _catalogService.GetStatusByIdAsync(id);
            if (status == null) return NotFound();
            return Ok(status);
        }

        // GET api/catalog/resstatuses
        [HttpGet("resstatuses")]
        public async Task<ActionResult<IEnumerable<ResStatus>>> GetResStatuses()
        {
            var resStatuses = await _catalogService.GetAllResStatusesAsync();
            return Ok(resStatuses);
        }

        // GET api/catalog/resstatuses/{id}
        [HttpGet("resstatuses/{id}")]
        public async Task<ActionResult<ResStatus>> GetResStatus(int id)
        {
            var resStatus = await _catalogService.GetResStatusByIdAsync(id);
            if (resStatus == null) return NotFound();
            return Ok(resStatus);
        }
    }
}
