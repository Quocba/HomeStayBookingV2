using System.Linq.Expressions;
using API.Controllers;
using BusinessObject.DTO;
using BusinessObject.Entities;
using BusinessObject.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MockQueryable;
using Moq;
using MockQueryable.Moq;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using APITesting.DTO;
#pragma warning disable

namespace APITesting;

public class HomeStayControllerTesting
{
    private Mock<IRepository<HomeStay>> _mockHomeStayRepo;
    private Mock<IRepository<User>> _mockUserRepo;
    private Mock<IRepository<HomeStayImage>> _mockHomeStayImageRepo;
    private Mock<IRepository<Calendar>> _mockCalendarRepo;
    private Mock<IWebHostEnvironment> _mockEnv;
    private Mock<IRepository<Amenity>> _mockAmenityRepo;
    private Mock<IRepository<HomestayAmenity>> _mockHomeStayAmenityRepo;
    private Mock<IRepository<HomeStayFacility>> _mockHomeStayFacilityRepo;
    private Mock<IRepository<Facility>> _mockFacilityRepo;
    private HomeStayController _controller;

    [SetUp]
    public void Setup()
    {
        _mockHomeStayRepo = new Mock<IRepository<HomeStay>>();
        _mockUserRepo = new Mock<IRepository<User>>();
        _mockHomeStayImageRepo = new Mock<IRepository<HomeStayImage>>();
        _mockCalendarRepo = new Mock<IRepository<Calendar>>();
        _mockEnv = new Mock<IWebHostEnvironment>();
        _mockAmenityRepo = new Mock<IRepository<Amenity>>();
        _mockHomeStayAmenityRepo = new Mock<IRepository<HomestayAmenity>>();
        _mockHomeStayFacilityRepo = new Mock<IRepository<HomeStayFacility>>();
        _mockFacilityRepo = new Mock<IRepository<Facility>>();

        _controller = new HomeStayController(
            _mockHomeStayRepo.Object,
            _mockUserRepo.Object,
            _mockHomeStayImageRepo.Object,
            _mockCalendarRepo.Object,
            _mockEnv.Object,
            _mockAmenityRepo.Object,
            _mockHomeStayAmenityRepo.Object,
            new HttpClient(),
            _mockHomeStayFacilityRepo.Object,
            _mockFacilityRepo.Object
        );
    }

    [Test]
    public async Task AddHomeStayFacility_ReturnsBadRequest_WhenRequestIsNull()
    {
        // Act
        var result = await _controller.AddHomeStayFacility(null);

        // Assert
        Assert.IsInstanceOf<BadRequestResult>(result);
    }

    [Test]
    public async Task AddHomeStayFacility_ReturnsOk_WhenRequestIsValid()
    {
        // Arrange
        var request = new AddHomeStayFacilityDTO
        {
            FacilityID = Guid.NewGuid(),
            HomeStayID = Guid.NewGuid()
        };

        _mockHomeStayFacilityRepo.Setup(r => r.AddAsync(It.IsAny<HomeStayFacility>()))
                                 .Returns(Task.CompletedTask);

        _mockHomeStayFacilityRepo.Setup(r => r.SaveAsync())
                                 .Returns(Task.CompletedTask);

        // Act
        var result = await _controller.AddHomeStayFacility(request);

        // Assert
        Assert.IsInstanceOf<OkObjectResult>(result);
    }

    [Test]
    public async Task AddHomeStayFacility_ReturnsInternalServerError_WhenExceptionThrown()
    {
        // Arrange
        var request = new AddHomeStayFacilityDTO
        {
            FacilityID = Guid.NewGuid(),
            HomeStayID = Guid.NewGuid()
        };

        _mockHomeStayFacilityRepo.Setup(r => r.AddAsync(It.IsAny<HomeStayFacility>()))
                                 .ThrowsAsync(new Exception("Something went wrong"));

        // Act
        var result = await _controller.AddHomeStayFacility(request);

        // Assert
        Assert.IsInstanceOf<ObjectResult>(result);
        var objectResult = result as ObjectResult;
        Assert.AreEqual(500, objectResult.StatusCode);
    }

    [Test]
    public async Task DeleteHomeStayFacility_ReturnsBadRequest_WhenIdIsEmpty()
    {
        // Act
        var result = await _controller.DeleteHomeStayFacility(Guid.Empty, Guid.NewGuid());

        // Assert
        Assert.IsInstanceOf<BadRequestResult>(result);
    }

    [Test]
    public async Task DeleteHomeStayFacility_ReturnsNotFound_WhenFacilityNotExist()
    {
        // Arrange
        var homeStayId = Guid.NewGuid();
        var facilityId = Guid.NewGuid();

        var emptyList = new List<HomeStayFacility>().AsQueryable().BuildMock();

        _mockHomeStayFacilityRepo
            .Setup(r => r.Find(It.IsAny<Expression<Func<HomeStayFacility, bool>>>()))
            .Returns(emptyList);

        // Act
        var result = await _controller.DeleteHomeStayFacility(homeStayId, facilityId);

        // Assert
        Assert.IsInstanceOf<NotFoundResult>(result); 
    }

    [Test]
    public async Task DeleteHomeStayFacility_ReturnsOk_WhenDeletedSuccessfully()
    {
        // Arrange
        var homeStayId = Guid.NewGuid();
        var facilityId = Guid.NewGuid();

        var facility = new HomeStayFacility
        {
            HomeStayID = homeStayId,
            FacilityID = facilityId
        };

        var facilityList = new List<HomeStayFacility> { facility }
            .AsQueryable()
            .BuildMock();

        _mockHomeStayFacilityRepo
            .Setup(r => r.Find(It.IsAny<Expression<Func<HomeStayFacility, bool>>>()))
            .Returns(facilityList);

        _mockHomeStayFacilityRepo
            .Setup(r => r.DeleteAsync(It.IsAny<HomeStayFacility>()))
            .Returns(Task.CompletedTask);

        _mockHomeStayFacilityRepo
            .Setup(r => r.SaveAsync())
            .Returns(Task.CompletedTask);

        // Act
        var result = await _controller.DeleteHomeStayFacility(homeStayId, facilityId);

        // Assert
        var okResult = result as ObjectResult;
        Assert.IsNotNull(okResult);
        Assert.AreEqual(200, okResult.StatusCode);
        Assert.That(okResult.Value.ToString(), Does.Contain("Delete Amenity Success"));
    }

