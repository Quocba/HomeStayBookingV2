using System.Linq.Expressions;
using API.Controllers;
using BusinessObject.DTO;
using BusinessObject.Entities;
using BusinessObject.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;
#pragma warning disable

namespace APITesting;

public class CalendarControllerTesting
{
    private Mock<IRepository<Calendar>> _mockCalendarRepo;
    private Mock<IRepository<HomeStay>> _mockHomeStayRepo;
    private CalendarController _controller;
    [SetUp]
    public void Setup()
    {
        _mockCalendarRepo = new Mock<IRepository<Calendar>>();
        _mockHomeStayRepo = new Mock<IRepository<HomeStay>>();

        _controller = new CalendarController(_mockCalendarRepo.Object, _mockHomeStayRepo.Object);
    }

    [Test]
    public async Task GetById_ReturnsOk_WhenCalendarExists()
    {
        // Arrange
        var calendarId = Guid.NewGuid();
        var calendar = new Calendar
        {
            Id = calendarId,
            Date = DateTime.Today,
            Price = 500000,
            HomeStayID = Guid.NewGuid(),
            BookingID = null,
            isDeleted = false
        };

        var mockCalendarRepo = new Mock<IRepository<Calendar>>();
        var mockHomeStayRepo = new Mock<IRepository<HomeStay>>();

        mockCalendarRepo.Setup(repo => repo.GetByIdAsync(calendarId))
                        .ReturnsAsync(calendar);

        var controller = new CalendarController(mockCalendarRepo.Object, mockHomeStayRepo.Object);

        // Act
        var result = await controller.GetById(calendarId);

        // Assert
        var okResult = result as OkObjectResult;
        Assert.IsNotNull(okResult);
        Assert.AreEqual(200, okResult.StatusCode);
        Assert.AreEqual(calendar, okResult.Value);
    }

    [Test]
    public async Task GetById_ReturnsNotFound_WhenCalendarDoesNotExist()
    {
        // Arrange
        var calendarId = Guid.NewGuid();
        var mockCalendarRepo = new Mock<IRepository<Calendar>>();
        var mockHomeStayRepo = new Mock<IRepository<HomeStay>>();

        mockCalendarRepo.Setup(repo => repo.GetByIdAsync(calendarId))
                        .ReturnsAsync((Calendar)null!);

        var controller = new CalendarController(mockCalendarRepo.Object, mockHomeStayRepo.Object);

        // Act
        var result = await controller.GetById(calendarId);

        // Assert
        var notFoundResult = result as NotFoundObjectResult;
        Assert.IsNotNull(notFoundResult);
        Assert.AreEqual(404, notFoundResult.StatusCode);
    }

    [Test]
    public async Task GetByHomeStayId_ReturnsOk_WhenCalendarsExist()
    {
        // Arrange
        var homeStayId = Guid.NewGuid();
        var calendars = new List<Calendar>
    {
        new Calendar { Id = Guid.NewGuid(), HomeStayID = homeStayId, Date = DateTime.Today, isDeleted = false },
        new Calendar { Id = Guid.NewGuid(), HomeStayID = homeStayId, Date = DateTime.Today.AddDays(1), isDeleted = false },
        new Calendar { Id = Guid.NewGuid(), HomeStayID = Guid.NewGuid(), Date = DateTime.Today, isDeleted = false } // khác HomeStay
    };

        var mockCalendarRepo = new Mock<IRepository<Calendar>>();
        var mockHomeStayRepo = new Mock<IRepository<HomeStay>>();

        mockCalendarRepo.Setup(r => r.GetAllAsync())
                        .ReturnsAsync(calendars);

        var controller = new CalendarController(mockCalendarRepo.Object, mockHomeStayRepo.Object);

        // Act
        var result = await controller.GetByHomeStayId(homeStayId);

        // Assert
        var okResult = result as OkObjectResult;
        Assert.IsNotNull(okResult);
        Assert.AreEqual(200, okResult.StatusCode);

        var resultData = okResult.Value as IEnumerable<Calendar>;
        Assert.IsNotNull(resultData);
        Assert.AreEqual(2, resultData.Count());
    }

