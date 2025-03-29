using ASPNETCore.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASPNETCore.Domain.Interfaces
{
    public interface IREObjectRepository
    {
        /// <summary>
        /// Получает все объекты недвижимости.
        /// </summary>
        Task<IEnumerable<REObject>> GetAllAsync();

        /// <summary>
        /// Получает объект недвижимости по его ID.
        /// </summary>
        Task<REObject> GetByIdAsync(int id);

        /// <summary>
        /// Поиск объектов недвижимости по названию.
        /// </summary>
        Task<IEnumerable<REObject>> SearchByNameAsync(string name);

        /// <summary>
        /// Добавляет новый объект недвижимости.
        /// </summary>
        Task AddAsync(REObject reObject);

        /// <summary>
        /// Обновляет существующий объект недвижимости.
        /// </summary>
        Task UpdateAsync(REObject reObject);

        /// <summary>
        /// Удаляет объект недвижимости по ID.
        /// </summary>
        Task DeleteAsync(int id);
    }
}
