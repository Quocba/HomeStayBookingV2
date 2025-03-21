using System.Globalization;
using System.Linq.Expressions;
using API.Controllers;
using BusinessObject.DTO;
using BusinessObject.Entities;
using BusinessObject.Interfaces;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Configuration;
using MockQueryable;
using Moq;
using Newtonsoft.Json;
using PayOSService.Services;
using MockQueryable.Moq;
using APITesting.DTO;
using Microsoft.EntityFrameworkCore;
#pragma warning disable

namespace APITesting.TestCase;

public class BookingControllerTesting
{
    private Mock<IBookingRepository> _mockBookingRepo;
    private Mock<IRepository<Voucher>> _mockVoucherRepo;
    private Mock<IUserRepository> _mockUserRepo;
    private Mock<IRepository<HomeStay>> _mockHomeStayRepo;
    private Mock<IEmailSender> _mockEmailSender;
    private Mock<IPayOSService> _mockPayOSService;
    private Mock<IConfiguration> _mockConfiguration;
    private Mock<IRepository<BusinessObject.Entities.Calendar>> _mockCalendarRepo;
    private BookingController _controller;
    [SetUp]
    public void Setup()
    {
        _mockBookingRepo = new Mock<IBookingRepository>();
        _mockVoucherRepo = new Mock<IRepository<Voucher>>();
        _mockUserRepo = new Mock<IUserRepository>();
        _mockHomeStayRepo = new Mock<IRepository<HomeStay>>();
        _mockEmailSender = new Mock<IEmailSender>();
        _mockPayOSService = new Mock<IPayOSService>();
        _mockConfiguration = new Mock<IConfiguration>();
        _mockCalendarRepo = new Mock<IRepository<BusinessObject.Entities.Calendar>>();

        _controller = new BookingController(
            _mockBookingRepo.Object,
            _mockVoucherRepo.Object,
            _mockUserRepo.Object,
            _mockHomeStayRepo.Object,
            _mockEmailSender.Object,
            _mockPayOSService.Object,
            _mockConfiguration.Object,
            _mockCalendarRepo.Object
        );
    }

    [Test]
    public async Task GetBookingHistory_UserNotFound_ReturnsNotFound()
    {
        // Arrange
        var userId = Guid.NewGuid();
        _mockUserRepo.Setup(r => r.GetByIdAsync(userId)).ReturnsAsync((User)null);

        // Act
        var result = await _controller.GetBookingHistory(userId) as NotFoundObjectResult;

        // Assert
        Assert.NotNull(result);
        Assert.AreEqual(404, result.StatusCode);
    }

    [Test]
    public async Task GetBookingHistory_NoBookings_ReturnsOkWithMessage()
    {
        // Arrange
        var userId = Guid.NewGuid();
        _mockUserRepo.Setup(r => r.GetByIdAsync(userId)).ReturnsAsync(new User());
        _mockBookingRepo.Setup(r => r.GetHistory(userId)).ReturnsAsync(new List<Booking>());

        // Act
        var result = await _controller.GetBookingHistory(userId) as OkObjectResult;

        // Assert
        Assert.NotNull(result);
        Assert.AreEqual(200, result.StatusCode);

        var json = JsonConvert.SerializeObject(result.Value);
        var response = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
        Assert.That(response["Message"], Is.EqualTo("No booking history found."));
    }

    [Test]
    public async Task GetBookingHistory_HasBookings_ReturnsBookingList()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var bookings = new List<Booking>
    {
        new Booking
        {
            Id = Guid.NewGuid(),
            CheckInDate = new DateTime(2025, 3, 21, 14, 0, 0),
            CheckOutDate = new DateTime(2025, 3, 22, 12, 0, 0),
            TotalPrice = 1500000,
            Status = "Completed"
        }
    };

        _mockUserRepo.Setup(repo => repo.GetByIdAsync(userId)).ReturnsAsync(new User());
        _mockBookingRepo.Setup(repo => repo.GetHistory(userId)).ReturnsAsync(bookings);

        // Act
        var result = await _controller.GetBookingHistory(userId) as OkObjectResult;

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(200, result.StatusCode);

        var json = JsonConvert.SerializeObject(result.Value);
        var list = JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(json);