    [Test]
    public async Task GetByHomeStayId_ReturnsNotFound_WhenNoCalendarsFound()
    {
        // Arrange
        var homeStayId = Guid.NewGuid();
        var calendars = new List<Calendar>
    {
        new Calendar { Id = Guid.NewGuid(), HomeStayID = Guid.NewGuid(), isDeleted = false },
        new Calendar { Id = Guid.NewGuid(), HomeStayID = Guid.NewGuid(), isDeleted = true } // isDeleted = true
    };

        var mockCalendarRepo = new Mock<IRepository<Calendar>>();
        var mockHomeStayRepo = new Mock<IRepository<HomeStay>>();

        mockCalendarRepo.Setup(r => r.GetAllAsync())
                        .ReturnsAsync(calendars);

        var controller = new CalendarController(mockCalendarRepo.Object, mockHomeStayRepo.Object);

        // Act
        var result = await controller.GetByHomeStayId(homeStayId);

        // Assert
        var notFoundResult = result as NotFoundObjectResult;
        Assert.IsNotNull(notFoundResult);
        Assert.AreEqual(404, notFoundResult.StatusCode);
    }

    [Test]
    public async Task GetByHomeStayId_ReturnsBadRequest_WhenHomeStayIdIsEmpty()
    {
        // Arrange
        var mockCalendarRepo = new Mock<IRepository<Calendar>>();
        var mockHomeStayRepo = new Mock<IRepository<HomeStay>>();

        var controller = new CalendarController(mockCalendarRepo.Object, mockHomeStayRepo.Object);

        // Act
        var result = await controller.GetByHomeStayId(Guid.Empty);

        // Assert
        var badRequestResult = result as BadRequestResult;
        Assert.IsNotNull(badRequestResult);
        Assert.AreEqual(400, badRequestResult.StatusCode);
    }


    [Test]
    public async Task GetByHomeStayId_ReturnsInternalServerError_WhenExceptionThrown()
    {
        // Arrange
        var homeStayId = Guid.NewGuid();
        var mockCalendarRepo = new Mock<IRepository<Calendar>>();
        var mockHomeStayRepo = new Mock<IRepository<HomeStay>>();

        mockCalendarRepo.Setup(r => r.GetAllAsync())
                        .ThrowsAsync(new Exception("Database error"));

        var controller = new CalendarController(mockCalendarRepo.Object, mockHomeStayRepo.Object);

        // Act
        var result = await controller.GetByHomeStayId(homeStayId);

        // Assert
        var objectResult = result as ObjectResult;
        Assert.IsNotNull(objectResult);
        Assert.AreEqual(500, objectResult.StatusCode);
    }

    [Test]
    public async Task Create_ReturnsOk_WhenCalendarsAreCreatedSuccessfully()
    {
        // Arrange
        var homeStayId = Guid.NewGuid();
        var calendarDTOs = new List<CalendarDTO>
    {
        new CalendarDTO { Date = DateTime.Today, Price = 1000000, HomeStayID = homeStayId }
    };

        var homeStay = new HomeStay { Id = homeStayId };

        _mockHomeStayRepo.Setup(r => r.GetByIdAsync(homeStayId))
                         .ReturnsAsync(homeStay);

        _mockCalendarRepo.Setup(r =>
            r.FindAsync(It.IsAny<Expression<Func<BusinessObject.Entities.Calendar, bool>>>()))
            .ReturnsAsync(new List<BusinessObject.Entities.Calendar>().AsQueryable());

        _mockCalendarRepo.Setup(r => r.AddRangeAsync(It.IsAny<IEnumerable<BusinessObject.Entities.Calendar>>()))
                         .Returns(Task.CompletedTask);

        _mockCalendarRepo.Setup(r => r.SaveAsync())
                         .Returns(Task.CompletedTask);

        // Act
        var result = await _controller.Create(calendarDTOs);

        // Assert
        var okResult = result as OkObjectResult;
        Assert.IsNotNull(okResult);
        Assert.AreEqual(200, okResult.StatusCode);
    }


    [Test]
    public async Task Create_ReturnsBadRequest_WhenInputIsNullOrEmpty()
    {
        // Arrange
        List<CalendarDTO> calendarDTOs = null;

        // Act
        var result = await _controller.Create(calendarDTOs);

        // Assert
        var badRequest = result as BadRequestObjectResult;
        Assert.IsNotNull(badRequest);
        Assert.AreEqual(400, badRequest.StatusCode);
    }

