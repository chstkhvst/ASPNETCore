using ASPNETCore.Domain.Entities;
using ASPNETCore.Domain.Interfaces;
using ASPNETCore.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ASPNETCore.Infrastructure.Repositories
{
    /// <summary>
    /// Репозиторий для работы со справочными данными системы (каталогами).
    /// </summary>
    public class CatalogRepository : ICatalogRepository
    {
        private readonly ApplicationDbContext _context;

        /// <summary>
        /// Инициализирует новый экземпляр <see cref="CatalogRepository"/>.
        /// </summary>
        /// <param name="context">Контекст базы данных.</param>
        public CatalogRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Получает все типы сделок из каталога.
        /// </summary>
        /// <returns>Коллекция всех типов сделок.</returns>
        public async Task<IEnumerable<DealType>> GetAllDealTypesAsync()
        {
            return await _context.DealTypes
                .AsNoTracking()
                .ToListAsync();
        }

        /// <summary>
        /// Получает тип сделки по указанному идентификатору.
        /// </summary>
        /// <param name="id">Идентификатор типа сделки.</param>
        /// <returns>Тип сделки или null, если не найден.</returns>
        public async Task<DealType> GetDealTypeByIdAsync(int id)
        {
            return await _context.DealTypes
                .AsNoTracking()
                .FirstOrDefaultAsync(d => d.Id == id);
        }

        /// <summary>
        /// Получает все типы объектов недвижимости из каталога.
        /// </summary>
        /// <returns>Коллекция всех типов объектов недвижимости.</returns>
        public async Task<IEnumerable<ObjectType>> GetAllObjectTypesAsync()
        {
            return await _context.ObjectTypes
                .AsNoTracking()
                .ToListAsync();
        }

        /// <summary>
        /// Получает тип объекта недвижимости по указанному идентификатору.
        /// </summary>
        /// <param name="id">Идентификатор типа объекта.</param>
        /// <returns>Тип объекта недвижимости или null, если не найден.</returns>
        public async Task<ObjectType> GetObjectTypeByIdAsync(int id)
        {
            return await _context.ObjectTypes
                .AsNoTracking()
                .FirstOrDefaultAsync(o => o.Id == id);
        }

        /// <summary>
        /// Получает все статусы объектов недвижимости из каталога.
        /// </summary>
        /// <returns>Коллекция всех статусов объектов.</returns>
        public async Task<IEnumerable<Status>> GetAllStatusesAsync()
        {
            return await _context.Statuses
                .AsNoTracking()
                .ToListAsync();
        }

        /// <summary>
        /// Получает статус объекта недвижимости по указанному идентификатору.
        /// </summary>
        /// <param name="id">Идентификатор статуса.</param>
        /// <returns>Статус объекта или null, если не найден.</returns>
        public async Task<Status> GetStatusByIdAsync(int id)
        {
            return await _context.Statuses
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        /// <summary>
        /// Получает все статусы бронирований из каталога.
        /// </summary>
        /// <returns>Коллекция всех статусов бронирований.</returns>
        public async Task<IEnumerable<ResStatus>> GetAllResStatusesAsync()
        {
            return await _context.ResStatuses
                .AsNoTracking()
                .ToListAsync();
        }

        /// <summary>
        /// Получает статус бронирования по указанному идентификатору.
        /// </summary>
        /// <param name="id">Идентификатор статуса бронирования.</param>
        /// <returns>Статус бронирования или null, если не найден.</returns>
        public async Task<ResStatus> GetResStatusByIdAsync(int id)
        {
            return await _context.ResStatuses
                .AsNoTracking()
                .FirstOrDefaultAsync(r => r.Id == id);
        }
    }
}