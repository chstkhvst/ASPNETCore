using ASPNETCore.Application.DTO;
using ASPNETCore.Domain.Entities;
using ASPNETCore.Domain.Interfaces;
using Microsoft.Extensions.Logging;
using System.Diagnostics.Contracts;

namespace ASPNETCore.Application.Services
{
    /// <summary>
    /// Сервис для работы с договорами
    /// </summary>
    public class ContractServices
    {
        private readonly IContractRepository _contractRepository;
        private readonly IReservationRepository _resRepository;
        private readonly ILogger<ContractServices> _logger; 

        /// <summary>
        /// Инициализирует новый экземпляр <see cref="ContractServices"/>
        /// </summary>
        /// <param name="contractRepository">Репозиторий для работы с договорами</param>
        /// <param name="resRepository">Репозиторий для работы с бронированиями</param>
        public ContractServices(IContractRepository contractRepository, IReservationRepository resRepository, ILogger<ContractServices> logger)
        {
            _contractRepository = contractRepository;
            _resRepository = resRepository;
            _logger = logger;
        }

        /// <summary>
        /// Получает все договоры
        /// </summary>
        /// <returns>Коллекция DTO договоров</returns>
        public async Task<IEnumerable<ContractDTO>> GetAllAsync()
        {
            var contracts = await _contractRepository.GetAllAsync();
            return contracts.Select(c => new ContractDTO
            {
                Id = c.Id,
                SignDate = c.SignDate,
                ReservationId = c.ReservationId,
                UserId = c.UserId,
                Total = c.Total,
                Reservation = c.Reservation,
                User = c.User,
            });
        }

        /// <summary>
        /// Получает договор по идентификатору
        /// </summary>
        /// <param name="id">Идентификатор договора</param>
        /// <returns>DTO договора или null, если не найден</returns>
        public async Task<ContractDTO> GetByIdAsync(int id)
        {
            var contract = await _contractRepository.GetByIdAsync(id);
            if (contract == null)
            {
                _logger.LogWarning($"Контракт с ID {id} не найден в БД!");
                return null;
            }

            return new ContractDTO
            {
                Id = contract.Id,
                SignDate = contract.SignDate,
                ReservationId = contract.ReservationId,
                UserId = contract.UserId,
                Total = contract.Total,
                Reservation = contract.Reservation,
                User = contract.User
            };
        }

        /// <summary>
        /// Ищет договоры по дате подписания
        /// </summary>
        /// <param name="signDate">Дата подписания</param>
        /// <returns>Коллекция DTO договоров</returns>
        public async Task<IEnumerable<ContractDTO>> SearchBySignDateAsync(DateTime signDate)
        {
            var contracts = await _contractRepository.SearchBySignDateAsync(signDate);
            return contracts.Select(c => new ContractDTO
            {
                Id = c.Id,
                SignDate = c.SignDate,
                ReservationId = c.ReservationId,
                UserId = c.UserId,
                Total = c.Total,
                Reservation = c.Reservation,
                User = c.User
            });
        }

        /// <summary>
        /// Добавляет новый контракт
        /// </summary>
        /// <param name="contractDto">DTO для создания договора</param>
        /// <exception cref="ArgumentException">
        /// Выбрасывается если:
        /// - Бронирование не найдено
        /// - По брони уже заключен договор
        /// </exception>
        /// <returns>Асинхронная задача</returns>
        public async Task AddAsync(CreateContractDTO contractDto)
        {
            var reser = await _resRepository.GetByIdAsync(contractDto.ReservationId);
            if (reser == null)
            {
                _logger.LogError($"Reservation с ID {contractDto.ReservationId} не найдена.");
                throw new ArgumentException($"Reservation с ID {contractDto.ReservationId} не найдена.");
            }
            if (reser.ResStatusId != 1)
            {
                _logger.LogError($"Договор по брони {reser.Id} уже заключен");
                throw new ArgumentException($"Договор по брони уже заключен");
            }

            int addPrice = reser.Object.Price / 10;
            if (addPrice > 70000) addPrice = 70000;

            var contract = new Domain.Entities.Contract
            {
                ReservationId = contractDto.ReservationId,
                UserId = contractDto.UserId,
                SignDate = DateTime.UtcNow,
                Total = reser.Object.Price + addPrice
            };
            await _contractRepository.AddAsync(contract);
            reser.ResStatusId = 2;
            await _resRepository.UpdateAsync(reser);
        }

        /// <summary>
        /// Обновляет существующий договор
        /// </summary>
        /// <param name="contractDto">DTO с обновленными данными договора</param>
        /// <returns>Асинхронная задача</returns>
        public async Task UpdateAsync(CreateContractDTO contractDto)
        {
            var contract = await _contractRepository.GetByIdAsync(contractDto.Id);
            if (contract != null)
            {
                contract.SignDate = contractDto.SignDate;
                contract.ReservationId = contractDto.ReservationId;
                contract.UserId = contractDto.UserId;
                contract.Total = contractDto.Total;

                await _contractRepository.UpdateAsync(contract);
            }
            else
                _logger.LogError($"Договор не найден (id {contractDto.Id})");
        }

        /// <summary>
        /// Удаляет договор по идентификатору
        /// </summary>
        /// <param name="id">Идентификатор договора</param>
        /// <exception cref="ApplicationException">Выбрасывается при ошибке удаления</exception>
        /// <returns>Асинхронная задача</returns>
        public async Task DeleteAsync(int id)
        {
            try
            {
                await _contractRepository.DeleteAsync(id);
            }
            catch (InvalidOperationException ex)
            {
                throw new ApplicationException($"Ошибка при удалении контракта: {ex.Message}", ex);
            }
        }
    }
}