    [Test]
    public async Task DeleteHomeStayFacility_ReturnsInternalServerError_WhenExceptionThrown()
    {
        // Arrange
        var homeStayId = Guid.NewGuid();
        var facilityId = Guid.NewGuid();

        _mockHomeStayFacilityRepo.Setup(r => r.Find(It.IsAny<Expression<Func<HomeStayFacility, bool>>>()))
            .Throws(new Exception("DB error"));

        // Act
        var result = await _controller.DeleteHomeStayFacility(homeStayId, facilityId);

        // Assert
        Assert.IsInstanceOf<ObjectResult>(result);
        var objectResult = result as ObjectResult;
        Assert.AreEqual(500, objectResult.StatusCode);
    }

    [Test]
    public async Task AddHomeStay_ReturnsOk_WhenValidRequest()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var request = new AddHomeStayRequest
        {
            MainImage = "main.jpg",
            Name = "Test HomeStay",
            Description = "A beautiful place",
            Address = "123 Street",
            CheckInTime = new TimeSpan(14, 0, 0).ToString(),
            CheckOutTime = new TimeSpan(12, 0, 0).ToString(),
            IsDeleted = false,
            City = "Hanoi",
            OpenIn = 2025,
            Standar = 5,
            Images = new List<string> { "img1.jpg", "img2.jpg" }
        };

        // Mock add & save methods
        _mockHomeStayRepo.Setup(r => r.AddAsync(It.IsAny<HomeStay>())).Returns(Task.CompletedTask);
        _mockHomeStayRepo.Setup(r => r.SaveAsync()).Returns(Task.CompletedTask);
        _mockHomeStayImageRepo.Setup(r => r.AddAsync(It.IsAny<HomeStayImage>())).Returns(Task.CompletedTask);

        // Act
        var result = await _controller.AddHomeStay(userId, request);

        // Assert
        var okResult = result as ObjectResult;
        Assert.IsNotNull(okResult);
        Assert.AreEqual(200, okResult.StatusCode);
        Assert.That(okResult.Value.ToString(), Does.Contain("Add Home Stay Success"));

