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
    public class ImageRepository : IImageRepository
    {
        private readonly ApplicationDbContext _context;

        public ImageRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        // Получение всех изображений для конкретного объекта
        public async Task<IEnumerable<ObjectImages>> GetByObjectIdAsync(int objectId)
        {
            return await _context.ObjectImages
                .Where(img => img.ObjectId == objectId)
                .AsNoTracking()
                .ToListAsync();
        }

        // Добавление нового изображения
        public async Task AddAsync(ObjectImages image)
        {
            _context.ObjectImages.Add(image);
            int changes = await _context.SaveChangesAsync();
            Console.WriteLine($"Добавлено {changes} изображений в БД.");
        }

        // Удаление изображения по ID
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