using ASPNETCore.Domain.Entities;
using ASPNETCore.Domain.Interfaces;
using ASPNETCore.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ASPNETCore.Infrastructure.Repositories
{
    public class CatalogRepository : ICatalogRepository
    {
        private readonly ApplicationDbContext _context;

        public CatalogRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        // Получить все DealType
        public async Task<IEnumerable<DealType>> GetAllDealTypesAsync()
        {
            return await _context.DealTypes
                .AsNoTracking()
                .ToListAsync();
        }

        // Получить DealType по ID
        public async Task<DealType> GetDealTypeByIdAsync(int id)
        {
            return await _context.DealTypes
                .AsNoTracking()
                .FirstOrDefaultAsync(d => d.Id == id);
        }

        // Получить все ObjectType
        public async Task<IEnumerable<ObjectType>> GetAllObjectTypesAsync()
        {
            return await _context.ObjectTypes
                .AsNoTracking()
                .ToListAsync();
        }

        // Получить ObjectType по ID
        public async Task<ObjectType> GetObjectTypeByIdAsync(int id)
        {
            return await _context.ObjectTypes
                .AsNoTracking()
                .FirstOrDefaultAsync(o => o.Id == id);
        }

        // Получить все Status
        public async Task<IEnumerable<Status>> GetAllStatusesAsync()
        {
            return await _context.Statuses
                .AsNoTracking()
                .ToListAsync();
        }

        // Получить Status по ID
        public async Task<Status> GetStatusByIdAsync(int id)
        {
            return await _context.Statuses
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        // Получить все ResStatus
        public async Task<IEnumerable<ResStatus>> GetAllResStatusesAsync()
        {
            return await _context.ResStatuses
                .AsNoTracking()
                .ToListAsync();
        }

        // Получить ResStatus по ID
        public async Task<ResStatus> GetResStatusByIdAsync(int id)
        {
            return await _context.ResStatuses
                .AsNoTracking()
                .FirstOrDefaultAsync(r => r.Id == id);
        }
    }
}
