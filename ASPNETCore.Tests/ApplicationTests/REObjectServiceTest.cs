using Moq;
using Xunit;
using System.Collections.Generic;
using System.Threading.Tasks;
using ASPNETCore.Application.Services;
using ASPNETCore.Domain.Interfaces;
using ASPNETCore.Domain.Entities;
using ASPNETCore.Application.DTO;

public class REObjectServiceTests
{
    //private readonly Mock<IREObjectRepository> _mockRepo;
    //private readonly REObjectServices _service;

    //public REObjectServiceTests()
    //{
    //    _mockRepo = new Mock<IREObjectRepository>();
    //    _service = new REObjectServices(_mockRepo.Object);
    //}

    //[Fact]
    //public async Task GetAllAsync_ShouldReturnAllObjects()
    //{
    //    // Arrange
    //    var objects = new List<REObject>
    //    {
    //        new REObject { Id = 1, Street = "Street 1", Price = 100000 },
    //        new REObject { Id = 2, Street = "Street 2", Price = 200000 }
    //    };
    //    _mockRepo.Setup(repo => repo.GetAllAsync()).ReturnsAsync(objects);
    //    // Act
    //    var result = await _service.GetAllAsync();
    //    // Assert
    //    Assert.Equal(2, result.Count());
    //    Assert.Contains(result, o => o.Street == "Street 1");
    //}

    //[Fact]
    //public async Task AddAsync_ShouldCallRepositoryMethod()
    //{
    //    // Arrange
    //    var newObject = new CreateREObjectDTO
    //    {
    //        Id = 3,
    //        Street = "New Street",
    //        Price = 300000
    //    };
    //    // Act
    //    await _service.AddAsync(newObject);
    //    // Assert
    //    _mockRepo.Verify(repo => repo.AddAsync(It.Is<REObject>(o => o.Street == "New Street")), Times.Once);
    //}
}
