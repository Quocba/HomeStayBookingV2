using System.Linq.Expressions;
using API.Controllers;
using BusinessObject.DTO;
using BusinessObject.Entities;
using BusinessObject.Interfaces;
using Microsoft.AspNetCore.Mvc;
using MockQueryable;
using Moq;

namespace APITesting;
#pragma warning disable

public class FacilityControllerTesting
{
    private Mock<IRepository<Facility>> _mockFacilityRepo;
    private FacilityController _controller;

    [SetUp]
    public void Setup()
    {
        _mockFacilityRepo = new Mock<IRepository<Facility>>();
        _controller = new FacilityController(_mockFacilityRepo.Object);
    }

    [Test]
    public async Task AddFacility_ReturnsBadRequest_WhenRequestIsNull()
    {
        // Arrange
        List<AddFacilityDTO> request = null;

        // Act
        var result = await _controller.AddFacility(request);

        // Assert
        var badRequestResult = result as BadRequestResult;
        Assert.IsNotNull(badRequestResult);
        Assert.AreEqual(400, badRequestResult.StatusCode);
    }

    [Test]
    public async Task AddFacility_ReturnsOk_WhenAddedSuccessfully()
    {
        // Arrange
        var request = new List<AddFacilityDTO>
            {
                new AddFacilityDTO { Name = "Wifi", Description = "Free wifi" }
            };

        _mockFacilityRepo.Setup(r => r.GetAllAsync())
                         .ReturnsAsync(new List<Facility>()); // Không có facility trùng

        _mockFacilityRepo.Setup(r => r.AddAsync(It.IsAny<Facility>()))
                         .Returns(Task.CompletedTask);

        _mockFacilityRepo.Setup(r => r.SaveAsync())
                         .Returns(Task.CompletedTask);

        // Act
        var result = await _controller.AddFacility(request);

        // Assert
        var okResult = result as OkObjectResult;
        Assert.IsNotNull(okResult);
        Assert.AreEqual(200, okResult.StatusCode);
        Assert.IsTrue(okResult.Value.ToString().Contains("Add Facility Success"));
    }

    [Test]
    public async Task AddFacility_ReturnsConflict_WhenFacilityAlreadyExists()
    {
        // Arrange
        var request = new List<AddFacilityDTO>
            {
                new AddFacilityDTO { Name = "Wifi", Description = "Free wifi" }
            };

        var existingFacilities = new List<Facility>
            {
                new Facility { Name = "Wifi", IsDeleted = false }
            };

        _mockFacilityRepo.Setup(r => r.GetAllAsync())
                         .ReturnsAsync(existingFacilities);

        // Act
        var result = await _controller.AddFacility(request);

        // Assert
        var conflictResult = result as ConflictObjectResult;
        Assert.IsNotNull(conflictResult);
        Assert.AreEqual(409, conflictResult.StatusCode);
        Assert.IsTrue(conflictResult.Value.ToString().Contains("Facility Already Exists"));
    }

    [Test]
    public async Task AddFacility_ReturnsServerError_WhenExceptionThrown()
    {
        // Arrange
        var request = new List<AddFacilityDTO>
            {
                new AddFacilityDTO { Name = "Wifi", Description = "Free wifi" }
            };

        _mockFacilityRepo.Setup(r => r.GetAllAsync())
                         .ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await _controller.AddFacility(request);

        // Assert
        var objectResult = result as ObjectResult;
        Assert.IsNotNull(objectResult);
        Assert.AreEqual(500, objectResult.StatusCode);
        Assert.IsTrue(objectResult.Value.ToString().Contains("Database error"));
    }

    [Test]
    public async Task EditFacility_ReturnsBadRequest_WhenRequestIsNullOrIdIsEmpty()
    {
        // Arrange
        var request = new UpdateFacilityDTO
        {
            FacilityID = Guid.Empty, // hoặc bạn có thể test request = null
            Name = "Updated name",
            Description = "Updated description"
        };

        // Act
        var result = await _controller.EditFacility(request);

        // Assert
        Assert.IsInstanceOf<BadRequestResult>(result);
    }

    [Test]
    public async Task EditFacility_ReturnsNotFound_WhenFacilityDoesNotExist()
    {
        // Arrange
        var request = new UpdateFacilityDTO
        {
            FacilityID = Guid.NewGuid(),
            Name = "Updated name",
            Description = "Updated description"
        };

        _mockFacilityRepo.Setup(r => r.Find(It.IsAny<Expression<Func<Facility, bool>>>()))
                         .Returns(new List<Facility>().AsQueryable().BuildMock());

        // Act
        var result = await _controller.EditFacility(request);

        // Assert
        Assert.IsInstanceOf<NotFoundResult>(result);
    }

    [Test]
    public async Task EditFacility_ReturnsOk_WhenUpdateIsSuccessful()
    {
        // Arrange
        var facilityId = Guid.NewGuid();
        var existingFacility = new Facility
        {
            Id = facilityId,
            Name = "Old Name",
            Description = "Old Description",
            CreateAt = DateTime.Now.AddDays(-1)
        };

        var request = new UpdateFacilityDTO
        {
            FacilityID = facilityId,
            Name = "New Name",
            Description = "New Description"
        };

        _mockFacilityRepo.Setup(r => r.Find(It.IsAny<Expression<Func<Facility, bool>>>()))
                         .Returns(new List<Facility> { existingFacility }.AsQueryable().BuildMock());

        _mockFacilityRepo.Setup(r => r.UpdateAsync(It.IsAny<Facility>())).Returns(Task.CompletedTask);
        _mockFacilityRepo.Setup(r => r.SaveAsync()).Returns(Task.CompletedTask);

        // Act
        var result = await _controller.EditFacility(request);

        // Assert
        var okResult = result as OkObjectResult;
        Assert.IsNotNull(okResult);
        Assert.AreEqual(200, okResult.StatusCode);
        Assert.AreEqual("New Name", existingFacility.Name);
        Assert.AreEqual("New Description", existingFacility.Description);
    }

