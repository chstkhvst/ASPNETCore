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
    /// <summary>
    /// Репозиторий для работы с договорами в системе.
    /// </summary>
    public class ContractRepository : IContractRepository
    {
        private readonly ApplicationDbContext _context;

        /// <summary>
        /// Инициализирует новый экземпляр <see cref="ContractRepository"/>.
        /// </summary>
        /// <param name="context">Контекст базы данных.</param>
        public ContractRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Получает договор по указанному идентификатору.
        /// </summary>
        /// <param name="id">Идентификатор договора.</param>
        /// <returns>Найденный договор со связанными данными или null, если не найден.</returns>
        public async Task<Contract> GetByIdAsync(int id)
        {
            return await _context.Contracts
                .Include(c => c.Reservation)
                    .ThenInclude(r => r.Object)
                        .ThenInclude(o => o.DealType)
                .Include(c => c.Reservation)
                    .ThenInclude(r => r.Object)
                        .ThenInclude(o => o.ObjectType)
                .Include(c => c.Reservation)
                    .ThenInclude(r => r.Object)
                        .ThenInclude(o => o.Status)
                .Include(c => c.Reservation)
                    .ThenInclude(r => r.ResStatus)
                .Include(c => c.Reservation)
                    .ThenInclude(r => r.User)
                .Include(c => c.User)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        /// <summary>
        /// Получает все договоры в системе.
        /// </summary>
        /// <returns>Коллекция всех договоров, отсортированная по идентификатору в порядке убывания.</returns>
        public async Task<IEnumerable<Contract>> GetAllAsync()
        {
            return await _context.Contracts
                .Include(c => c.Reservation)
                    .ThenInclude(r => r.Object)
                        .ThenInclude(o => o.DealType)
                .Include(c => c.Reservation)
                    .ThenInclude(r => r.Object)
                        .ThenInclude(o => o.ObjectType)
                .Include(c => c.Reservation)
                    .ThenInclude(r => r.Object)
                        .ThenInclude(o => o.Status)
                .Include(c => c.Reservation)
                    .ThenInclude(r => r.ResStatus)
                .Include(c => c.Reservation)
                    .ThenInclude(r => r.User)
                .Include(c => c.User)
                .OrderByDescending(c => c.Id)
                .AsNoTracking()
                .ToListAsync();
        }

        /// <summary>
        /// Выполняет поиск договоров по дате подписания.
        /// </summary>
        /// <param name="signDate">Дата подписания для поиска.</param>
        /// <returns>Коллекция договоров, подписанных в указанную дату.</returns>
        public async Task<IEnumerable<Contract>> SearchBySignDateAsync(DateTime signDate)
        {
            return await _context.Contracts
                .Where(c => c.SignDate == signDate)
                .Include(c => c.Reservation)
                .Include(c => c.User)
                .ToListAsync();
        }

        /// <summary>
        /// Добавляет новый договор в систему.
        /// </summary>
        /// <param name="contract">Добавляемый договор.</param>
        /// <returns>Асинхронная задача.</returns>
        public async Task AddAsync(Contract contract)
        {
            var reservation = await _context.Reservations
                .Include(r => r.Object)
                .FirstOrDefaultAsync(r => r.Id == contract.ReservationId);
            _context.Contracts.Add(contract);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Обновляет существующий договор.
        /// </summary>
        /// <param name="contract">Договор с обновленными данными.</param>
        /// <exception cref="KeyNotFoundException">Выбрасывается, если договор не найден.</exception>
        /// <returns>Асинхронная задача.</returns>
        public async Task UpdateAsync(Contract contract)
        {
            var existingContract = await _context.Contracts.FindAsync(contract.Id);
            if (existingContract == null)
                throw new KeyNotFoundException($"Contract с ID {contract.Id} не найден.");

            _context.Entry(existingContract).CurrentValues.SetValues(contract);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Удаляет договор по указанному идентификатору.
        /// </summary>
        /// <param name="id">Идентификатор удаляемого договора.</param>
        /// <returns>Асинхронная задача.</returns>
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