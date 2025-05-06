using ASPNETCore.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASPNETCore.Domain.Interfaces
{
    public interface IImageRepository
    {
        Task<IEnumerable<ObjectImages>> GetByObjectIdAsync(int objectId);
        Task AddAsync(ObjectImages image);
        Task DeleteAsync(int id);
    }
}
