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
    public class REObjectRepository : IREObjectRepository
    {
        private readonly ApplicationDbContext _context;

        public REObjectRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        // Получить объект недвижимости по ID
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

        // Получить все объекты недвижимости
        //public async Task<IEnumerable<REObject>> GetAllAsync()
        //{
        //    return await _context.Objects
        //        .Include(o => o.ObjectType)
        //        .Include(o => o.Status)
        //        .ToListAsync();
        //}
        public async Task<IEnumerable<REObject>> GetAllAsync()
        {
            return await _context.Objects
                .Include(o => o.ObjectType) 
                .Include(o => o.Status)     
                .Include(o => o.DealType)
                .Include(o=>o.ObjectImages)
                .AsNoTracking() 
                .ToListAsync();
        }


        // Поиск объектов по улице
        public async Task<IEnumerable<REObject>> SearchByNameAsync(string street)
        {
            return await _context.Objects
                .Where(o => o.Street.Contains(street))
                //.Include(o => o.ObjectType)
                //.Include(o => o.Status)
                .ToListAsync();
        }

        // Добавить новый объект недвижимости
        public async Task AddAsync(REObject reObject)
        {
            _context.Objects.Add(reObject);
            int changes = await _context.SaveChangesAsync();
            Console.WriteLine($"Добавлено {changes} объектов в БД.");
        }

        // Обновить объект недвижимости
        //public async Task UpdateAsync(REObject reObject)
        //{
        //    var existingObject = await _context.Objects.FindAsync(reObject.Id);
        //    if (existingObject == null)
        //        throw new KeyNotFoundException($"REObject с ID {reObject.Id} не найден.");

        //    _context.Entry(existingObject).CurrentValues.SetValues(reObject);
        //    await _context.SaveChangesAsync();
        //}
        public async Task UpdateAsync(REObject reObject)
        {
            var existingObject = await _context.Objects
                .Include(o => o.ObjectImages) //  включаем связанные изображения
                .FirstOrDefaultAsync(o => o.Id == reObject.Id);

            if (existingObject == null)
                throw new KeyNotFoundException($"REObject с ID {reObject.Id} не найден.");

            // Обновляем скалярные свойства
            _context.Entry(existingObject).CurrentValues.SetValues(reObject);
            await _context.SaveChangesAsync();
        }
        // Удалить объект недвижимости
        public async Task DeleteAsync(int id)
        {
            var obj = await _context.Objects
                .Include(o => o.ObjectImages) // Включаем связанные изображения
                .FirstOrDefaultAsync(o => o.Id == id);

            if (obj != null)
            {
                // Удаляем все связанные изображения
                _context.ObjectImages.RemoveRange(obj.ObjectImages);

                // Удаляем сам объект
                _context.Objects.Remove(obj);

                await _context.SaveChangesAsync();
            }
        }
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

            // Применяем фильтры, если они указаны
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
