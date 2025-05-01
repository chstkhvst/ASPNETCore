using ASPNETCore.Application.DTO;
using ASPNETCore.Domain.Entities;
using ASPNETCore.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ASPNETCore.Application.Services
{
    public class ContractServices
    {
        private readonly IContractRepository _contractRepository;

        public ContractServices(IContractRepository contractRepository)
        {
            _contractRepository = contractRepository;
        }

        // Получение всех контрактов
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

        // Получение контракта по ID
        public async Task<ContractDTO> GetByIdAsync(int id)
        {
            var contract = await _contractRepository.GetByIdAsync(id);
            if (contract == null)
            {
                Console.WriteLine($"Контракт с ID {id} не найден в БД!");
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

        // Поиск контрактов по дате подписания
        public async Task<IEnumerable<ContractDTO>> SearchBySignDateAsync(DateTime signDate)
        {
            var contracts = await _contractRepository.SearchBySignDateAsync(signDate);
            return contracts.Select(c => new ContractDTO
            {
                Id = c.Id,
                SignDate = c.SignDate,
                ReservationId = c.ReservationId,
                UserId = c.UserId,
                Total = c.Total
            });
        }

        // Добавление нового контракта
        //public async Task AddAsync(CreateContractDTO contractDto)
        //{
        //    var contract = new Contract
        //    {
        //        SignDate = contractDto.SignDate,
        //        ReservationId = contractDto.ReservationId,
        //        UserId = contractDto.UserId,
        //        Total = contractDto.Total
        //    };
        //    await _contractRepository.AddAsync(contract);
        //}

        public async Task AddAsync(CreateContractDTO contractDto)
        {
            var contract = new Contract
            {
                ReservationId = contractDto.ReservationId,
                UserId = contractDto.UserId
                // SignDate и Total будут установлены в репозитории
            };
            await _contractRepository.AddAsync(contract);
        }

        // Обновление контракта
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
        }

        // Удаление контракта
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