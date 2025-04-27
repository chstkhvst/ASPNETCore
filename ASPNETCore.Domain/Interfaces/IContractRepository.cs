using ASPNETCore.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ASPNETCore.Domain.Interfaces
{
    public interface IContractRepository
    {
        Task<Contract> GetByIdAsync(int id);
        Task<IEnumerable<Contract>> GetAllAsync();
        Task<IEnumerable<Contract>> SearchBySignDateAsync(DateTime signDate);
        Task AddAsync(Contract contract);
        Task UpdateAsync(Contract contract);
        Task DeleteAsync(int id);
    }
}