    [Test]
    public async Task EditFacility_ReturnsServerError_WhenExceptionThrown()
    {
        // Arrange
        var request = new UpdateFacilityDTO
        {
            FacilityID = Guid.NewGuid(),
            Name = "Test",
            Description = "Test"
        };

        _mockFacilityRepo
            .Setup(r => r.Find(It.IsAny<Expression<Func<Facility, bool>>>()))
            .Throws(new Exception("Database error"));

        // Act
        var result = await _controller.EditFacility(request);

        // Assert
        var objectResult = result as ObjectResult;
        Assert.IsNotNull(objectResult);
        Assert.AreEqual(500, objectResult.StatusCode);
    }

    [Test]
    public async Task GetAll_ReturnsOkResult_WhenFacilitiesExist()
    {
        // Arrange
        var facilities = new List<Facility>
    {
        new Facility { Id = Guid.NewGuid(), Name = "Facility 1", IsDeleted = false }
    }.AsQueryable();

        _mockFacilityRepo.Setup(r => r.Find(It.IsAny<Expression<Func<Facility, bool>>>()))
                         .Returns(facilities.BuildMock()); 

        var controller = new FacilityController(_mockFacilityRepo.Object);

        // Act
        var result = await controller.GetAll();

        // Assert
        Assert.IsNotNull(result);
        Assert.IsInstanceOf<OkObjectResult>(result);
    }


    [Test]
    public async Task GetAll_ReturnsNotFound_WhenNoFacilitiesExist()
    {
        // Arrange
        var emptyList = new List<Facility>().AsQueryable().BuildMock();
        _mockFacilityRepo.Setup(r => r.Find(It.IsAny<Expression<Func<Facility, bool>>>()))
                         .Returns(emptyList);

        // Act
        var result = await _controller.GetAll();

        // Assert
        var objectResult = result as ObjectResult;
        Assert.AreEqual(404, objectResult.StatusCode);
    }

    [Test]
    public async Task GetAll_ReturnsServerError_WhenExceptionThrown()
    {
        // Arrange
        _mockFacilityRepo.Setup(r => r.Find(It.IsAny<Expression<Func<Facility, bool>>>()))
                         .Throws(new Exception("Unexpected error"));

        // Act
        var result = await _controller.GetAll();

        // Assert
        var objectResult = result as ObjectResult;
        Assert.IsNotNull(objectResult);
        Assert.AreEqual(500, objectResult.StatusCode);
    }

    [Test]
    public async Task DeleteFacility_ReturnsOk_WhenFacilitiesFound()
    {
        // Arrange
        var ids = new List<Guid> { Guid.NewGuid() };
        var request = new DeleteListDTO { ID = ids };

        var mockFacilities = new List<Facility>
    {
        new Facility { Id = ids[0], Name = "Pool", IsDeleted = false }
    }.AsQueryable().BuildMock(); // Dùng MockQueryable.Moq

        _mockFacilityRepo.Setup(r => r.Find(It.IsAny<Expression<Func<Facility, bool>>>()))
                         .Returns(mockFacilities);
        _mockFacilityRepo.Setup(r => r.SaveAsync()).Returns(Task.CompletedTask);

        // Act
        var result = await _controller.DeleteFacility(request);

        // Assert
        var okResult = result as OkObjectResult;
        Assert.IsNotNull(okResult);
        Assert.AreEqual(200, okResult.StatusCode);
    }

    [Test]
    public async Task DeleteFacility_ReturnsBadRequest_WhenRequestIsNull()
    {
        // Act
        var result = await _controller.DeleteFacility(null);

        // Assert
        Assert.IsInstanceOf<BadRequestResult>(result);
    }

    [Test]
    public async Task DeleteFacility_ReturnsNotFound_WhenNoFacilitiesFound()
    {
        // Arrange
        var request = new DeleteListDTO { ID = new List<Guid> { Guid.NewGuid() } };
        var emptyFacilities = new List<Facility>().AsQueryable().BuildMock();

        _mockFacilityRepo.Setup(r => r.Find(It.IsAny<Expression<Func<Facility, bool>>>()))
                         .Returns(emptyFacilities);

        // Act
        var result = await _controller.DeleteFacility(request);

        // Assert
        Assert.IsInstanceOf<NotFoundResult>(result);
    }

    [Test]
    public async Task DeleteFacility_ReturnsServerError_WhenExceptionThrown()
    {
        // Arrange
        var request = new DeleteListDTO { ID = new List<Guid> { Guid.NewGuid() } };

        _mockFacilityRepo.Setup(r => r.Find(It.IsAny<Expression<Func<Facility, bool>>>()))
                         .Throws(new Exception("DB Error"));

        // Act
        var result = await _controller.DeleteFacility(request);

        // Assert
        var objectResult = result as ObjectResult;
        Assert.IsNotNull(objectResult);
        Assert.AreEqual(500, objectResult.StatusCode);
    }
}
