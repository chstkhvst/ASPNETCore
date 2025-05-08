using ASPNETCore.Domain.Entities;
using ASPNETCore.Domain;

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

        Task<IEnumerable<REObject>> GetFilteredAsync(
            int? typeId,
            int? dealTypeId,
            int? statusId);
        /// <summary>
        /// Получает все объекты недвижимости с пагинацией.
        /// </summary>
        /// <param name="page">Номер страницы (начиная с 1)</param>
        /// <param name="pageSize">Количество элементов на странице</param>
        Task<PaginatedResponse<REObject>> GetAllPaginatedAsync(int page, int pageSize);
        /// <summary>
        /// Получает отфильтрованные объекты недвижимости с пагинацией.
        /// </summary>
        /// <param name="typeId">Фильтр по типу объекта</param>
        /// <param name="dealTypeId">Фильтр по типу сделки</param>
        /// <param name="statusId">Фильтр по статусу</param>
        /// <param name="page">Номер страницы (начиная с 1)</param>
        /// <param name="pageSize">Количество элементов на странице</param>
        Task<PaginatedResponse<REObject>> GetFilteredPaginatedAsync(
            int? typeId,
            int? dealTypeId,
            int? statusId,
            int page,
            int pageSize);
    }
}
