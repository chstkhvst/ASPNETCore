using ASPNETCore.Domain.Entities;
using ASPNETCore.Domain.Interfaces;
using ASPNETCore.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace ASPNETCore.Infrastructure.Repositories
{
    /// <summary>
    /// Репозиторий для работы с объектами недвижимости в базе данных.
    /// </summary>
    public class REObjectRepository : IREObjectRepository
    {
        private readonly ApplicationDbContext _context;

        /// <summary>
        /// Инициализирует новый экземпляр <see cref="REObjectRepository"/>.
        /// </summary>
        /// <param name="context">Контекст базы данных.</param>
        public REObjectRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Получает объект недвижимости по указанному идентификатору.
        /// </summary>
        /// <param name="id">Идентификатор объекта недвижимости.</param>
        /// <returns>Объект недвижимости или null, если объект не найден.</returns>
        public async Task<REObject> GetByIdAsync(int id)
        {
            return await _context.Objects
                .Include(o => o.ObjectType)
                .Include(o => o.Status)
                .Include(o => o.DealType)
                .Include(o => o.ObjectImages)
                .AsNoTracking()
                .FirstOrDefaultAsync(o => o.Id == id);
        }

        /// <summary>
        /// Получает все объекты недвижимости из базы данных.
        /// </summary>
        /// <returns>Коллекция всех объектов недвижимости.</returns>
        public async Task<IEnumerable<REObject>> GetAllAsync()
        {
            return await _context.Objects
                .Include(o => o.ObjectType)
                .Include(o => o.Status)
                .Include(o => o.DealType)
                .Include(o => o.ObjectImages)
                .AsNoTracking()
                .ToListAsync();
        }

        /// <summary>
        /// Выполняет поиск объектов недвижимости по названию улицы.
        /// </summary>
        /// <param name="street">Название улицы для поиска.</param>
        /// <returns>Коллекция найденных объектов недвижимости.</returns>
        public async Task<IEnumerable<REObject>> SearchByNameAsync(string street)
        {
            return await _context.Objects
                .Where(o => o.Street.Contains(street))
                .ToListAsync();
        }

        /// <summary>
        /// Добавляет новый объект недвижимости в базу данных.
        /// </summary>
        /// <param name="reObject">Добавляемый объект недвижимости.</param>
        /// <returns>Асинхронная задача.</returns>
        public async Task AddAsync(REObject reObject)
        {
            _context.Objects.Add(reObject);
            int changes = await _context.SaveChangesAsync();
            Console.WriteLine($"Добавлено {changes} объектов в БД.");
        }

        /// <summary>
        /// Обновляет существующий объект недвижимости в базе данных.
        /// </summary>
        /// <param name="reObject">Объект недвижимости с обновленными данными.</param>
        /// <exception cref="KeyNotFoundException">Выбрасывается, если объект не найден.</exception>
        /// <returns>Асинхронная задача.</returns>
        public async Task UpdateAsync(REObject reObject)
        {
            var existingObject = await _context.Objects
                .Include(o => o.ObjectImages)
                .FirstOrDefaultAsync(o => o.Id == reObject.Id);

            if (existingObject == null)
                throw new KeyNotFoundException($"REObject с ID {reObject.Id} не найден.");

            _context.Entry(existingObject).CurrentValues.SetValues(reObject);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Удаляет объект недвижимости по указанному идентификатору.
        /// </summary>
        /// <param name="id">Идентификатор удаляемого объекта.</param>
        /// <returns>Асинхронная задача.</returns>
        public async Task DeleteAsync(int id)
        {
            var obj = await _context.Objects
                .Include(o => o.ObjectImages)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (obj != null)
            {
                _context.ObjectImages.RemoveRange(obj.ObjectImages);
                _context.Objects.Remove(obj);
                await _context.SaveChangesAsync();
            }
        }

        /// <summary>
        /// Получает объекты недвижимости с применением фильтров.
        /// </summary>
        /// <param name="typeId">Идентификатор типа объекта (опционально).</param>
        /// <param name="dealTypeId">Идентификатор типа сделки (опционально).</param>
        /// <param name="statusId">Идентификатор статуса объекта (опционально).</param>
        /// <returns>Коллекция отфильтрованных объектов недвижимости.</returns>
        public async Task<IEnumerable<REObject>> GetFilteredAsync(
            int? typeId,
            int? dealTypeId,
            int? statusId)
        {
            var query = _context.Objects
                .Include(o => o.ObjectType)
                .Include(o => o.Status)
                .Include(o => o.DealType)
                .Include(o => o.ObjectImages)
                .AsNoTracking()
                .AsQueryable();

            if (typeId.HasValue)
            {
                query = query.Where(o => o.TypeId == typeId.Value);
            }

            if (dealTypeId.HasValue)
            {
                query = query.Where(o => o.DealTypeId == dealTypeId.Value);
            }

            if (statusId.HasValue)
            {
                query = query.Where(o => o.StatusId == statusId.Value);
            }

            return await query.ToListAsync();
        }
    }
}