        // Verify AddAsync được gọi
        _mockHomeStayRepo.Verify(r => r.AddAsync(It.IsAny<HomeStay>()), Times.Once);
        _mockHomeStayImageRepo.Verify(r => r.AddAsync(It.IsAny<HomeStayImage>()), Times.Exactly(2)); // vì có 2 ảnh
        _mockHomeStayRepo.Verify(r => r.SaveAsync(), Times.Once);
    }

    [Test]
    public async Task AddHomeStay_ReturnsBadRequest_WhenRequestIsNull()
    {
        // Arrange
        var userId = Guid.NewGuid();

        // Act
        var result = await _controller.AddHomeStay(userId, null);

        // Assert
        Assert.IsInstanceOf<BadRequestResult>(result);
    }

    [Test]
    public async Task AddHomeStay_ReturnsInternalServerError_WhenExceptionThrown()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var request = new AddHomeStayRequest
        {
            MainImage = "main.jpg",
            Name = "Test HomeStay",
            Description = "Nice place",
            Address = "123 Test St",
            CheckInTime = new TimeSpan(14, 0, 0).ToString(),
            CheckOutTime = new TimeSpan(12, 0, 0).ToString(),
            IsDeleted = false,
            City = "Hanoi",
            OpenIn = 2025,
            Standar = 4,
            Images = new List<string> { "img1.jpg" }
        };

        _mockHomeStayRepo.Setup(r => r.AddAsync(It.IsAny<HomeStay>()))
                         .ThrowsAsync(new Exception("Something went wrong"));

        // Act
        var result = await _controller.AddHomeStay(userId, request);

        // Assert
        var objectResult = result as ObjectResult;
        Assert.IsNotNull(objectResult);
        Assert.AreEqual(500, objectResult.StatusCode);
        Assert.That(objectResult.Value.ToString(), Does.Contain("Something went wrong"));
    }


    [Test]
    public async Task AddHomeStayAmenity_ReturnsBadRequest_WhenRequestIsNull()
    {
        // Act
        var result = await _controller.AddHomeStayAmennity(null);

        // Assert
        Assert.IsInstanceOf<BadRequestResult>(result);
    }


    [Test]
    public async Task AddHomeStayAmenity_ReturnsNotFound_WhenHomeStayDoesNotExist()
    {
        var request = new AddAmenityDTO
        {
            HomeStayID = Guid.NewGuid(),
            AmenityName = new List<string> { "Wifi" }
        };

        _mockHomeStayRepo.Setup(x => x.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((HomeStay)null);

        var result = await _controller.AddHomeStayAmennity(request);

        Assert.IsInstanceOf<NotFoundResult>(result);
    }

    [Test]
    public async Task AddHomeStayAmenity_ReturnsConflict_WhenAmenityAlreadyExists()
    {
        var homestayId = Guid.NewGuid();
        var amenityId = Guid.NewGuid();

        var request = new AddAmenityDTO
        {
            HomeStayID = homestayId,
            AmenityName = new List<string> { "Wifi" }
        };

        var homestay = new HomeStay { Id = homestayId };
        var amenity = new Amenity { Id = amenityId, Name = "Wifi" };
        var existing = new HomestayAmenity { HomeStayID = homestayId, AmenityId = amenityId };

        _mockHomeStayRepo.Setup(x => x.GetByIdAsync(homestayId)).ReturnsAsync(homestay);
        _mockAmenityRepo.Setup(x => x.Find(It.IsAny<Expression<Func<Amenity, bool>>>()))
                        .Returns(new List<Amenity> { amenity }.AsQueryable().BuildMock());
        _mockHomeStayAmenityRepo.Setup(x => x.Find(It.IsAny<Expression<Func<HomestayAmenity, bool>>>()))
                                 .Returns(new List<HomestayAmenity> { existing }.AsQueryable().BuildMock());

        var result = await _controller.AddHomeStayAmennity(request);

        Assert.IsInstanceOf<ConflictResult>(result);
    }


    [Test]
    public async Task AddHomeStayAmenity_ReturnsOk_WhenSuccess()
    {
        var homestayId = Guid.NewGuid();
        var amenityId = Guid.NewGuid();

        var request = new AddAmenityDTO
        {
            HomeStayID = homestayId,
            AmenityName = new List<string> { "Wifi" }
        };

        var homestay = new HomeStay { Id = homestayId };
        var amenity = new Amenity { Id = amenityId, Name = "Wifi" };

        _mockHomeStayRepo.Setup(x => x.GetByIdAsync(homestayId)).ReturnsAsync(homestay);
        _mockAmenityRepo.Setup(x => x.Find(It.IsAny<Expression<Func<Amenity, bool>>>()))
                        .Returns(new List<Amenity> { amenity }.AsQueryable().BuildMock());
        _mockHomeStayAmenityRepo.Setup(x => x.Find(It.IsAny<Expression<Func<HomestayAmenity, bool>>>()))
                                 .Returns(new List<HomestayAmenity>().AsQueryable().BuildMock());
        _mockHomeStayAmenityRepo.Setup(x => x.AddAsync(It.IsAny<HomestayAmenity>())).Returns(Task.CompletedTask);
        _mockHomeStayAmenityRepo.Setup(x => x.SaveAsync()).Returns(Task.CompletedTask);

        var result = await _controller.AddHomeStayAmennity(request);

        var okResult = result as OkObjectResult;
        Assert.IsNotNull(okResult);
        Assert.That(okResult.StatusCode, Is.EqualTo(200));
        Assert.That(okResult.Value.ToString(), Does.Contain("Add Amentity Success"));
    }

    [Test]
    public async Task AddHomeStayAmenity_ReturnsInternalServerError_WhenExceptionThrown()
    {
        var request = new AddAmenityDTO
        {
            HomeStayID = Guid.NewGuid(),
            AmenityName = new List<string> { "Wifi" }
        };

        _mockHomeStayRepo.Setup(x => x.GetByIdAsync(It.IsAny<Guid>())).ThrowsAsync(new Exception("Database error"));

        var result = await _controller.AddHomeStayAmennity(request);

        var objectResult = result as ObjectResult;
        Assert.IsNotNull(objectResult);
        Assert.AreEqual(500, objectResult.StatusCode);
        Assert.That(objectResult.Value.ToString(), Does.Contain("An error occurred"));
    }


    [Test]
    public async Task EditHomeStay_ReturnsBadRequest_WhenRequestIsNull()
    {
        var result = await _controller.EditHomeStay(null);

        Assert.IsInstanceOf<BadRequestResult>(result);
    }

    [Test]
    public async Task EditHomeStay_ReturnsNotFound_WhenHomeStayDoesNotExist()
    {
        var request = new EditHomeStayInforRequest
        {
            HomeStayID = Guid.NewGuid()
        };

        _mockHomeStayRepo.Setup(x => x.GetByIdAsync(It.IsAny<Guid>()))
                         .ReturnsAsync((HomeStay)null);

        var result = await _controller.EditHomeStay(request);

        Assert.IsInstanceOf<NotFoundResult>(result);
    }

    [Test]
    public async Task EditHomeStay_ReturnsOk_WhenUpdateSuccessful()
    {
        var homeStayId = Guid.NewGuid();
        var existingHomeStay = new HomeStay
        {
            Id = homeStayId,
            Name = "Old Name",
            MainImage = "old.jpg",
            Standar = 2,
            CheckInTime = new TimeSpan(12, 0, 0).ToString(),
            CheckOutTime = new TimeSpan(10, 0, 0).ToString(),
            Address = "Old Address",
            OpenIn = 2015,
            Description = "Old Desc"
        };

        var request = new EditHomeStayInforRequest
        {
            HomeStayID = homeStayId,
            Name = "New Name",
            MainImage = "new.jpg",
            Standar = 4,
            CheckInTime = new TimeSpan(13, 0, 0).ToString(),
            CheckOutTime = new TimeSpan(11, 0, 0).ToString(),
            Address = "New Address",
            OpenIn = 2020,
            Description = "New Desc"
        };

        _mockHomeStayRepo.Setup(x => x.GetByIdAsync(homeStayId)).ReturnsAsync(existingHomeStay);
        _mockHomeStayRepo.Setup(x => x.UpdateAsync(It.IsAny<HomeStay>())).Returns(Task.CompletedTask);
        _mockHomeStayRepo.Setup(x => x.SaveAsync()).Returns(Task.CompletedTask);

        var result = await _controller.EditHomeStay(request);

        var okResult = result as OkObjectResult;
        Assert.IsNotNull(okResult);
        Assert.AreEqual(200, okResult.StatusCode);
        Assert.That(okResult.Value.ToString(), Does.Contain("Update Home Stay Success"));
    }

    [Test]
    public async Task EditHomeStay_SkipNullFields_KeepsOriginalValues()
    {
        var homeStayId = Guid.NewGuid();
        var existing = new HomeStay
        {
            Id = homeStayId,
            Name = "Stay A",
            Standar = 3,
            OpenIn = 2021
        };

        var request = new EditHomeStayInforRequest
        {
            HomeStayID = homeStayId,
            Name = null,                 
            MainImage = null,           
            Standar = 0,                
            OpenIn = 0                  
        };

        _mockHomeStayRepo.Setup(x => x.GetByIdAsync(homeStayId)).ReturnsAsync(existing);
        _mockHomeStayRepo.Setup(x => x.UpdateAsync(It.IsAny<HomeStay>())).Returns(Task.CompletedTask);
        _mockHomeStayRepo.Setup(x => x.SaveAsync()).Returns(Task.CompletedTask);

        var result = await _controller.EditHomeStay(request);

        var okResult = result as OkObjectResult;
        Assert.IsNotNull(okResult);
        Assert.AreEqual(200, okResult.StatusCode);
    }

    [Test]
    public async Task EditHomeStay_ReturnsInternalServerError_WhenExceptionThrown()
    {
        var request = new EditHomeStayInforRequest
        {
            HomeStayID = Guid.NewGuid()
        };

        _mockHomeStayRepo.Setup(x => x.GetByIdAsync(It.IsAny<Guid>())).ThrowsAsync(new Exception("Some error"));

        var result = await _controller.EditHomeStay(request);

        var objectResult = result as ObjectResult;
        Assert.IsNotNull(objectResult);
        Assert.AreEqual(500, objectResult.StatusCode);
        Assert.That(objectResult.Value.ToString(), Does.Contain("An error occurred"));
    }

    [Test]
    public async Task DeleteHomeStay_ReturnsBadRequest_WhenIdIsEmpty()
    {
        var result = await _controller.DeleteHomeStay(Guid.Empty);

        Assert.IsInstanceOf<BadRequestResult>(result);
    }

    [Test]
    public async Task DeleteHomeStay_ReturnsNotFound_WhenHomeStayNotFound()
    {
        var homeStayId = Guid.NewGuid();

        _mockHomeStayRepo.Setup(x => x.GetByIdAsync(homeStayId)).ReturnsAsync((HomeStay)null);

        var result = await _controller.DeleteHomeStay(homeStayId);

        Assert.IsInstanceOf<NotFoundResult>(result);
    }

    [Test]
    public async Task DeleteHomeStay_ReturnsOk_WhenHomeStaySoftDeleted()
    {
        var homeStayId = Guid.NewGuid();
        var homeStay = new HomeStay { Id = homeStayId, isDeleted = false };

        _mockHomeStayRepo.Setup(x => x.GetByIdAsync(homeStayId)).ReturnsAsync(homeStay);
        _mockHomeStayRepo.Setup(x => x.UpdateAsync(homeStay)).Returns(Task.CompletedTask);
        _mockHomeStayRepo.Setup(x => x.SaveAsync()).Returns(Task.CompletedTask);

        var result = await _controller.DeleteHomeStay(homeStayId);

        var okResult = result as OkObjectResult;
        Assert.IsNotNull(okResult);
        Assert.AreEqual(200, okResult.StatusCode);
        Assert.That(okResult.Value.ToString(), Does.Contain("Delete Success"));
        Assert.IsTrue(homeStay.isDeleted);
    }

    [Test]
    public async Task DeleteHomeStay_ReturnsOk_WhenHomeStayRestored()
    {
        var homeStayId = Guid.NewGuid();
        var homeStay = new HomeStay { Id = homeStayId, isDeleted = true };

        _mockHomeStayRepo.Setup(x => x.GetByIdAsync(homeStayId)).ReturnsAsync(homeStay);
        _mockHomeStayRepo.Setup(x => x.UpdateAsync(homeStay)).Returns(Task.CompletedTask);
        _mockHomeStayRepo.Setup(x => x.SaveAsync()).Returns(Task.CompletedTask);

        var result = await _controller.DeleteHomeStay(homeStayId);

        var okResult = result as OkObjectResult;
        Assert.IsNotNull(okResult);
        Assert.AreEqual(200, okResult.StatusCode);
        Assert.That(okResult.Value.ToString(), Does.Contain("Restore Home Stay Success"));
        Assert.IsFalse(homeStay.isDeleted);
    }

    [Test]
    public async Task DeleteHomeStay_ReturnsInternalServerError_WhenExceptionThrown()
    {
        var homeStayId = Guid.NewGuid();

        _mockHomeStayRepo.Setup(x => x.GetByIdAsync(homeStayId))
                         .ThrowsAsync(new Exception("DB connection error"));

        var result = await _controller.DeleteHomeStay(homeStayId);

        var objectResult = result as ObjectResult;
        Assert.IsNotNull(objectResult);
        Assert.AreEqual(500, objectResult.StatusCode);
        Assert.That(objectResult.Value.ToString(), Does.Contain("DB connection error"));
    }

    [Test]
    public async Task DeleteHomeStayAmenity_ReturnsBadRequest_WhenGuidIsEmpty()
    {
        var result = await _controller.DeleteHomeStayAmenity(Guid.Empty, Guid.NewGuid());

        Assert.IsInstanceOf<BadRequestResult>(result);
    }


    [Test]
    public async Task DeleteHomeStayAmenity_ReturnsOk_WhenDeleteSuccessful()
    {
        // Arrange
        var homeStayId = Guid.NewGuid();
        var amenityId = Guid.NewGuid();

        var homestayAmenity = new HomestayAmenity
        {
            HomeStayID = homeStayId,
            AmenityId = amenityId,
            isDeleted = false
        };

        var amenityList = new List<HomestayAmenity> { homestayAmenity }.AsQueryable();
        var mockAmenityList = amenityList.BuildMock(); // Mock<IQueryable<HomestayAmenity>>

        _mockHomeStayAmenityRepo.Setup(r =>
                r.Find(It.IsAny<Expression<Func<HomestayAmenity, bool>>>()))
            .Returns(mockAmenityList);

        _mockHomeStayAmenityRepo.Setup(r => r.DeleteAsync(It.IsAny<HomestayAmenity>()))
            .Returns(Task.CompletedTask);

        _mockHomeStayAmenityRepo.Setup(r => r.SaveAsync())
            .Returns(Task.CompletedTask);

        // Act
        var result = await _controller.DeleteHomeStayAmenity(homeStayId, amenityId);

        // Assert
        var objectResult = result as OkObjectResult;
        Assert.IsNotNull(objectResult);
        Assert.AreEqual(200, objectResult.StatusCode);
    }

    [Test]
        public async Task DeleteHomeStayAmenity_ThrowsException_WhenDeleteFails()
        {
            var mockAmenity = new HomestayAmenity
            {
                HomeStayID = Guid.NewGuid(),
                AmenityId = Guid.NewGuid(),
                isDeleted = false
            };

            _mockHomeStayAmenityRepo
                .Setup(x => x.Find(It.IsAny<Expression<Func<HomestayAmenity, bool>>>()))
                .Returns(new List<HomestayAmenity> { mockAmenity }.AsQueryable());

            _mockHomeStayAmenityRepo
                .Setup(x => x.DeleteAsync(It.IsAny<HomestayAmenity>()))
                .ThrowsAsync(new Exception("Delete failed"));

            var result = await _controller.DeleteHomeStayAmenity(mockAmenity.HomeStayID, mockAmenity.AmenityId);

            var objectResult = result as ObjectResult;
            Assert.That(objectResult, Is.Not.Null);
            Assert.That(objectResult.StatusCode, Is.EqualTo(500));
        }

    [Test]
    public async Task DeleteHomeStayAmenity_Returns500_WhenExceptionOccurs()
    {
        // Arrange
        var homeStayId = Guid.NewGuid();
        var amenityId = Guid.NewGuid();

        // Setup để giả lập exception khi gọi Find
        _mockHomeStayAmenityRepo
            .Setup(r => r.Find(It.IsAny<Expression<Func<BusinessObject.Entities.HomestayAmenity, bool>>>()))
            .Throws(new Exception("Database connection failed"));

        // Act
        var result = await _controller.DeleteHomeStayAmenity(homeStayId, amenityId);

        // Assert
        var objectResult = result as ObjectResult;

        Assert.IsNotNull(objectResult);
        Assert.AreEqual(500, objectResult.StatusCode);
        Assert.That(objectResult.Value.ToString(), Does.Contain("Database connection failed"));
    }

        [Test]
        public async Task GetAllHomeStay_ReturnsOk_WhenDataExists()
        {
            // Arrange
            var homeStayId = Guid.NewGuid();
            var calendar = new Calendar { Id = Guid.NewGuid(), Price = 100, isBooked = false, Date = DateTime.Now };
            var amenity = new Amenity { Id = Guid.NewGuid(), Name = "Wifi" };
            var facility = new Facility { Id = Guid.NewGuid(), Name = "Pool", Description = "Nice" };

            var homeStay = new HomeStay
            {
                Id = homeStayId,
                Name = "Test Stay",
                MainImage = "img.jpg",
                Address = "123 Test",
                City = "Test City",
                CheckInTime = TimeSpan.FromHours(14).ToString(),
                CheckOutTime = TimeSpan.FromHours(12).ToString(),
                OpenIn = 2025,
                Description = "A nice stay",
                Standar = 5,
                isDeleted = false,
                Calendars = new List<Calendar> { calendar },
                HomestayAmenities = new List<HomestayAmenity>
                {
                    new HomestayAmenity { Amenity = amenity }
                },
                HomestayFacilities = new List<HomeStayFacility>
                {
                    new HomeStayFacility { Facility = facility }
                }
            };

            var mockData = new List<HomeStay> { homeStay }
                .AsQueryable()
                .BuildMock(); 

        _mockHomeStayRepo
            .Setup(r => r.FindWithInclude(It.IsAny<Expression<Func<HomeStay, object>>[]>()))
            .Returns(mockData);

        var request = new FilterDTO
            {
                Standard = new List<int> { 5 },
                AmenityNames = new List<string> { "Wifi" },
                MinPrice = 50,
                MaxPrice = 150
            };

            // Act
            var result = await _controller.GetAllHomeStay(request);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
        }

    [Test]
    public async Task GetAllHomeStay_ReturnsNotFound_WhenNoData()
    {
        // Arrange
        var mockData = new List<HomeStay>()
            .AsQueryable()
            .BuildMock();

        _mockHomeStayRepo.Setup(r => r.FindWithInclude(It.IsAny<Expression<Func<HomeStay, object>>[]>()))
                         .Returns(mockData); 

        var request = new FilterDTO();

        // Act
        var result = await _controller.GetAllHomeStay(request);

        // Assert
        Assert.IsInstanceOf<NotFoundResult>(result);
    }


    [Test]
    public async Task GetHomeStayDetail_ReturnsOk_WithFullDetail()
    {
        // Arrange
        var homeStayId = Guid.NewGuid();

        // Mock dữ liệu Calendar
        var mockCalendars = new List<Calendar>
    {
        new Calendar
        {
            Id = Guid.NewGuid(),
            HomeStay = new HomeStay { Id = homeStayId },
            Date = DateTime.Now,
            Price = 500,
            isBooked = false,
            isDeleted = false
        }
    }.AsQueryable().BuildMock();

        _mockCalendarRepo.Setup(r => r.FindWithInclude(It.IsAny<Expression<Func<Calendar, object>>[]>()))
                         .Returns(mockCalendars);

        // Mock dữ liệu HomeStay
        var mockHomeStays = new List<HomeStay>
    {
        new HomeStay
        {
            Id = homeStayId,
            Name = "Test HomeStay",
            MainImage = "image.jpg",
            Address = "123 Street",
            City = "Hanoi",
            CheckInTime = TimeSpan.FromHours(12).ToString(),
            CheckOutTime = TimeSpan.FromHours(11).ToString(),
            OpenIn = 2020,
            Description = "Nice place",
            Standar = 4,
            isDeleted = false,
            Calendars = new List<Calendar>(), // Not used in this test
            HomestayAmenities = new List<HomestayAmenity>
            {
                new HomestayAmenity
                {
                    Amenity = new Amenity { Id = Guid.NewGuid(), Name = "Wifi" }
                }
            },
            HomestayImages = new List<HomeStayImage>
            {
                new HomeStayImage { Image = "img1.jpg" }
            },
            HomestayFacilities = new List<HomeStayFacility>
            {
                new HomeStayFacility
                {
                    FacilityID = Guid.NewGuid(),
                    Facility = new Facility { Name = "Pool", Description = "Swimming pool" }
                }
            },
            FeedBacks = new List<FeedBack>
            {
                new FeedBack
                {
                    User = new User { FullName = "John Doe", Avatar = "avatar.jpg", Email = "john@example.com" },
                    Rating = 5,
                    Description = "Great!",
                    IsReply = false
                }
            }
        }
    }.AsQueryable().BuildMock();

        _mockHomeStayRepo.Setup(r => r.FindWithInclude(It.IsAny<Expression<Func<HomeStay, object>>[]>()))
                         .Returns(mockHomeStays);

        // Act
        var result = await _controller.GetHomeStayDetail(homeStayId);

        // Assert
        var okResult = result as OkObjectResult;
        Assert.IsNotNull(okResult);
        Assert.AreEqual(200, okResult.StatusCode);

        var json = JObject.FromObject(okResult.Value!);

        Assert.AreEqual("Test HomeStay", (string)json["Name"]);
        Assert.AreEqual("123 Street", (string)json["Address"]);
    }

    [Test]
    public async Task GetHomeStayDetail_ReturnsNotFound_WhenHomeStayDoesNotExist()
    {
        // Arrange
        var homeStayId = Guid.NewGuid();

        // Mock cho FindWithInclude trả về null (không có dữ liệu)
        var emptyData = new List<HomeStay>().AsQueryable().BuildMock();

        _mockHomeStayRepo.Setup(r => r.FindWithInclude(It.IsAny<Expression<Func<HomeStay, object>>[]>()))
                         .Returns(emptyData);

        // Act
        var result = await _controller.GetHomeStayDetail(homeStayId);

        // Assert
        Assert.IsInstanceOf<NotFoundResult>(result);
    }


    [Test]
    public async Task FilterHomeStayWithStatus_ReturnsOk_WhenDataExists()
    {
        // Arrange
        var mockData = new List<HomeStay>
    {
        new HomeStay { Id = Guid.NewGuid(), Name = "Villa 1", isDeleted = false },
        new HomeStay { Id = Guid.NewGuid(), Name = "Villa 2", isDeleted = false }
    }.AsQueryable().BuildMock();

        _mockHomeStayRepo.Setup(r => r.Find(It.IsAny<Expression<Func<HomeStay, bool>>>()))
                         .Returns(mockData);

        // Act
        var result = await _controller.FilterHomeStayWithStatus(false);

        // Assert
        var okResult = result as OkObjectResult;
        Assert.IsNotNull(okResult);
        Assert.AreEqual(200, okResult.StatusCode);

        var data = okResult.Value as IEnumerable<HomeStay>;
        Assert.IsNotNull(data);
        Assert.AreEqual(2, data.Count());
    }

    [Test]
    public async Task FilterHomeStayWithStatus_ReturnsNotFound_WhenNoData()
    {
        // Arrange
        var emptyData = new List<HomeStay>().AsQueryable().BuildMock();

        _mockHomeStayRepo.Setup(r => r.Find(It.IsAny<Expression<Func<HomeStay, bool>>>()))
                         .Returns(emptyData);

        // Act
        var result = await _controller.FilterHomeStayWithStatus(true);

        // Assert
        Assert.IsInstanceOf<NotFoundResult>(result);
    }

    [Test]
    public async Task AddHomeStayImage_ReturnsOk_WhenValidRequest()
    {
        // Arrange
        var homeStayId = Guid.NewGuid();
        var request = new HomeStayImageDTO
        {
            HomeStayID = homeStayId,
            Images = new List<string> { "image1.jpg", "image2.jpg" }
        };

        var mockHomeStay = new HomeStay { Id = homeStayId, Name = "Mock Homestay" };

        _mockHomeStayRepo.Setup(r => r.GetByIdAsync(homeStayId)).ReturnsAsync(mockHomeStay);
        _mockHomeStayImageRepo.Setup(r => r.AddAsync(It.IsAny<HomeStayImage>())).Returns(Task.CompletedTask);
        _mockHomeStayImageRepo.Setup(r => r.SaveAsync()).Returns(Task.CompletedTask);

        // Act
        var result = await _controller.AddHomeStayImage(request);

        // Assert
        var okResult = result as OkObjectResult;
        Assert.IsNotNull(okResult);
        Assert.AreEqual(200, okResult.StatusCode);
    }

    [Test]
    public async Task AddHomeStayImage_ReturnsBadRequest_WhenRequestIsNull()
    {
        // Act
        var result = await _controller.AddHomeStayImage(null);

        // Assert
        Assert.IsInstanceOf<BadRequestResult>(result);
    }

    [Test]
    public async Task AddHomeStayImage_Returns500_WhenExceptionOccurs()
    {
        // Arrange
        var request = new HomeStayImageDTO
        {
            HomeStayID = Guid.NewGuid(),
            Images = new List<string> { "img.jpg" }
        };

        _mockHomeStayRepo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ThrowsAsync(new Exception("Test exception"));

        // Act
        var result = await _controller.AddHomeStayImage(request);

        // Assert
        var objectResult = result as ObjectResult;
        Assert.IsNotNull(objectResult);
        Assert.AreEqual(500, objectResult.StatusCode);
        StringAssert.Contains("Test exception", objectResult.Value!.ToString());
    }

    [Test]
    public async Task GetHomeStayImage_ReturnsOk_WhenImagesExist()
    {
        // Arrange
        var homeStayId = Guid.NewGuid();

        var mockImages = new List<HomeStayImage>
    {
        new HomeStayImage
        {
            Id = Guid.NewGuid(),
            Image = "image1.jpg",
            HomeStayID = homeStayId,
            HomeStay = new HomeStay { Id = homeStayId }
        },
        new HomeStayImage
        {
            Id = Guid.NewGuid(),
            Image = "image2.jpg",
            HomeStayID = homeStayId,
            HomeStay = new HomeStay { Id = homeStayId }
        }
    }.AsQueryable().BuildMock();

        _mockHomeStayImageRepo
            .Setup(r => r.FindWithInclude(It.IsAny<Expression<Func<HomeStayImage, object>>[]>()))
            .Returns(mockImages);

        // Act
        var result = await _controller.GetHomeStayImage(homeStayId);

        // Assert
        var okResult = result as OkObjectResult;
        Assert.IsNotNull(okResult);
        Assert.AreEqual(200, okResult.StatusCode);
    }

    [Test]
    public async Task GetHomeStayImage_ReturnsNotFound_WhenNoImagesExist()
    {
        // Arrange
        var homeStayId = Guid.NewGuid();

        var mockEmptyList = new List<HomeStayImage>().AsQueryable().BuildMock();

        _mockHomeStayImageRepo
            .Setup(r => r.FindWithInclude(It.IsAny<Expression<Func<HomeStayImage, object>>[]> ()))
            .Returns(mockEmptyList);

        // Act
        var result = await _controller.GetHomeStayImage(homeStayId);

        // Assert
        Assert.IsInstanceOf<NotFoundResult>(result);
    }

    [Test]
    public async Task GetHomeStayImage_Returns500_WhenExceptionThrown()
    {
        // Arrange
        var homeStayId = Guid.NewGuid();

        _mockHomeStayImageRepo
            .Setup(r => r.FindWithInclude(It.IsAny<Expression<Func<HomeStayImage, object>>[]> ()))
            .Throws(new Exception("DB error"));

        // Act
        var result = await _controller.GetHomeStayImage(homeStayId);

        // Assert
        var objectResult = result as ObjectResult;
        Assert.IsNotNull(objectResult);
        Assert.AreEqual(500, objectResult.StatusCode);
        StringAssert.Contains("DB error", objectResult.Value!.ToString());
    }

    [Test]
    public async Task DeleteHomeStayImage_ReturnsOk_WhenImagesDeletedSuccessfully()
    {
        // Arrange
        var imageId1 = Guid.NewGuid();
        var imageId2 = Guid.NewGuid();
        var request = new DeleteHomeStayImageDTO
        {
            ImageIds = new List<Guid> { imageId1, imageId2 }
        };

        var mockImages = new List<HomeStayImage>
    {
        new HomeStayImage { Id = imageId1 },
        new HomeStayImage { Id = imageId2 }
    }.AsQueryable().BuildMock();

        _mockHomeStayImageRepo
            .Setup(r => r.Find(It.IsAny<Expression<Func<HomeStayImage, bool>>>()))
            .Returns(mockImages);

        _mockHomeStayImageRepo.Setup(r => r.DeleteRange(It.IsAny<IEnumerable<HomeStayImage>>()));
        _mockHomeStayImageRepo.Setup(r => r.SaveAsync()).Returns(Task.CompletedTask);
        // Act
        var result = await _controller.DeleteHomeStayImage(request);

        // Assert
        var okResult = result as OkObjectResult;
        Assert.IsNotNull(okResult);
        Assert.AreEqual(200, okResult.StatusCode);
    }

    [Test]
    public async Task DeleteHomeStayImage_ReturnsBadRequest_WhenRequestIsInvalid()
    {
        // Arrange
        var request = new DeleteHomeStayImageDTO
        {
            ImageIds = null
        };

        // Act
        var result = await _controller.DeleteHomeStayImage(request);

        // Assert
        var badRequest = result as BadRequestObjectResult;
        Assert.IsNotNull(badRequest);
        Assert.AreEqual(400, badRequest.StatusCode);
    }

    [Test]
    public async Task DeleteHomeStayImage_ReturnsNotFound_WhenNoImagesMatch()
    {
        // Arrange
        var request = new DeleteHomeStayImageDTO
        {
            ImageIds = new List<Guid> { Guid.NewGuid() }
        };

        var emptyImages = new List<HomeStayImage>().AsQueryable().BuildMock();

        _mockHomeStayImageRepo
            .Setup(r => r.Find(It.IsAny<Expression<Func<HomeStayImage, bool>>>()))
            .Returns(emptyImages);

        // Act
        var result = await _controller.DeleteHomeStayImage(request);

        // Assert
        var notFound = result as NotFoundObjectResult;
        Assert.IsNotNull(notFound);
        Assert.AreEqual(404, notFound.StatusCode);
    }

    [Test]
    public async Task DeleteHomeStayImage_Returns500_WhenExceptionThrown()
    {
        // Arrange
        var request = new DeleteHomeStayImageDTO
        {
            ImageIds = new List<Guid> { Guid.NewGuid() }
        };

        _mockHomeStayImageRepo
            .Setup(r => r.Find(It.IsAny<Expression<Func<HomeStayImage, bool>>>()))
            .Throws(new Exception("DB failure"));

        // Act
        var result = await _controller.DeleteHomeStayImage(request);

        // Assert
        var objectResult = result as ObjectResult;
        Assert.IsNotNull(objectResult);
        Assert.AreEqual(500, objectResult.StatusCode);
        StringAssert.Contains("DB failure", objectResult.Value!.ToString());
    }

    [Test]
    public async Task GetAllCity_ReturnsOk_WithDistinctCityList()
    {
        // Arrange
        var mockData = new List<HomeStay>
    {
        new HomeStay { City = "Hanoi" },
        new HomeStay { City = "Da Nang" },
        new HomeStay { City = "Hanoi" },
        new HomeStay { City = "Ho Chi Minh" }
    };

        _mockHomeStayRepo.Setup(repo => repo.GetAllAsync())
                         .ReturnsAsync(mockData);

        // Act
        var result = await _controller.GetAllCity();

        // Assert
        var okResult = result as OkObjectResult;
        Assert.IsNotNull(okResult);
        Assert.AreEqual(200, okResult.StatusCode);

        var cityList = okResult.Value as List<string>;
        Assert.IsNotNull(cityList);
        Assert.AreEqual(3, cityList.Count);
        CollectionAssert.AreEquivalent(new[] { "Hanoi", "Da Nang", "Ho Chi Minh" }, cityList);
    }

    [Test]
    public async Task SearchByCity_ReturnsOk_WithHomeStayList()
    {
        // Arrange
        var city = "Hanoi";
        var mockHomeStayId = Guid.NewGuid();

        var mockData = new List<HomeStay>
    {
        new HomeStay
        {
            Id = mockHomeStayId,
            Name = "Hanoi Homestay",
            MainImage = "img.jpg",
            Address = "123 Street",
            City = city,
            CheckInTime = "14:00",
            CheckOutTime = "12:00",
            OpenIn = 2022,
            Description = "Nice view",
            Standar = 5,
            isDeleted = false,
            Calendars = new List<Calendar>
            {
                new Calendar { Id = Guid.NewGuid(), Date = DateTime.Today, Price = 300, isBooked = false }
            },
            HomestayAmenities = new List<HomestayAmenity>
            {
                new HomestayAmenity
                {
                    Amenity = new Amenity { Id = Guid.NewGuid(), Name = "Wifi" }
                }
            },
            HomestayFacilities = new List<HomeStayFacility>
            {
                new HomeStayFacility
                {
                    FacilityID = Guid.NewGuid(),
                    Facility = new Facility { Name = "Pool", Description = "Swimming Pool" }
                }
            },
            FeedBacks = new List<FeedBack>()
        }
    }.AsQueryable().BuildMock();

        _mockHomeStayRepo.Setup(repo => repo.FindWithInclude(It.IsAny<Expression<Func<HomeStay, object>>[]>()))
                         .Returns(mockData);

        // Act
        var result = await _controller.SearchByCity(city);

        // Assert
        var okResult = result as OkObjectResult;
        Assert.IsNotNull(okResult);
        Assert.AreEqual(200, okResult.StatusCode);
    }

    [Test]
    public async Task SearchByCity_ReturnsOk_WithEmptyList()
    {
        // Arrange
        var city = "NonExistingCity";
        var emptyData = new List<HomeStay>().AsQueryable().BuildMock();

        _mockHomeStayRepo.Setup(repo => repo.FindWithInclude(It.IsAny<Expression<Func<HomeStay, object>>[]>()))
                         .Returns(emptyData);

        // Act
        var result = await _controller.SearchByCity(city);

        // Assert
        var okResult = result as OkObjectResult;
        Assert.IsNotNull(okResult);
        Assert.AreEqual(200, okResult.StatusCode);
    }

    [Test]
    public async Task GetHomeStayByUser_ReturnsOk_WithHomeStayList()
    {
        // Arrange
        var userId = Guid.NewGuid();

        var mockHomeStays = new List<HomeStay>
    {
        new HomeStay
        {
            Id = Guid.NewGuid(),
            UserID = userId,
            Name = "Home stay 2",
            MainImage = "main2.jpg",
            Address = "14 Bach Dang st\n15 Bach Dang st",
            City = "Ho Chi Minh",
            CheckInTime = "11:30",
            CheckOutTime = "11:30",
            OpenIn = 2025,
            Description = "Home stay 2 Des",
            Standar = 5,
            isDeleted = false,
            Calendars = new List<Calendar>(),
            HomestayAmenities = new List<HomestayAmenity>
            {
                new HomestayAmenity
                {
                    Amenity = new Amenity { Id = Guid.NewGuid(), Name = "Swimming Pool" }
                }
            },
            HomestayFacilities = new List<HomeStayFacility>
            {
                new HomeStayFacility
                {
                    FacilityID = Guid.NewGuid(),
                    Facility = new Facility
                    {
                        Name = "Wifi",
                        Description = "Wifi Des"
                    }
                }
            }
        }
    }.AsQueryable().BuildMock();

        _mockHomeStayRepo
            .Setup(repo => repo.FindWithInclude(It.IsAny<Expression<Func<HomeStay, object>>[]>()))
            .Returns(mockHomeStays);
        FilterDTO filter = new FilterDTO
        {
            AmenityNames = null,
            MaxPrice = null,
            MinPrice = null,
            SearchText = null,
            Standard = null
        };
        // Act
        var result = await _controller.GetHomeStayByUser(userId, filter);

        // Assert
        var okResult = result as OkObjectResult;
        Assert.IsNotNull(okResult);
        Assert.AreEqual(200, okResult.StatusCode);

        // Dùng JObject để truy xuất an toàn
        var json = JsonConvert.SerializeObject(okResult.Value);
        var homeStayList = JArray.Parse(json);

        Assert.AreEqual(1, homeStayList.Count);

        var homeStay = homeStayList[0];

        Assert.AreEqual("Home stay 2", homeStay["Name"]?.ToString());
        Assert.AreEqual("Ho Chi Minh", homeStay["City"]?.ToString());
    }


    [Test]
    public async Task GetHomeStayByUser_ReturnsNotFound_WhenNoHomeStayFound()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var emptyList = new List<HomeStay>().AsQueryable().BuildMock();

        _mockHomeStayRepo.Setup(repo => repo.FindWithInclude(It.IsAny<Expression<Func<HomeStay, object>>[]>()))
                         .Returns(emptyList);
        FilterDTO filter = new FilterDTO
        {
            AmenityNames = null,
            MaxPrice = null,
            MinPrice = null,
            SearchText = null,
            Standard = null
        };
        // Act
        var result = await _controller.GetHomeStayByUser(userId, filter);

        // Assert
        Assert.IsInstanceOf<NotFoundResult>(result);
    }
}
