using System.Collections.Generic;
using System.Threading.Tasks;
using ASPNETCore.Domain.Entities;
using ASPNETCore.Domain.Interfaces;

namespace ASPNETCore.Application.Services
{
    public class CatalogService
    {
        private readonly ICatalogRepository _catalogRepository;

        public CatalogService(ICatalogRepository catalogRepository)
        {
            _catalogRepository = catalogRepository;
        }

        // DealType
        public Task<IEnumerable<DealType>> GetAllDealTypesAsync() =>
            _catalogRepository.GetAllDealTypesAsync();

        public Task<DealType> GetDealTypeByIdAsync(int id) =>
            _catalogRepository.GetDealTypeByIdAsync(id);

        // ObjectType
        public Task<IEnumerable<ObjectType>> GetAllObjectTypesAsync() =>
            _catalogRepository.GetAllObjectTypesAsync();

        public Task<ObjectType> GetObjectTypeByIdAsync(int id) =>
            _catalogRepository.GetObjectTypeByIdAsync(id);

        // Status
        public Task<IEnumerable<Status>> GetAllStatusesAsync() =>
            _catalogRepository.GetAllStatusesAsync();

        public Task<Status> GetStatusByIdAsync(int id) =>
            _catalogRepository.GetStatusByIdAsync(id);

        // ResStatus
        public Task<IEnumerable<ResStatus>> GetAllResStatusesAsync() =>
            _catalogRepository.GetAllResStatusesAsync();

        public Task<ResStatus> GetResStatusByIdAsync(int id) =>
            _catalogRepository.GetResStatusByIdAsync(id);
    }
}
