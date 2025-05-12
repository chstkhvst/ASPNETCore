using ASPNETCore.Application.DTO;
using ASPNETCore.Application.Services;
using ASPNETCore.Domain.Entities;
using ASPNETCore.Domain.Interfaces;
using Moq;
using Xunit;
using Microsoft.Extensions.Logging;

namespace ASPNETCore.Tests.ApplicationTests
{
    public class ContractServiceTest
    {
        private readonly Mock<IContractRepository> _contractRepoMock;
        private readonly Mock<IReservationRepository> _reservationRepoMock;
        private readonly ContractServices _service;
        private readonly Mock<ILogger<ContractServices>> _loggermock = new();

        public ContractServiceTest()
        {
            _contractRepoMock = new Mock<IContractRepository>();
            _reservationRepoMock = new Mock<IReservationRepository>();
            _service = new ContractServices(_contractRepoMock.Object, _reservationRepoMock.Object, _loggermock.Object);
        }

        [Fact]
        public async Task AddAsync_ShouldThrow_WhenReservationNotFound()
        {
            // Arrange
            var dto = new CreateContractDTO
            {
                ReservationId = 123,
                UserId = "user1"
            };

            _reservationRepoMock.Setup(r => r.GetByIdAsync(dto.ReservationId)).ReturnsAsync((Reservation?)null);

            // Act & Assert
            var ex = await Assert.ThrowsAsync<ArgumentException>(() => _service.AddAsync(dto));
            Assert.Equal("Reservation с ID 123 не найдена.", ex.Message);
        }

        [Fact]
        public async Task AddAsync_ShouldThrow_WhenReservationStatusIsNot1()
        {
            // Arrange
            var dto = new CreateContractDTO
            {
                ReservationId = 10,
                UserId = "user1"
            };

            var reservation = new Reservation
            {
                Id = 10,
                ResStatusId = 2, // Не 1
                Object = new REObject { Price = 500000 }
            };

            _reservationRepoMock.Setup(r => r.GetByIdAsync(dto.ReservationId)).ReturnsAsync(reservation);

            // Act & Assert
            var ex = await Assert.ThrowsAsync<ArgumentException>(() => _service.AddAsync(dto));
            Assert.Equal("Договор по брони уже заключен", ex.Message);
        }

        [Fact]
        public async Task AddAsync_ShouldCalculateTotalWithCappedAddPrice()
        {
            // Arrange
            var dto = new CreateContractDTO
            {
                ReservationId = 1,
                UserId = "user1"
            };

            var reservation = new Reservation
            {
                Id = 1,
                ResStatusId = 1,
                Object = new REObject { Price = 1000000 } // Надбавка: 100000 и обрезается до 70
            };

            _reservationRepoMock.Setup(r => r.GetByIdAsync(dto.ReservationId)).ReturnsAsync(reservation);
            _contractRepoMock.Setup(r => r.AddAsync(It.IsAny<Contract>())).Returns(Task.CompletedTask);
            _reservationRepoMock.Setup(r => r.UpdateAsync(It.IsAny<Reservation>())).Returns(Task.CompletedTask);

            // Act
            await _service.AddAsync(dto);

            // Assert
            _contractRepoMock.Verify(c => c.AddAsync(It.Is<Contract>(contract =>
                contract.Total == 1000000 + 70000 &&
                contract.UserId == dto.UserId &&
                contract.ReservationId == dto.ReservationId
            )), Times.Once);
        }

        [Fact]
        public async Task AddAsync_ShouldCalculateTotalWithFullAddPrice_WhenBelowLimit()
        {
            // Arrange
            var dto = new CreateContractDTO
            {
                ReservationId = 2,
                UserId = "user2"
            };

            var reservation = new Reservation
            {
                Id = 2,
                ResStatusId = 1,
                Object = new REObject { Price = 500000 } // Надбавка 50000
            };

            _reservationRepoMock.Setup(r => r.GetByIdAsync(dto.ReservationId)).ReturnsAsync(reservation);
            _contractRepoMock.Setup(c => c.AddAsync(It.IsAny<Contract>())).Returns(Task.CompletedTask);
            _reservationRepoMock.Setup(r => r.UpdateAsync(It.IsAny<Reservation>())).Returns(Task.CompletedTask);

            // Act
            await _service.AddAsync(dto);

            // Assert
            _contractRepoMock.Verify(c => c.AddAsync(It.Is<Contract>(contract =>
                contract.Total == 550000 &&
                contract.UserId == dto.UserId &&
                contract.ReservationId == dto.ReservationId
            )), Times.Once);
        }

        [Fact]
        public async Task AddAsync_ShouldAddContractAndUpdateReservation_WhenValid()
        {
            // Arrange
            var dto = new CreateContractDTO
            {
                ReservationId = 3,
                UserId = "user3"
            };

            var reservation = new Reservation
            {
                Id = 3,
                ResStatusId = 1,
                Object = new REObject { Price = 300000 }
            };

            _reservationRepoMock.Setup(r => r.GetByIdAsync(dto.ReservationId)).ReturnsAsync(reservation);
            _contractRepoMock.Setup(c => c.AddAsync(It.IsAny<Contract>())).Returns(Task.CompletedTask);
            _reservationRepoMock.Setup(r => r.UpdateAsync(It.IsAny<Reservation>())).Returns(Task.CompletedTask);

            // Act
            await _service.AddAsync(dto);

            // Assert
            _contractRepoMock.Verify(c => c.AddAsync(It.IsAny<Contract>()), Times.Once);
            _reservationRepoMock.Verify(r => r.UpdateAsync(It.Is<Reservation>(res => res.ResStatusId == 2)), Times.Once);
        }
    }
}
