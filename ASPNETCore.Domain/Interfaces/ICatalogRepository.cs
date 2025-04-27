using ASPNETCore.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASPNETCore.Domain.Interfaces
{
    public interface ICatalogRepository
    {
        Task<IEnumerable<DealType>> GetAllDealTypesAsync();
        Task<DealType> GetDealTypeByIdAsync(int id);

        Task<IEnumerable<ObjectType>> GetAllObjectTypesAsync();
        Task<ObjectType> GetObjectTypeByIdAsync(int id);

        Task<IEnumerable<Status>> GetAllStatusesAsync();
        Task<Status> GetStatusByIdAsync(int id);

        Task<IEnumerable<ResStatus>> GetAllResStatusesAsync();
        Task<ResStatus> GetResStatusByIdAsync(int id);
    }

}
