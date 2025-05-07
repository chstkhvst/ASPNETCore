using ASPNETCore.Application.DTO;
using ASPNETCore.Application.Services;
using ASPNETCore.Domain.Entities;
using ASPNETCore.Domain.Interfaces;
using Moq;

namespace ASPNETCore.Tests.ApplicationTests
{
    public class ReservationServiceTest
    {
        private readonly Mock<IREObjectRepository> _reObjectRepoMock;
        private readonly Mock<IReservationRepository> _reservationRepoMock;
        private readonly ReservationServices _service;

        public ReservationServiceTest()
        {
            _reObjectRepoMock = new Mock<IREObjectRepository>();
            _reservationRepoMock = new Mock<IReservationRepository>();
            _service = new ReservationServices(_reservationRepoMock.Object, _reObjectRepoMock.Object);
        }

        [Fact]
        public async Task AddAsync_ShouldThrow_WhenEndDateBeforeStartDate()
        {
            // Arrange
            var dto = new CreateReservationDTO
            {
                ObjectId = 1,
                UserId = "user1",
                StartDate = DateTime.Today,
                EndDate = DateTime.Today.AddDays(-1),
                ResStatusId = 1
            };

            // Act & Assert
            var ex = await Assert.ThrowsAsync<ArgumentException>(() => _service.AddAsync(dto));
            Assert.Equal("Ошибка даты", ex.Message);
        }

        [Fact]
        public async Task AddAsync_ShouldThrow_WhenObjectNotFound()
        {
            // Arrange
            var dto = new CreateReservationDTO
            {
                ObjectId = 999,
                UserId = "user1",
                StartDate = DateTime.Today,
                EndDate = DateTime.Today.AddDays(2),
                ResStatusId = 1
            };
            _reObjectRepoMock.Setup(r => r.GetByIdAsync(dto.ObjectId)).ReturnsAsync((REObject?)null);

            // Act & Assert
            var ex = await Assert.ThrowsAsync<ArgumentException>(() => _service.AddAsync(dto));
            Assert.Equal("Объект невозможно забронировать", ex.Message);
        }

        [Fact]
        public async Task AddAsync_ShouldThrow_WhenObjectNotAvailable()
        {
            // Arrange
            var dto = new CreateReservationDTO
            {
                ObjectId = 1,
                UserId = "user1",
                StartDate = DateTime.Today,
                EndDate = DateTime.Today.AddDays(2),
                ResStatusId = 1
            };

            var obj = new REObject { Id = 1, StatusId = 2 }; // Недоступен
            _reObjectRepoMock.Setup(r => r.GetByIdAsync(dto.ObjectId)).ReturnsAsync(obj);

            // Act & Assert
            var ex = await Assert.ThrowsAsync<ArgumentException>(() => _service.AddAsync(dto));
            Assert.Equal("Объект невозможно забронировать", ex.Message);
        }

        [Fact]
        public async Task AddAsync_ShouldAddReservationAndUpdateStatus_WhenValid()
        {
            // Arrange
            var dto = new CreateReservationDTO
            {
                ObjectId = 1,
                UserId = "user1",
                StartDate = DateTime.Today,
                EndDate = DateTime.Today.AddDays(3),
                ResStatusId = 1
            };

            var obj = new REObject { Id = 1, StatusId = 1 }; // Доступен

            _reObjectRepoMock.Setup(r => r.GetByIdAsync(dto.ObjectId)).ReturnsAsync(obj);
            _reservationRepoMock.Setup(r => r.AddAsync(It.IsAny<Reservation>())).Returns(Task.CompletedTask);
            _reObjectRepoMock.Setup(r => r.UpdateAsync(It.IsAny<REObject>())).Returns(Task.CompletedTask);

            // Act
            await _service.AddAsync(dto);

            // Assert
            _reservationRepoMock.Verify(r => r.AddAsync(It.Is<Reservation>(res =>
                res.ObjectId == dto.ObjectId &&
                res.UserId == dto.UserId &&
                res.StartDate == dto.StartDate &&
                res.EndDate == dto.EndDate &&
                res.ResStatusId == dto.ResStatusId
            )), Times.Once);

            _reObjectRepoMock.Verify(r => r.UpdateAsync(It.Is<REObject>(o => o.StatusId == 2)), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_ShouldUpdateReservation_WhenReservationExists()
        {
            // Arrange
            var dto = new CreateReservationDTO
            {
                Id = 1,
                ObjectId = 10,
                UserId = "user1",
                StartDate = DateTime.Today,
                EndDate = DateTime.Today.AddDays(5),
                ResStatusId = 2
            };

            var existingReservation = new Reservation
            {
                Id = 1,
                ObjectId = 99, // Старые значения
                UserId = "oldUser",
                StartDate = DateTime.Today.AddDays(-2),
                EndDate = DateTime.Today.AddDays(1),
                ResStatusId = 1
            };

            _reservationRepoMock.Setup(r => r.GetByIdAsync(dto.Id)).ReturnsAsync(existingReservation);
            _reservationRepoMock.Setup(r => r.UpdateAsync(It.IsAny<Reservation>())).Returns(Task.CompletedTask);

            // Act
            await _service.UpdateAsync(dto);

            // Assert
            _reservationRepoMock.Verify(r => r.UpdateAsync(It.Is<Reservation>(res =>
                res.ObjectId == dto.ObjectId &&
                res.UserId == dto.UserId &&
                res.StartDate == dto.StartDate &&
                res.EndDate == dto.EndDate &&
                res.ResStatusId == dto.ResStatusId
            )), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_ShouldUpdateObjectStatus_WhenResStatusIdIs3()
        {
            // Arrange
            var dto = new CreateReservationDTO
            {
                Id = 1,
                ObjectId = 10,
                UserId = "user1",
                StartDate = DateTime.Today,
                EndDate = DateTime.Today.AddDays(5),
                ResStatusId = 3
            };

            var reservation = new Reservation { Id = 1 };
            var obj = new REObject { Id = 10, StatusId = 2 };

            _reservationRepoMock.Setup(r => r.GetByIdAsync(dto.Id)).ReturnsAsync(reservation);
            _reObjectRepoMock.Setup(r => r.GetByIdAsync(dto.ObjectId)).ReturnsAsync(obj);
            _reservationRepoMock.Setup(r => r.UpdateAsync(It.IsAny<Reservation>())).Returns(Task.CompletedTask);
            _reObjectRepoMock.Setup(r => r.UpdateAsync(It.IsAny<REObject>())).Returns(Task.CompletedTask);

            // Act
            await _service.UpdateAsync(dto);

            // Assert
            _reObjectRepoMock.Verify(r => r.UpdateAsync(It.Is<REObject>(o => o.StatusId == 1)), Times.Once);
        }

    }
}