        Assert.IsNotNull(list);
        Assert.AreEqual("Completed", list[0]["Status"].ToString());
    }

    [Test]
    public async Task CreateBooking_BookingDtoNull_ReturnsBadRequest()
    {
        // Arrange
        var userId = Guid.NewGuid();

        // Act
        var result = await _controller.CreateBooking(userId, null) as BadRequestObjectResult;

        // Assert
        Assert.NotNull(result);
        Assert.AreEqual(400, result.StatusCode);

        var json = JsonConvert.SerializeObject(result.Value);
        var response = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
        Assert.That(response["Message"], Is.EqualTo("Invalid booking data"));
    }

    [Test]
    public async Task CreateBooking_UserNotFound_ReturnsNotFound()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var dto = new BookingDTO { Calenders = new List<BookingCalendarDTO> { new() } };

        _mockUserRepo.Setup(r => r.GetByIdAsync(userId)).ReturnsAsync((User)null);

        // Act
        var result = await _controller.CreateBooking(userId, dto) as NotFoundObjectResult;

        // Assert
        Assert.NotNull(result);
        Assert.AreEqual(404, result.StatusCode);

        var json = JsonConvert.SerializeObject(result.Value);
        var response = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
        Assert.That(response["Message"], Is.EqualTo("User not found"));
    }

    [Test]
    public async Task CreateBooking_ValidRequest_ReturnsOk()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var homeStayId = Guid.NewGuid();
        var calendarId1 = Guid.NewGuid();
        var calendarId2 = Guid.NewGuid();

        var bookingDto = new BookingDTO
        {
            Calenders = new List<BookingCalendarDTO>
        {
            new BookingCalendarDTO { CalenderID = calendarId1 },
            new BookingCalendarDTO { CalenderID = calendarId2 }
        },
            VoucherCode = null,
            IsOnline = true
        };

        var user = new User
        {
            Id = userId,
            FullName = "Test User",
            Email = "test@example.com"
        };

        var calendar1 = new BusinessObject.Entities.Calendar
        {
            Id = calendarId1,
            Date = DateTime.Today,
            Price = 500000,
            HomeStayID = homeStayId,
            BookingID = null,
            isBooked = false
        };

        var calendar2 = new BusinessObject.Entities.Calendar
        {
            Id = calendarId2,
            Date = DateTime.Today.AddDays(1),
            Price = 500000,
            HomeStayID = homeStayId,
            BookingID = null,
            isBooked = false
        };

        var homeStay = new HomeStay
        {
            Id = homeStayId,
            CheckInTime = "14:00",
            CheckOutTime = "12:00",
        };

        _mockUserRepo.Setup(x => x.GetByIdAsync(userId)).ReturnsAsync(user);

        _mockCalendarRepo.Setup(x =>
            x.FindAsync(It.IsAny<Expression<Func<BusinessObject.Entities.Calendar, bool>>>()))
            .ReturnsAsync(new List<BusinessObject.Entities.Calendar> { calendar1, calendar2 }.AsQueryable());


        _mockHomeStayRepo.Setup(x => x.GetByIdAsync(homeStayId)).ReturnsAsync(homeStay);

        _mockBookingRepo.Setup(x => x.AddAsync(It.IsAny<Booking>())).Returns(Task.CompletedTask);
        _mockCalendarRepo.Setup(x => x.UpdateAsync(It.IsAny<BusinessObject.Entities.Calendar>())).Returns(Task.CompletedTask);
        _mockBookingRepo.Setup(x => x.SaveAsync()).Returns(Task.CompletedTask);
        _mockCalendarRepo.Setup(x => x.SaveAsync()).Returns(Task.CompletedTask);
        _mockEmailSender.Setup(x => x.SendEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .Returns(Task.CompletedTask);

        var result = await _controller.CreateBooking(userId, bookingDto);

        // Assert
        Assert.NotNull(result);
        var okResult = result as OkObjectResult;
        Assert.AreEqual(200, okResult.StatusCode);
    }

    [Test]
    public async Task CreateBooking_EmptyCalenders_ReturnsBadRequest()
    {
        var userId = Guid.NewGuid();

        var bookingDto = new BookingDTO
        {
            Calenders = new List<BookingCalendarDTO>()
        };

        var result = await _controller.CreateBooking(userId, bookingDto);

        var badRequest = result as BadRequestObjectResult;
        Assert.NotNull(badRequest);
        Assert.AreEqual(400, badRequest.StatusCode);
    }


    [Test]
    public async Task CreateBooking_NoCalendarsFound_ReturnsBadRequest()
    {
        var userId = Guid.NewGuid();
        var calendarId = Guid.NewGuid();

        var bookingDto = new BookingDTO
        {
            Calenders = new List<BookingCalendarDTO> { new BookingCalendarDTO { CalenderID = calendarId } }
        };

        _mockUserRepo.Setup(x => x.GetByIdAsync(userId)).ReturnsAsync(new User());
        _mockCalendarRepo.Setup(x => x.FindAsync(It.IsAny<Expression<Func<BusinessObject.Entities.Calendar, bool>>>()))
            .ReturnsAsync(new List<BusinessObject.Entities.Calendar>().AsQueryable());

        var result = await _controller.CreateBooking(userId, bookingDto);

        var badRequest = result as BadRequestObjectResult;
        Assert.NotNull(badRequest);
        Assert.AreEqual(400, badRequest.StatusCode);
    }

    [Test]
    public async Task CreateBooking_HomeStayNotFound_ReturnsBadRequest()
    {
        var userId = Guid.NewGuid();
        var calendarId = Guid.NewGuid();
        var homeStayId = Guid.NewGuid();

        var calendar = new BusinessObject.Entities.Calendar
        {
            Id = calendarId,
            HomeStayID = homeStayId,
            BookingID = null,
            isBooked = false,
            Date = DateTime.Today,
            Price = 100000
        };

        var bookingDto = new BookingDTO
        {
            Calenders = new List<BookingCalendarDTO> { new BookingCalendarDTO { CalenderID = calendarId } }
        };

        _mockUserRepo.Setup(x => x.GetByIdAsync(userId)).ReturnsAsync(new User());
        _mockCalendarRepo.Setup(x => x.FindAsync(It.IsAny<Expression<Func<BusinessObject.Entities.Calendar, bool>>>()))
            .ReturnsAsync(new List<BusinessObject.Entities.Calendar> { calendar }.AsQueryable());

        _mockHomeStayRepo.Setup(x => x.GetByIdAsync(homeStayId)).ReturnsAsync((HomeStay)null);

        var result = await _controller.CreateBooking(userId, bookingDto);

        var badRequest = result as BadRequestObjectResult;
        Assert.NotNull(badRequest);
        Assert.AreEqual(400, badRequest.StatusCode);
    }

    [Test]
    public async Task CreateBooking_CalendarAlreadyBooked_ReturnsBadRequest()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var calendarId = Guid.NewGuid();
        var homeStayId = Guid.NewGuid();

        var calendar = new BusinessObject.Entities.Calendar
        {
            Id = calendarId,
            HomeStayID = homeStayId,
            BookingID = Guid.NewGuid(), // Đã được đặt
            isBooked = true,
            Date = DateTime.Today,
            Price = 100000
        };

        var bookingDto = new BookingDTO
        {
            Calenders = new List<BookingCalendarDTO>
        {
            new BookingCalendarDTO { CalenderID = calendarId }
        }
        };

        var homeStay = new HomeStay
        {
            Id = homeStayId,
            CheckInTime = "14:00",
            CheckOutTime = "12:00"
        };

        _mockUserRepo.Setup(x => x.GetByIdAsync(userId)).ReturnsAsync(new User());
        _mockCalendarRepo.Setup(x =>
            x.FindAsync(It.IsAny<Expression<Func<BusinessObject.Entities.Calendar, bool>>>()))
            .ReturnsAsync(new List<BusinessObject.Entities.Calendar> { calendar }.AsQueryable());
        _mockHomeStayRepo.Setup(x => x.GetByIdAsync(homeStayId)).ReturnsAsync(homeStay);

        // Act
        var result = await _controller.CreateBooking(userId, bookingDto);

        // Assert
        var badRequest = result as BadRequestObjectResult;
        Assert.NotNull(badRequest);
        Assert.AreEqual(400, badRequest.StatusCode);
    }

    [Test]
    public async Task ConfirmBooking_BookingNotFound_ReturnsNotFound()
    {
        var bookingId = Guid.NewGuid();
        _mockBookingRepo.Setup(x => x.GetByIdAsync(bookingId)).ReturnsAsync((Booking)null!);

        var result = await _controller.ConfirmBooking(bookingId);

        Assert.IsInstanceOf<NotFoundResult>(result);
    }


    [Test]
    public async Task ConfirmBooking_UserNotFound_ReturnsNotFound()
    {
        // Arrange
        var bookingId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var calendarId = Guid.NewGuid();
        var homeStayId = Guid.NewGuid();

        var homeStay = new HomeStay { Id = homeStayId };

        var calendar = new BusinessObject.Entities.Calendar
        {
            Id = calendarId,
            HomeStayID = homeStayId,
            HomeStay = homeStay
        };

        var booking = new Booking
        {
            Id = bookingId,
            UserID = userId,
            Calendars = new List<BusinessObject.Entities.Calendar> { calendar }
        };

        _mockBookingRepo.Setup(x => x.GetByIdAsync(bookingId)).ReturnsAsync(booking);
        _mockUserRepo.Setup(x => x.GetByIdAsync(userId)).ReturnsAsync((User)null!);

        var mockBookings = new List<Booking> { booking }.AsQueryable().BuildMock();
        _mockBookingRepo.Setup(x => x.FindWithInclude()).Returns(mockBookings);

        // Act
        var result = await _controller.ConfirmBooking(bookingId);

        // Assert
        Assert.IsInstanceOf<NotFoundResult>(result);
    }

    [Test]
    public async Task ConfirmBooking_Success_ReturnsRedirect()
    {

        var bookingId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        var homeStay = new HomeStay
        {
            Id = Guid.NewGuid(),
            Name = "Test HomeStay",
            Address = "123 Street",
            MainImage = "https://example.com/image.jpg"
        };

        var user = new User
        {
            Id = userId,
            Email = "user@example.com",
            FullName = "Nguyen Van A",
            Phone = "0123456789"
        };

        var calendar = new BusinessObject.Entities.Calendar
        {
            Id = Guid.NewGuid(),
            HomeStay = homeStay
        };

        var booking = new Booking
        {
            Id = bookingId,
            UserID = userId,
            Status = "Pending",
            CheckInDate = DateTime.Now,
            CheckOutDate = DateTime.Now.AddDays(2),
            TotalPrice = 200,
            Calendars = new List<BusinessObject.Entities.Calendar> { calendar },
            User = user
        };


        var mockBookingQueryable = new List<Booking> { booking }
            .AsQueryable()
            .BuildMock()
            .BuildMockDbSet();
        _mockConfiguration.Setup(x => x["Base:UrlClient"]).Returns("http://localhost:3000");
        _mockBookingRepo.Setup(x => x.GetByIdAsync(bookingId)).ReturnsAsync(booking);
        _mockUserRepo.Setup(x => x.GetByIdAsync(userId)).ReturnsAsync(user);
        _mockBookingRepo.Setup(x => x.FindWithInclude()).Returns(mockBookingQueryable.Object);
        _mockBookingRepo.Setup(x => x.SaveAsync()).Returns(Task.CompletedTask);
        _mockEmailSender.Setup(x => x.SendEmailAsync(
            user.Email,
            It.IsAny<string>(),
            It.IsAny<string>()
        )).Returns(Task.CompletedTask);

        // Act
        var result = await _controller.ConfirmBooking(bookingId);

        // Assert
        Assert.IsInstanceOf<RedirectResult>(result);
        var redirect = result as RedirectResult;
        Assert.That(redirect!.Url, Is.EqualTo(_mockConfiguration.Object["Base:UrlClient"]));
        Assert.That(booking.Status, Is.EqualTo("Confirmed"));
    }

    [Test]
    public async Task CancelBooking_Success_ReturnsOk()
    {
        // Arrange
        var bookingId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        var booking = new Booking
        {
            Id = bookingId,
            UserID = userId,
            Status = "Pending",
            CheckInDate = DateTime.UtcNow.AddDays(2),
            CheckOutDate = DateTime.UtcNow.AddDays(4),
            TotalPrice = 500
        };

        var user = new User
        {
            Id = userId,
            Email = "test@example.com",
            FullName = "Nguyen Van A"
        };

        _mockBookingRepo.Setup(x => x.GetByIdAsync(bookingId)).ReturnsAsync(booking);
        _mockUserRepo.Setup(x => x.GetByIdAsync(userId)).ReturnsAsync(user);
        _mockConfiguration.Setup(x => x["Base:Url"]).Returns("https://client.example.com");
        _mockEmailSender.Setup(x => x.SendEmailAsync(
            user.Email,
            It.IsAny<string>(),
            It.IsAny<string>()
        )).Returns(Task.CompletedTask);

        // Act
        var result = await _controller.CancelBooking(bookingId);

        // Assert
        Assert.IsInstanceOf<OkObjectResult>(result);
        var ok = result as OkObjectResult;
        Assert.That(ok!.Value, Is.Not.Null);
        StringAssert.Contains("Cancellation request sent", ok.Value!.ToString());
    }

    [Test]
    public async Task CancelBooking_BookingNotFound_ReturnsNotFound()
    {
        var bookingId = Guid.NewGuid();

        _mockBookingRepo.Setup(x => x.GetByIdAsync(bookingId)).ReturnsAsync((Booking?)null);

        var result = await _controller.CancelBooking(bookingId);

        Assert.IsInstanceOf<NotFoundObjectResult>(result);
        var notFound = result as NotFoundObjectResult;
        StringAssert.Contains("Booking not found", notFound!.Value!.ToString());
    }

    [Test]
    public async Task CancelBooking_PaidBooking_ReturnsBadRequest()
    {
        var booking = new Booking
        {
            Id = Guid.NewGuid(),
            Status = "Paid",
            CheckInDate = DateTime.UtcNow.AddDays(3)
        };

        _mockBookingRepo.Setup(x => x.GetByIdAsync(booking.Id)).ReturnsAsync(booking);

        var result = await _controller.CancelBooking(booking.Id);

        Assert.IsInstanceOf<BadRequestObjectResult>(result);
        var badRequest = result as BadRequestObjectResult;
        StringAssert.Contains("Cannot cancel a paid booking", badRequest!.Value!.ToString());
    }

    [Test]
    public async Task CancelBooking_AlreadyCanceled_ReturnsBadRequest()
    {
        var booking = new Booking
        {
            Id = Guid.NewGuid(),
            Status = "Canceled",
            CheckInDate = DateTime.UtcNow.AddDays(3)
        };

        _mockBookingRepo.Setup(x => x.GetByIdAsync(booking.Id)).ReturnsAsync(booking);

        var result = await _controller.CancelBooking(booking.Id);

        Assert.IsInstanceOf<BadRequestObjectResult>(result);
        var badRequest = result as BadRequestObjectResult;
        StringAssert.Contains("already canceled", badRequest!.Value!.ToString());
    }

    [Test]
    public async Task CancelBooking_LessThanOneDayBeforeCheckIn_ReturnsBadRequest()
    {
        var booking = new Booking
        {
            Id = Guid.NewGuid(),
            Status = "Pending",
            CheckInDate = DateTime.UtcNow.AddHours(12)
        };

        _mockBookingRepo.Setup(x => x.GetByIdAsync(booking.Id)).ReturnsAsync(booking);

        var result = await _controller.CancelBooking(booking.Id);

        Assert.IsInstanceOf<BadRequestObjectResult>(result);
        var badRequest = result as BadRequestObjectResult;
        StringAssert.Contains("Cannot cancel less than 1 day", badRequest!.Value!.ToString());
    }

    [Test]
    public async Task CancelBooking_UserNotFound_ReturnsNotFound()
    {
        var booking = new Booking
        {
            Id = Guid.NewGuid(),
            UserID = Guid.NewGuid(),
            Status = "Pending",
            CheckInDate = DateTime.UtcNow.AddDays(3)
        };

        _mockBookingRepo.Setup(x => x.GetByIdAsync(booking.Id)).ReturnsAsync(booking);
        _mockUserRepo.Setup(x => x.GetByIdAsync(booking.UserID)).ReturnsAsync((User?)null);

        var result = await _controller.CancelBooking(booking.Id);

        Assert.IsInstanceOf<NotFoundObjectResult>(result);
        var notFound = result as NotFoundObjectResult;
        StringAssert.Contains("User not found", notFound!.Value!.ToString());
    }

    [Test]
    public async Task ConfirmCancelBooking_Success_ReturnsOk()
    {
        // Arrange
        var bookingId = Guid.NewGuid();

        var booking = new Booking
        {
            Id = bookingId,
            Status = "Pending"
        };

        var calendars = new List<BusinessObject.Entities.Calendar>
    {
        new BusinessObject.Entities.Calendar { Id = Guid.NewGuid(), BookingID = bookingId },
        new BusinessObject.Entities.Calendar { Id = Guid.NewGuid(), BookingID = bookingId }
    }.AsQueryable();

        var mockCalendarQueryable = calendars.BuildMock().BuildMockDbSet();

        _mockBookingRepo.Setup(x => x.GetByIdAsync(bookingId)).ReturnsAsync(booking);
        _mockCalendarRepo.Setup(x => x.FindAsync(It.IsAny<Expression<Func<BusinessObject.Entities.Calendar, bool>>>())).ReturnsAsync(calendars);
        _mockCalendarRepo.Setup(x => x.SaveAsync()).Returns(Task.CompletedTask);
        _mockBookingRepo.Setup(x => x.SaveAsync()).Returns(Task.CompletedTask);

        // Act
        var result = await _controller.ConfirmCancelBooking(bookingId);

        // Assert
        Assert.IsInstanceOf<OkObjectResult>(result);
        var ok = result as OkObjectResult;
        StringAssert.Contains("successfully canceled", ok!.Value!.ToString());

        Assert.That(booking.Status, Is.EqualTo("Canceled"));
        foreach (var calendar in calendars)
        {
            Assert.That(calendar.BookingID, Is.Null);
        }
    }


    [Test]
    public async Task ConfirmCancelBooking_BookingNotFound_ReturnsNotFound()
    {
        // Arrange
        var bookingId = Guid.NewGuid();

        _mockBookingRepo.Setup(x => x.GetByIdAsync(bookingId)).ReturnsAsync((Booking?)null);

        // Act
        var result = await _controller.ConfirmCancelBooking(bookingId);

        // Assert
        Assert.IsInstanceOf<NotFoundObjectResult>(result);
        var notFound = result as NotFoundObjectResult;
        StringAssert.Contains("Booking not found", notFound!.Value!.ToString());
    }

    [Test]
    public async Task ConfirmCancelBooking_AlreadyCanceled_ReturnsBadRequest()
    {
        // Arrange
        var booking = new Booking
        {
            Id = Guid.NewGuid(),
            Status = "Canceled"
        };

        _mockBookingRepo.Setup(x => x.GetByIdAsync(booking.Id)).ReturnsAsync(booking);

        // Act
        var result = await _controller.ConfirmCancelBooking(booking.Id);

        // Assert
        Assert.IsInstanceOf<BadRequestObjectResult>(result);
        var badRequest = result as BadRequestObjectResult;
        StringAssert.Contains("already canceled", badRequest!.Value!.ToString());
    }

    [Test]
    public async Task ConfirmCancelBooking_ExceptionThrown_ReturnsStatus500()
    {
        // Arrange
        var bookingId = Guid.NewGuid();

        _mockBookingRepo.Setup(x => x.GetByIdAsync(bookingId))
            .ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await _controller.ConfirmCancelBooking(bookingId);

        // Assert
        Assert.IsInstanceOf<ObjectResult>(result);
        var statusResult = result as ObjectResult;

        Assert.That(statusResult!.StatusCode, Is.EqualTo(500));
        StringAssert.Contains("Database error", statusResult.Value!.ToString());
    }

    [Test]
    public async Task ConfirmBookingStatus_StatusIsConfirmed_ReturnsOk()
    {
        // Arrange
        var bookingId = Guid.NewGuid();
        var booking = new Booking
        {
            Id = bookingId,
            Status = "Confirmed"
        };

        _mockBookingRepo.Setup(x => x.GetByIdAsync(bookingId)).ReturnsAsync(booking);
        _mockBookingRepo.Setup(x => x.UpdateAsync(booking)).Returns(Task.CompletedTask);
        _mockBookingRepo.Setup(x => x.SaveAsync()).Returns(Task.CompletedTask);

        // Act
        var result = await _controller.ConfirmBookingStatus(bookingId);

        // Assert
        Assert.IsInstanceOf<OkObjectResult>(result);
        var okResult = result as OkObjectResult;
        Assert.That(okResult!.Value!.ToString(), Does.Contain("Update Booking Status Success"));
        Assert.That(booking.Status, Is.EqualTo("Paid"));
    }

    [Test]
    public async Task ConfirmBookingStatus_BookingNotFoundOrInvalidStatus_ReturnsNotFound()
    {
        // Arrange
        var bookingId = Guid.NewGuid();
        var booking = new Booking
        {
            Id = bookingId,
            Status = "Pending" // Không phải "Confirmed"
        };

        _mockBookingRepo.Setup(x => x.GetByIdAsync(bookingId)).ReturnsAsync(booking);

        // Act
        var result = await _controller.ConfirmBookingStatus(bookingId);

        // Assert
        Assert.IsInstanceOf<NotFoundResult>(result);
    }

    [Test]
    public async Task ConfirmBookingStatus_ExceptionThrown_ReturnsStatus500()
    {
        // Arrange
        var bookingId = Guid.NewGuid();
        _mockBookingRepo.Setup(x => x.GetByIdAsync(bookingId)).ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await _controller.ConfirmBookingStatus(bookingId);

        // Assert
        Assert.IsInstanceOf<ObjectResult>(result);
        var objectResult = result as ObjectResult;
        Assert.That(objectResult!.StatusCode, Is.EqualTo(500));
        StringAssert.Contains("Database error", objectResult.Value!.ToString());
    }

    [Test]
    public async Task HomeStayRevenueStatistics_ReturnsOk_WithCorrectData()
    {
        // Arrange
        var homeStayId = Guid.NewGuid();
        _mockCalendarRepo.Setup(x => x.FindWithInclude(It.IsAny<Expression<Func<BusinessObject.Entities.Calendar, object>>>()))
            .Returns(new List<BusinessObject.Entities.Calendar>
            {
            new BusinessObject.Entities.Calendar
            {
                Booking = new Booking
                {
                    CheckInDate = new DateTime(2023, 5, 10),
                    Status = "Paid",
                    TotalPrice = 200,
                },
                HomeStayID = homeStayId
            }
            }.AsQueryable().BuildMockDbSet().Object);

        // Act
        var result = await _controller.HomeStayRevenueStatistics(homeStayId, 2023);
        var objectResult = result as ObjectResult;

        // Assert
        Assert.IsNotNull(objectResult);
        Assert.That(objectResult.StatusCode, Is.EqualTo(200));

        // Chuyển kết quả sang list có kiểu cụ thể
        var json = JsonConvert.SerializeObject(objectResult.Value);
        var list = JsonConvert.DeserializeObject<List<RevenueStatisticResult>>(json);

        Assert.IsNotNull(list);

        var may = list.FirstOrDefault(x => x.Month == "May");
        Assert.IsNotNull(may);
        Assert.That(may.Booking, Is.EqualTo(1));
        Assert.That(may.Revenue, Is.EqualTo(200));
    }


    [Test]
    public async Task HomeStayRevenueStatistics_ReturnsEmptyList_WhenNoPaidBookings()
    {
        // Arrange
        var homeStayId = Guid.NewGuid();

        _mockCalendarRepo.Setup(x => x.FindWithInclude(It.IsAny<Expression<Func<BusinessObject.Entities.Calendar, object>>>()))
            .Returns(new List<BusinessObject.Entities.Calendar>
            {
            new BusinessObject.Entities.Calendar
            {
                Booking = new Booking
                {
                    CheckInDate = new DateTime(2023, 5, 10),
                    Status = "Pending", // not Paid
                    TotalPrice = 100,
                },
                HomeStayID = homeStayId
            }
            }.AsQueryable().BuildMockDbSet().Object);

        // Act
        var result = await _controller.HomeStayRevenueStatistics(homeStayId, 2023);
        var objectResult = result as ObjectResult;

        // Assert
        Assert.IsNotNull(objectResult);
        Assert.That(objectResult.StatusCode, Is.EqualTo(200));

        // Chuyển objectResult.Value sang List<RevenueStatisticResult>
        var json = JsonConvert.SerializeObject(objectResult.Value);
        var list = JsonConvert.DeserializeObject<List<RevenueStatisticResult>>(json);

        Assert.IsNotNull(list);
        foreach (var item in list)
        {
            Assert.That(item.Booking, Is.EqualTo(0));
            Assert.That(item.Revenue, Is.EqualTo(0));
        }
    }



    [Test]
    public async Task HomeStayRevenueStatistics_Returns500_OnException()
    {
        // Arrange
        _mockCalendarRepo
            .Setup(x => x.FindWithInclude(It.IsAny<Expression<Func<BusinessObject.Entities.Calendar, object>>>()))
            .Throws(new Exception("DB error"));


        // Act
        var result = await _controller.HomeStayRevenueStatistics(Guid.NewGuid(), 2025);

        // Assert
        var objectResult = result as ObjectResult;
        Assert.IsNotNull(objectResult);
        Assert.That(objectResult.StatusCode, Is.EqualTo(500));
        Assert.That(objectResult.Value.ToString(), Does.Contain("DB error"));
    }

    [Test]
    public async Task ExportBookingByHomeStayID_ReturnsExcelFile_WhenBookingsExist()
    {
        // Arrange
        var homeStayId = Guid.NewGuid();
        var bookingId = Guid.NewGuid();

        var calendarList = new List<BusinessObject.Entities.Calendar>
    {
        new BusinessObject.Entities.Calendar
        {
            HomeStayID = homeStayId,
            HomeStay = new HomeStay { Name = "Test Homestay" },
            Booking = new Booking
            {
                Id = bookingId,
                CheckInDate = DateTime.Today,
                CheckOutDate = DateTime.Today.AddDays(2),
                UnitPrice = 500000,
                TotalPrice = 1000000,
                Status = "Confirmed",
                ReasonCancel = null
            }
        }
    };

        var mockQueryable = calendarList.AsQueryable().BuildMock();

        _mockCalendarRepo
            .Setup(repo => repo.FindWithInclude(It.IsAny<Expression<Func<BusinessObject.Entities.Calendar, object>>[]>()))
            .Returns(mockQueryable);

        var result = await _controller.ExportBookingByHomeStayID(homeStayId);

        var fileResult = result as FileStreamResult;
        Assert.IsNotNull(fileResult, "Kết quả trả về không phải file (null)");
        Assert.AreEqual("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileResult.ContentType);
        Assert.IsTrue(fileResult.FileDownloadName.Contains(homeStayId.ToString()), "Tên file không chứa đúng HomeStay ID");
    }

    [Test]
    public async Task ExportBookingByHomeStayID_ReturnsNotFound_WhenNoBookingsExist()
    {
        // Arrange
        var homeStayId = Guid.NewGuid();

        // Setup repo trả về danh sách rỗng
        var emptyList = new List<BusinessObject.Entities.Calendar>().AsQueryable().BuildMock();
        _mockCalendarRepo
            .Setup(repo => repo.FindWithInclude(It.IsAny<Expression<Func<BusinessObject.Entities.Calendar, object>>[]>()))
            .Returns(emptyList);

        // Act
        var result = await _controller.ExportBookingByHomeStayID(homeStayId);

        // Assert
        var notFoundResult = result as NotFoundObjectResult;
        Assert.IsNotNull(notFoundResult);
        Assert.AreEqual(404, notFoundResult.StatusCode);
    }

    [Test]
    public async Task ExportBookingByHomeStayID_ReturnsServerError_WhenExceptionThrown()
    {

        var homeStayId = Guid.NewGuid();

        _mockCalendarRepo
            .Setup(repo => repo.FindWithInclude(It.IsAny<Expression<Func<BusinessObject.Entities.Calendar, object>>[]>()))
            .Throws(new Exception("Unexpected error"));

        try
        {
            var result = await _controller.ExportBookingByHomeStayID(homeStayId);

            var statusResult = result as ObjectResult;
            Assert.IsNotNull(statusResult);
            Assert.AreEqual(500, statusResult.StatusCode);
            Assert.IsTrue(statusResult.Value.ToString().Contains("Unexpected error"));
        }
        catch (Exception ex)
        {
            Assert.AreEqual("Unexpected error", ex.Message);
        }
    }

    [Test]
    public async Task AnalyzeRevenueSystemHomeStay_ReturnsGroupedRevenueByHomeStay()
    {
        // Arrange
        var homeStayId1 = Guid.NewGuid();
        var homeStayId2 = Guid.NewGuid();

        var bookings = new List<Booking>
    {
        new Booking
        {
            Id = Guid.NewGuid(),
            TotalPrice = 1000000,
            Calendars = new List<BusinessObject.Entities.Calendar>
            {
                new BusinessObject.Entities.Calendar
                {
                    HomeStay = new HomeStay
                    {
                        Id = homeStayId1,
                        Name = "HomeStay 1",
                        MainImage = "image1.jpg",
                        Address = "Address 1"
                    }
                }
            }
        },
        new Booking
        {
            Id = Guid.NewGuid(),
            TotalPrice = 2000000,
            Calendars = new List<BusinessObject.Entities.Calendar>
            {
                new BusinessObject.Entities.Calendar
                {
                    HomeStay = new HomeStay
                    {
                        Id = homeStayId1,
                        Name = "HomeStay 1",
                        MainImage = "image1.jpg",
                        Address = "Address 1"
                    }
                },
                new BusinessObject.Entities.Calendar
                {
                    HomeStay = new HomeStay
                    {
                        Id = homeStayId2,
                        Name = "HomeStay 2",
                        MainImage = "image2.jpg",
                        Address = "Address 2"
                    }
                }
            }
        }
    };

        var mockBookingQueryable = bookings.AsQueryable().BuildMock();

        _mockBookingRepo
            .Setup(repo => repo.FindWithInclude(It.IsAny<Expression<Func<Booking, object>>[]>()))
            .Returns(mockBookingQueryable);

        // Act
        var result = await _controller.AnalyzeRevenueSystemHomeStay();

        // Assert
        var okResult = result as OkObjectResult;
        Assert.IsNotNull(okResult);

        var list = ((IEnumerable<object>)okResult.Value)
            .Select(x => JsonConvert.DeserializeObject<HomeStayRevenueDTO>(JsonConvert.SerializeObject(x)))
            .ToList();

        Assert.AreEqual(2, list.Count);

        var hs1 = list.FirstOrDefault(x => x.HomeStayID == homeStayId1);
        var hs2 = list.FirstOrDefault(x => x.HomeStayID == homeStayId2);

        Assert.IsNotNull(hs1);
        Assert.AreEqual(3000000, hs1.TotalRevenue);

        Assert.IsNotNull(hs2);
        Assert.AreEqual(2000000, hs2.TotalRevenue);
    }

    [Test]
    public async Task GetBookingByHomeStay_ReturnsBookings_ForValidHomeStayID()
    {
        // Arrange
        var homeStayId = Guid.NewGuid();

        var bookingList = new List<Booking>
    {
        new Booking
        {
            Id = Guid.NewGuid(),
            CheckInDate = DateTime.Today,
            CheckOutDate = DateTime.Today.AddDays(2),
            TotalPrice = 2000000,
            UnitPrice = 1000000,
            Status = "Confirmed",
            ReasonCancel = null,
            isDeleted = false,
            User = new User
            {
                FullName = "Nguyen Van A",
                Email = "a@example.com",
                Phone = "0123456789",
                Address = "Hanoi",
                Avatar = "avatar.jpg",
                Gender = "Male",
                CitizenID = 123456789
            },
            Calendars = new List<BusinessObject.Entities.Calendar>
            {
                new BusinessObject.Entities.Calendar { HomeStayID = homeStayId, HomeStay = new HomeStay { Name = "Test Homestay" } }
            }
        }
    };

        var mockBookingQueryable = bookingList.AsQueryable().BuildMock();

        _mockBookingRepo
            .Setup(repo => repo.FindWithInclude(
                It.IsAny<Expression<Func<Booking, object>>[]>()
            )).Returns(mockBookingQueryable);

        // Act
        var result = await _controller.GetBookingByHomeStay(homeStayId);

        // Assert
        var okResult = result as OkObjectResult;
        Assert.IsNotNull(okResult);
        var data = okResult.Value as IEnumerable<dynamic>;
        Assert.IsNotNull(data);
    }

    [Test]
    public async Task GetBookingByHomeStay_ReturnsNotFound_WhenNoBookingMatchHomeStayID()
    {
        var homeStayId = Guid.NewGuid();

        var bookings = new List<Booking>();

        var mockBookingQueryable = bookings.AsQueryable().BuildMock();

        _mockBookingRepo
            .Setup(repo => repo.FindWithInclude(It.IsAny<Expression<Func<Booking, object>>[]>()))
            .Returns(mockBookingQueryable);


        var result = await _controller.GetBookingByHomeStay(homeStayId);


        Assert.IsInstanceOf<NotFoundResult>(result);
    }

    [Test]
    public async Task GetBookingByHomeStay_ReturnsServerError_WhenExceptionOccurs()
    {
        // Arrange
        var homeStayId = Guid.NewGuid();

        _mockBookingRepo
            .Setup(repo => repo.FindWithInclude(It.IsAny<Expression<Func<Booking, object>>[]>()))
            .Throws(new Exception("Database connection failed"));

        // Act
        var result = await _controller.GetBookingByHomeStay(homeStayId);

        // Assert
        var statusResult = result as ObjectResult;
        Assert.IsNotNull(statusResult);
        Assert.AreEqual(500, statusResult.StatusCode);
        Assert.IsTrue(statusResult.Value.ToString().Contains("Database connection failed"));
    }
}
