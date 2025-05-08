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
    /// Репозиторий для работы с изображениями объектов недвижимости
    /// </summary>
    public class ImageRepository : IImageRepository
    {
        private readonly ApplicationDbContext _context;

        /// <summary>
        /// Инициализирует новый экземпляр <see cref="ImageRepository"/>
        /// </summary>
        /// <param name="context">Контекст базы данных</param>
        public ImageRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Получает все изображения для указанного объекта недвижимости
        /// </summary>
        /// <param name="objectId">Идентификатор объекта недвижимости</param>
        /// <returns>Коллекция изображений объекта</returns>
        public async Task<IEnumerable<ObjectImages>> GetByObjectIdAsync(int objectId)
        {
            return await _context.ObjectImages
                .Where(img => img.ObjectId == objectId)
                .AsNoTracking()
                .ToListAsync();
        }

        /// <summary>
        /// Добавляет новое изображение для объекта недвижимости
        /// </summary>
        /// <param name="image">Добавляемое изображение</param>
        /// <returns>Асинхронная задача</returns>
        public async Task AddAsync(ObjectImages image)
        {
            _context.ObjectImages.Add(image);
            int changes = await _context.SaveChangesAsync();
            Console.WriteLine($"Добавлено {changes} изображений в БД.");
        }

        /// <summary>
        /// Удаляет изображение по указанному идентификатору
        /// </summary>
        /// <param name="id">Идентификатор удаляемого изображения</param>
        /// <returns>Асинхронная задача</returns>
        public async Task DeleteAsync(int id)
        {
            var image = await _context.ObjectImages.FindAsync(id);
            if (image != null)
            {
                _context.ObjectImages.Remove(image);
                await _context.SaveChangesAsync();
            }
        }
    }
}