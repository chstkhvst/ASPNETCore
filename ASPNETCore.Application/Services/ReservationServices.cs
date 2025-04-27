using ASPNETCore.Application.DTO;
using ASPNETCore.Domain.Entities;
using ASPNETCore.Domain.Interfaces;

namespace ASPNETCore.Application.Services
{
    public class ReservationServices
    {
        private readonly IReservationRepository _reservationRepository;

        public ReservationServices(IReservationRepository reservationRepository)
        {
            _reservationRepository = reservationRepository;
        }

        // Получение всех броней
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

        // Получение брони по ID
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

        // Добавление новой брони
        public async Task AddAsync(CreateReservationDTO r)
        {
            var reservation = new Reservation
            {
                ObjectId = r.ObjectId,
                UserId = r.UserId,
                StartDate = r.StartDate,
                EndDate = r.EndDate,
                ResStatusId = r.ResStatusId
            };
            await _reservationRepository.AddAsync(reservation);
        }

        // Обновление брони
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
        }

        // Удаление брони
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
