using ASPNETCore.Domain.Entities;
using ASPNETCore.Domain.Interfaces;
using ASPNETCore.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ASPNETCore.Infrastructure.Repositories
{
    public class ReservationRepository : IReservationRepository
    {
        private readonly ApplicationDbContext _context;

        public ReservationRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        // Получить бронь по ID
        public async Task<Reservation> GetByIdAsync(int id)
        {
            return await _context.Reservations
                .Include(r => r.Object)
                    .ThenInclude(o => o.ObjectType)
                .Include(r => r.Object)
                    .ThenInclude(o => o.DealType)
                .Include(r => r.Object)
                    .ThenInclude(o => o.Status)
                .Include(r => r.ResStatus)        // Загружаем статус брони
                .Include(r => r.User)             // Загружаем пользователя
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        // Получить все брони
        public async Task<IEnumerable<Reservation>> GetAllAsync()
        {
            return await _context.Reservations
                                .Include(r => r.Object)
                    .ThenInclude(o => o.ObjectType)
                .Include(r => r.Object)
                    .ThenInclude(o => o.DealType)
                .Include(r => r.Object)
                    .ThenInclude(o => o.Status)
                .Include(r => r.ResStatus)        // Загружаем статус брони
                .Include(r => r.User)             // Загружаем пользователя
                .OrderByDescending(r => r.Id)
                .AsNoTracking()
                .ToListAsync();
        }

        // Добавить новую бронь
        public async Task AddAsync(Reservation reservation)
        {
            //if (reservation.Object.StatusId != 1)
            //    throw new ArgumentException("Объект невозможно забронировать");
            _context.Reservations.Add(reservation);
            int changes = await _context.SaveChangesAsync();
            Console.WriteLine($"Добавлено {changes} брони в БД.");
        }

        // Обновить существующую бронь
        public async Task UpdateAsync(Reservation reservation)
        {
            var existingReservation = await _context.Reservations.FindAsync(reservation.Id);
            if (existingReservation == null)
                throw new KeyNotFoundException($"Бронь с ID {reservation.Id} не найдена.");

            _context.Entry(existingReservation).CurrentValues.SetValues(reservation);
            await _context.SaveChangesAsync();
        }

        // Удалить бронь
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
