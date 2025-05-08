using ASPNETCore.Domain.Entities;
using ASPNETCore.Domain.Interfaces;
using ASPNETCore.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ASPNETCore.Infrastructure.Repositories
{
    /// <summary>
    /// Репозиторий для работы с бронированиями объектов недвижимости
    /// </summary>
    public class ReservationRepository : IReservationRepository
    {
        private readonly ApplicationDbContext _context;

        /// <summary>
        /// Инициализирует новый экземпляр <see cref="ReservationRepository"/>
        /// </summary>
        /// <param name="context">Контекст базы данных</param>
        public ReservationRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Получает бронирование по указанному идентификатору
        /// </summary>
        /// <param name="id">Идентификатор бронирования</param>
        /// <returns>Бронирование с полной информацией или null, если не найдено</returns>
        public async Task<Reservation> GetByIdAsync(int id)
        {
            return await _context.Reservations
                .Include(r => r.Object)
                    .ThenInclude(o => o.ObjectType)
                .Include(r => r.Object)
                    .ThenInclude(o => o.DealType)
                .Include(r => r.Object)
                    .ThenInclude(o => o.Status)
                .Include(r => r.ResStatus)
                .Include(r => r.User)
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        /// <summary>
        /// Получает все бронирования, отсортированные по ID в порядке убывания
        /// </summary>
        /// <returns>Коллекция всех бронирований с полной информацией</returns>
        public async Task<IEnumerable<Reservation>> GetAllAsync()
        {
            return await _context.Reservations
                .Include(r => r.Object)
                    .ThenInclude(o => o.ObjectType)
                .Include(r => r.Object)
                    .ThenInclude(o => o.DealType)
                .Include(r => r.Object)
                    .ThenInclude(o => o.Status)
                .Include(r => r.ResStatus)
                .Include(r => r.User)
                .OrderByDescending(r => r.Id)
                .AsNoTracking()
                .ToListAsync();
        }

        /// <summary>
        /// Добавляет новое бронирование
        /// </summary>
        /// <param name="reservation">Добавляемое бронирование</param>
        /// <returns>Асинхронная задача</returns>
        public async Task AddAsync(Reservation reservation)
        {
            _context.Reservations.Add(reservation);
            int changes = await _context.SaveChangesAsync();
            Console.WriteLine($"Добавлено {changes} брони в БД.");
        }

        /// <summary>
        /// Обновляет существующее бронирование
        /// </summary>
        /// <param name="reservation">Данные для обновления</param>
        /// <exception cref="KeyNotFoundException">Выбрасывается, если бронирование не найдено</exception>
        /// <returns>Асинхронная задача</returns>
        public async Task UpdateAsync(Reservation reservation)
        {
            var existingReservation = await _context.Reservations.FindAsync(reservation.Id);
            if (existingReservation == null)
                throw new KeyNotFoundException($"Бронь с ID {reservation.Id} не найдена.");

            _context.Entry(existingReservation).CurrentValues.SetValues(reservation);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Удаляет бронирование по указанному идентификатору
        /// </summary>
        /// <param name="id">Идентификатор бронирования</param>
        /// <returns>Асинхронная задача</returns>
        public async Task DeleteAsync(int id)
        {
            var reservation = await _context.Reservations.FindAsync(id);
            if (reservation != null)
            {
                _context.Reservations.Remove(reservation);
                await _context.SaveChangesAsync();
            }
        }
    }
}