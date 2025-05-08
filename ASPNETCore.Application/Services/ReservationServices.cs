using ASPNETCore.Application.DTO;
using ASPNETCore.Domain.Entities;
using ASPNETCore.Domain.Interfaces;

namespace ASPNETCore.Application.Services
{
    /// <summary>
    /// Сервис для работы с бронированиями объектов недвижимости
    /// </summary>
    public class ReservationServices
    {
        private readonly IReservationRepository _reservationRepository;
        private readonly IREObjectRepository _reobjRepository;

        /// <summary>
        /// Инициализирует новый экземпляр <see cref="ReservationServices"/>
        /// </summary>
        /// <param name="reservationRepository">Репозиторий бронирований</param>
        /// <param name="reobjRepository">Репозиторий объектов недвижимости</param>
        public ReservationServices(IReservationRepository reservationRepository, IREObjectRepository reobjRepository)
        {
            _reservationRepository = reservationRepository;
            _reobjRepository = reobjRepository;
        }

        /// <summary>
        /// Получает все бронирования
        /// </summary>
        /// <returns>Коллекция DTO бронирований</returns>
        public async Task<IEnumerable<ReservationDTO>> GetAllAsync()
        {
            var reservations = await _reservationRepository.GetAllAsync();
            return reservations.Select(r => new ReservationDTO
            {
                Id = r.Id,
                ObjectId = r.ObjectId,
                UserId = r.UserId,
                StartDate = r.StartDate,
                EndDate = r.EndDate,
                ResStatusId = r.ResStatusId,
                Object = r.Object,
                ResStatus = r.ResStatus,
                User = r.User,
            });
        }

        /// <summary>
        /// Получает бронирование по идентификатору
        /// </summary>
        /// <param name="id">Идентификатор бронирования</param>
        /// <returns>DTO бронирования или null, если не найдено</returns>
        public async Task<ReservationDTO> GetByIdAsync(int id)
        {
            var r = await _reservationRepository.GetByIdAsync(id);
            if (r == null)
            {
                Console.WriteLine($"Бронь с ID {id} не найдена в БД!");
                return null;
            }

            return new ReservationDTO
            {
                Id = r.Id,
                ObjectId = r.ObjectId,
                UserId = r.UserId,
                StartDate = r.StartDate,
                EndDate = r.EndDate,
                ResStatusId = r.ResStatusId,
                Object = r.Object,
                ResStatus = r.ResStatus,
                User = r.User
            };
        }

        /// <summary>
        /// Добавляет новое бронирование
        /// </summary>
        /// <param name="r">DTO для создания бронирования</param>
        /// <exception cref="ArgumentException">
        /// Выбрасывается при:
        /// - Неправильной дате (EndDate меньше StartDate)
        /// - Невозможности забронировать объект
        /// </exception>
        /// <returns>Асинхронная задача</returns>
        public async Task AddAsync(CreateReservationDTO r)
        {
            if (r.EndDate < r.StartDate)
                throw new ArgumentException("Ошибка даты");
            var obj = await _reobjRepository.GetByIdAsync(r.ObjectId);
            if (obj?.StatusId != 1)
                throw new ArgumentException("Объект невозможно забронировать");
            var reservation = new Reservation
            {
                ObjectId = r.ObjectId,
                UserId = r.UserId,
                StartDate = r.StartDate,
                EndDate = r.EndDate,
                ResStatusId = r.ResStatusId
            };
            await _reservationRepository.AddAsync(reservation);
            obj.StatusId = 2;
            await _reobjRepository.UpdateAsync(obj);
        }

        /// <summary>
        /// Обновляет существующее бронирование
        /// </summary>
        /// <param name="r">DTO с обновленными данными бронирования</param>
        /// <remarks>
        /// При установке ResStatusId = 3 (отмена) возвращает объект в доступное состояние (StatusId = 1)
        /// </remarks>
        /// <returns>Асинхронная задача</returns>
        public async Task UpdateAsync(CreateReservationDTO r)
        {
            var reservation = await _reservationRepository.GetByIdAsync(r.Id);
            if (reservation != null)
            {
                reservation.ObjectId = r.ObjectId;
                reservation.UserId = r.UserId;
                reservation.StartDate = r.StartDate;
                reservation.EndDate = r.EndDate;
                reservation.ResStatusId = r.ResStatusId;

                await _reservationRepository.UpdateAsync(reservation);
            }
            if (r.ResStatusId == 3)
            {
                var obj = await _reobjRepository.GetByIdAsync(r.ObjectId);
                obj.StatusId = 1;
                await _reobjRepository.UpdateAsync(obj);
            }
        }

        /// <summary>
        /// Удаляет бронирование
        /// </summary>
        /// <param name="id">Идентификатор бронирования</param>
        /// <exception cref="ApplicationException">Выбрасывается при ошибке удаления</exception>
        /// <returns>Асинхронная задача</returns>
        public async Task DeleteAsync(int id)
        {
            try
            {
                await _reservationRepository.DeleteAsync(id);
            }
            catch (InvalidOperationException ex)
            {
                throw new ApplicationException($"Ошибка при удалении брони: {ex.Message}", ex);
            }
        }
    }
}