using ASPNETCore.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ASPNETCore.Domain.Interfaces
{
    public interface IReservationRepository
    {
        Task<IEnumerable<Reservation>> GetAllAsync();
        Task<Reservation> GetByIdAsync(int id);
        Task AddAsync(Reservation reservation);
        Task UpdateAsync(Reservation reservation);
        Task DeleteAsync(int id);
        Task<IEnumerable<Reservation>> SearchByPhoneNumberAsync(string phoneNumber);
    }
}
