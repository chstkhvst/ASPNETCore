using ASPNETCore.Domain.Entities;
using ASPNETCore.Domain.Interfaces;

namespace ASPNETCore.Application.Services
{
    /// <summary>
    /// Сервис для работы со справочными данными системы
    /// </summary>
    public class CatalogService
    {
        private readonly ICatalogRepository _catalogRepository;

        /// <summary>
        /// Инициализирует новый экземпляр <see cref="CatalogService"/>
        /// </summary>
        /// <param name="catalogRepository">Репозиторий для работы со справочными данными</param>
        public CatalogService(ICatalogRepository catalogRepository)
        {
            _catalogRepository = catalogRepository;
        }

        #region DealType Methods

        /// <summary>
        /// Получает все типы сделок
        /// </summary>
        /// <returns>Коллекция всех типов сделок</returns>
        public Task<IEnumerable<DealType>> GetAllDealTypesAsync() =>
            _catalogRepository.GetAllDealTypesAsync();

        /// <summary>
        /// Получает тип сделки по идентификатору
        /// </summary>
        /// <param name="id">Идентификатор типа сделки</param>
        /// <returns>Тип сделки или null, если не найден</returns>
        public Task<DealType> GetDealTypeByIdAsync(int id) =>
            _catalogRepository.GetDealTypeByIdAsync(id);

        #endregion

        #region ObjectType Methods

        /// <summary>
        /// Получает все типы объектов недвижимости
        /// </summary>
        /// <returns>Коллекция всех типов объектов</returns>
        public Task<IEnumerable<ObjectType>> GetAllObjectTypesAsync() =>
            _catalogRepository.GetAllObjectTypesAsync();

        /// <summary>
        /// Получает тип объекта недвижимости по идентификатору
        /// </summary>
        /// <param name="id">Идентификатор типа объекта</param>
        /// <returns>Тип объекта или null, если не найден</returns>
        public Task<ObjectType> GetObjectTypeByIdAsync(int id) =>
            _catalogRepository.GetObjectTypeByIdAsync(id);

        #endregion

        #region Status Methods

        /// <summary>
        /// Получает все статусы объектов недвижимости
        /// </summary>
        /// <returns>Коллекция всех статусов объектов</returns>
        public Task<IEnumerable<Status>> GetAllStatusesAsync() =>
            _catalogRepository.GetAllStatusesAsync();

        /// <summary>
        /// Получает статус объекта недвижимости по идентификатору
        /// </summary>
        /// <param name="id">Идентификатор статуса</param>
        /// <returns>Статус объекта или null, если не найден</returns>
        public Task<Status> GetStatusByIdAsync(int id) =>
            _catalogRepository.GetStatusByIdAsync(id);

        #endregion

        #region ResStatus Methods

        /// <summary>
        /// Получает все статусы бронирований
        /// </summary>
        /// <returns>Коллекция всех статусов бронирований</returns>
        public Task<IEnumerable<ResStatus>> GetAllResStatusesAsync() =>
            _catalogRepository.GetAllResStatusesAsync();

        /// <summary>
        /// Получает статус бронирования по идентификатору
        /// </summary>
        /// <param name="id">Идентификатор статуса бронирования</param>
        /// <returns>Статус бронирования или null, если не найден</returns>
        public Task<ResStatus> GetResStatusByIdAsync(int id) =>
            _catalogRepository.GetResStatusByIdAsync(id);

        #endregion
    }
}