    [Test]
    public async Task Create_ReturnsBadRequest_WhenHomeStayNotFound()
    {
        // Arrange
        var calendarDTOs = new List<CalendarDTO>
    {
        new CalendarDTO { Date = DateTime.Today, Price = 1000000, HomeStayID = Guid.NewGuid() }
    };

        _mockHomeStayRepo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>()))
                         .ReturnsAsync((HomeStay)null);

        // Act
        var result = await _controller.Create(calendarDTOs);

        // Assert
        var badRequest = result as BadRequestObjectResult;
        Assert.IsNotNull(badRequest);
        Assert.AreEqual(400, badRequest.StatusCode);
    }


    [Test]
    public async Task Create_UpdatesExistingCalendar_WhenCalendarAlreadyExists()
    {
        // Arrange
        var homeStayId = Guid.NewGuid();
        var date = DateTime.Today;
        var existingCalendar = new Calendar
        {
            Id = Guid.NewGuid(),
            Date = date,
            HomeStay = new HomeStay { Id = homeStayId },
            Price = 800000
        };

        var calendarDTOs = new List<CalendarDTO>
    {
        new CalendarDTO { Date = date, Price = 1000000, HomeStayID = homeStayId }
    };

        _mockHomeStayRepo.Setup(r => r.GetByIdAsync(homeStayId))
                         .ReturnsAsync(existingCalendar.HomeStay);

        _mockCalendarRepo.Setup(r =>
            r.FindAsync(It.IsAny<Expression<Func<Calendar, bool>>>()))
            .ReturnsAsync(new List<Calendar> { existingCalendar }.AsQueryable());

        _mockCalendarRepo.Setup(r => r.SaveAsync())
                         .Returns(Task.CompletedTask);

        // Act
        var result = await _controller.Create(calendarDTOs);

        // Assert
        var okResult = result as OkObjectResult;
        Assert.IsNotNull(okResult);
        Assert.AreEqual(200, okResult.StatusCode);

        var response = okResult.Value as dynamic;
        Assert.IsNotNull(response);
        Assert.AreEqual(1000000, existingCalendar.Price);

        _mockCalendarRepo.Verify(r => r.AddRangeAsync(It.IsAny<IEnumerable<Calendar>>()), Times.Never);

        _mockCalendarRepo.Verify(r => r.SaveAsync(), Times.Once);
    }



    [Test]
    public async Task Create_Returns500_WhenExceptionThrown()
    {
        // Arrange
        var calendarDTOs = new List<CalendarDTO>
    {
        new CalendarDTO { Date = DateTime.Today, Price = 1000000, HomeStayID = Guid.NewGuid() }
    };

        _mockHomeStayRepo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>()))
                         .ThrowsAsync(new Exception("Test error"));

        // Act
        var result = await _controller.Create(calendarDTOs);

        // Assert
        var errorResult = result as ObjectResult;
        Assert.IsNotNull(errorResult);
        Assert.AreEqual(500, errorResult.StatusCode);
        Assert.That(errorResult.Value.ToString(), Does.Contain("Test error"));
    }

    [Test]
    public async Task Update_ReturnsOk_WhenCalendarUpdatedSuccessfully()
    {
        // Arrange
        var id = Guid.NewGuid();
        var homeStayId = Guid.NewGuid();
        var calendar = new Calendar
        {
            Id = id,
            Date = DateTime.Today,
            Price = 500000,
            HomeStay = new HomeStay { Id = homeStayId }
        };

        var dto = new CalendarDTO
        {
            Date = DateTime.Today.AddDays(1),
            Price = 1000000,
            HomeStayID = homeStayId
        };

        var homeStay = new HomeStay { Id = homeStayId };

        _mockCalendarRepo.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(calendar);
        _mockHomeStayRepo.Setup(r => r.GetByIdAsync(homeStayId)).ReturnsAsync(homeStay);
        _mockCalendarRepo.Setup(r => r.UpdateAsync(calendar)).Returns(Task.CompletedTask);
        _mockCalendarRepo.Setup(r => r.SaveAsync()).Returns(Task.CompletedTask);

        // Act
        var result = await _controller.Update(id, dto);

        // Assert
        var okResult = result as OkObjectResult;
        Assert.IsNotNull(okResult);
        Assert.AreEqual(200, okResult.StatusCode);

        // Đảm bảo calendar được cập nhật đúng
        Assert.AreEqual(dto.Date, calendar.Date);
        Assert.AreEqual(dto.Price, calendar.Price);
        Assert.AreEqual(homeStay, calendar.HomeStay);
    }

    [Test]
    public async Task Update_ReturnsNotFound_WhenCalendarDoesNotExist()
    {
        // Arrange
        var id = Guid.NewGuid();
        var dto = new CalendarDTO
        {
            Date = DateTime.Today,
            Price = 1000000,
            HomeStayID = Guid.NewGuid()
        };

        _mockCalendarRepo.Setup(r => r.GetByIdAsync(id)).ReturnsAsync((Calendar)null);

        // Act
        var result = await _controller.Update(id, dto);

        // Assert
        var notFound = result as NotFoundObjectResult;
        Assert.IsNotNull(notFound);
        Assert.AreEqual(404, notFound.StatusCode);
    }

    [Test]
    public async Task Update_ReturnsBadRequest_WhenHomeStayNotFound()
    {
        // Arrange
        var id = Guid.NewGuid();
        var homeStayId = Guid.NewGuid();
        var calendar = new Calendar
        {
            Id = id,
            Date = DateTime.Today,
            Price = 500000,
            HomeStay = new HomeStay { Id = homeStayId }
        };

        var dto = new CalendarDTO
        {
            Date = DateTime.Today.AddDays(1),
            Price = 1000000,
            HomeStayID = homeStayId
        };

        _mockCalendarRepo.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(calendar);
        _mockHomeStayRepo.Setup(r => r.GetByIdAsync(homeStayId)).ReturnsAsync((HomeStay)null);

        // Act
        var result = await _controller.Update(id, dto);

        // Assert
        var badRequest = result as BadRequestObjectResult;
        Assert.IsNotNull(badRequest);
        Assert.AreEqual(400, badRequest.StatusCode);
    }

    [Test]
    public async Task Update_ReturnsBadRequest_WhenIdIsEmpty()
    {
        // Arrange
        var dto = new CalendarDTO
        {
            Date = DateTime.Today,
            Price = 1000000,
            HomeStayID = Guid.NewGuid()
        };

        // Act
        var result = await _controller.Update(Guid.Empty, dto);

        // Assert
        var badRequestResult = result as BadRequestResult;
        Assert.IsNotNull(badRequestResult);
        Assert.AreEqual(400, badRequestResult.StatusCode);
    }


    [Test]
    public async Task Update_ReturnsServerError_WhenExceptionThrown()
    {
        // Arrange
        var id = Guid.NewGuid();
        var dto = new CalendarDTO
        {
            Date = DateTime.Today,
            Price = 1000000,
            HomeStayID = Guid.NewGuid()
        };

        _mockCalendarRepo.Setup(r => r.GetByIdAsync(id))
            .ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await _controller.Update(id, dto);

        // Assert
        var statusResult = result as ObjectResult;
        Assert.IsNotNull(statusResult);
        Assert.AreEqual(500, statusResult.StatusCode);
        Assert.IsTrue(statusResult.Value.ToString().Contains("Database error"));
    }

    [Test]
    public async Task SoftDelete_ReturnsOk_WhenCalendarExists()
    {
        // Arrange
        var id = Guid.NewGuid();
        var calendar = new Calendar { Id = id, isDeleted = false };

        _mockCalendarRepo.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(calendar);
        _mockCalendarRepo.Setup(r => r.UpdateAsync(calendar)).Returns(Task.CompletedTask);
        _mockCalendarRepo.Setup(r => r.SaveAsync()).Returns(Task.CompletedTask);

        // Act
        var result = await _controller.SoftDelete(id);

        // Assert
        var okResult = result as OkObjectResult;
        Assert.IsNotNull(okResult);
        Assert.AreEqual(200, okResult.StatusCode);
        Assert.IsTrue(calendar.isDeleted); // Calendar phải bị đánh dấu là đã xóa
    }

    [Test]
    public async Task SoftDelete_ReturnsBadRequest_WhenIdIsEmpty()
    {
        // Act
        var result = await _controller.SoftDelete(Guid.Empty);

        // Assert
        var badRequestResult = result as BadRequestResult;
        Assert.IsNotNull(badRequestResult);
        Assert.AreEqual(400, badRequestResult.StatusCode);
    }

    [Test]
    public async Task SoftDelete_ReturnsNotFound_WhenCalendarDoesNotExist()
    {
        // Arrange
        var id = Guid.NewGuid();
        _mockCalendarRepo.Setup(r => r.GetByIdAsync(id)).ReturnsAsync((Calendar)null);

        // Act
        var result = await _controller.SoftDelete(id);

        // Assert
        var notFoundResult = result as NotFoundObjectResult;
        Assert.IsNotNull(notFoundResult);
        Assert.AreEqual(404, notFoundResult.StatusCode);
        Assert.IsTrue(notFoundResult.Value.ToString().Contains("Calendar not found"));
    }

    [Test]
    public async Task SoftDelete_ReturnsServerError_WhenExceptionThrown()
    {
        // Arrange
        var id = Guid.NewGuid();
        _mockCalendarRepo.Setup(r => r.GetByIdAsync(id)).ThrowsAsync(new Exception("Unexpected error"));

        // Act
        var result = await _controller.SoftDelete(id);

        // Assert
        var objectResult = result as ObjectResult;
        Assert.IsNotNull(objectResult);
        Assert.AreEqual(500, objectResult.StatusCode);
        Assert.IsTrue(objectResult.Value.ToString().Contains("Unexpected error"));
    }
}
