using ASPNETCore.Domain.Entities;
using ASPNETCore.Domain.Interfaces;
using ASPNETCore.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ASPNETCore.Infrastructure.Repositories
{
    public class ContractRepository : IContractRepository
    {
        private readonly ApplicationDbContext _context;

        public ContractRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Contract> GetByIdAsync(int id)
        {
            return await _context.Contracts
                .Include(c => c.Reservation)
                    .ThenInclude(r => r.Object)
                        .ThenInclude(o => o.DealType)   // Загружаем DealType
                .Include(c => c.Reservation)
                    .ThenInclude(r => r.Object)
                        .ThenInclude(o => o.ObjectType) // Загружаем ObjectType
                .Include(c => c.Reservation)
                    .ThenInclude(r => r.Object)
                        .ThenInclude(o => o.Status)    // Загружаем Status
                .Include(c => c.Reservation)
                    .ThenInclude(r => r.ResStatus)
                .Include(c => c.Reservation)
                    .ThenInclude(r => r.User)
                .Include(c => c.User)
                .FirstOrDefaultAsync(c => c.Id == id);
        }



        public async Task<IEnumerable<Contract>> GetAllAsync()
        {
            return await _context.Contracts
                .Include(c => c.Reservation)
                    .ThenInclude(r => r.Object)
                        .ThenInclude(o => o.DealType)   // Загружаем DealType
                .Include(c => c.Reservation)
                    .ThenInclude(r => r.Object)
                        .ThenInclude(o => o.ObjectType) // Загружаем ObjectType
                .Include(c => c.Reservation)
                    .ThenInclude(r => r.Object)
                        .ThenInclude(o => o.Status)    // Загружаем Status
                .Include(c => c.Reservation)
                    .ThenInclude(r => r.ResStatus)
                .Include(c => c.Reservation)
                    .ThenInclude(r => r.User)
                .Include(c => c.User)
                .AsNoTracking()
                .ToListAsync();
        }


        // Поиск контрактов по дате подписания
        public async Task<IEnumerable<Contract>> SearchBySignDateAsync(DateTime signDate)
        {
            return await _context.Contracts
                .Where(c => c.SignDate == signDate)
                .Include(c => c.Reservation)
                .Include(c => c.User)
                .ToListAsync();
        }

        // Добавить новый контракт
        //public async Task AddAsync(Contract contract)
        //{
        //    _context.Contracts.Add(contract);
        //    int changes = await _context.SaveChangesAsync();
        //    Console.WriteLine($"Добавлено {changes} контрактов в БД.");
        //}
        public async Task AddAsync(Contract contract)
        {
            var reservation = await _context.Reservations
                .Include(r => r.Object)
                .FirstOrDefaultAsync(r => r.Id == contract.ReservationId);

            if (reservation == null)
                throw new ArgumentException($"Reservation с ID {contract.ReservationId} не найдена.");
            if (reservation.ResStatusId != 1)
                throw new ArgumentException($"Договор по брони уже заключен");
            contract.SignDate = DateTime.UtcNow;
            contract.Total = reservation.Object.Price + 20000;
            reservation.ResStatusId = 2;

            _context.Contracts.Add(contract);
            await _context.SaveChangesAsync();
        }


        // Обновить контракт
        public async Task UpdateAsync(Contract contract)
        {
            var existingContract = await _context.Contracts.FindAsync(contract.Id);
            if (existingContract == null)
                throw new KeyNotFoundException($"Contract с ID {contract.Id} не найден.");

            _context.Entry(existingContract).CurrentValues.SetValues(contract);
            await _context.SaveChangesAsync();
        }

        // Удалить контракт
        public async Task DeleteAsync(int id)
        {
            var contract = await _context.Contracts.FindAsync(id);
            if (contract != null)
            {
                _context.Contracts.Remove(contract);
                await _context.SaveChangesAsync();
            }
        }
    }
}