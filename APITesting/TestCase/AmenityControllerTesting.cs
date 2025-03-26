﻿using System.Net.Http;
using System.Net;
using System.Text;
using API.Controllers;
using BusinessObject.DTO;
using BusinessObject.Entities;
using BusinessObject.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Newtonsoft.Json;
using MockQueryable;

namespace APITesting.TestCase;

public class AmenityControllerTesting
{
    private Mock<IRepository<Amenity>> _mockRepo;
    private AmenityController _controller;

    [SetUp]
    public void Setup()
    {
        _mockRepo = new Mock<IRepository<Amenity>>();
        _controller = new AmenityController(_mockRepo.Object);
    }


    [Test]
    public async Task GetAllAmentitySuccess()
    {
        // Arrange
        var fakeAmenities = new List<Amenity>
    {
        new Amenity { Id = Guid.NewGuid(), Name = "WiFi" },
        new Amenity { Id = Guid.NewGuid(), Name = "Air Conditioner" }
    }.AsQueryable();

        var mockAmenityQueryable = fakeAmenities.BuildMock(); // tạo IQueryable mock hỗ trợ async

        _mockRepo.Setup(x => x.FindWithInclude()).Returns(mockAmenityQueryable);

        // Act
        var result = await _controller.GetAllSystemAmenity();

        // Assert
        Assert.IsNotNull(result);
    }


    [Test]
    public async Task AddAmenitySystem_Success()
    {
        // Arrange
        var request = new AddSystemAmenityRequest
        {
            AmenityNames = new List<string> { "Free Wifi", "Air Conditioning" }
        };

        _mockRepo.Setup(repo => repo.GetAllAsync())
            .ReturnsAsync(new List<Amenity>()); // Không có tiện ích nào trùng

        _mockRepo.Setup(repo => repo.AddRangeAsync(It.IsAny<List<Amenity>>()))
            .Returns(Task.CompletedTask);
        _mockRepo.Setup(repo => repo.SaveAsync()).Returns(Task.CompletedTask);

        // Act
        var result = await _controller.AddAmenitySystem(request) as OkObjectResult;

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(200, result.StatusCode);

        var json = JsonConvert.SerializeObject(result.Value);
        var response = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);

        Assert.IsNotNull(response);
        Assert.AreEqual("Add Amenity Success", response["Message"]);

    }

    [Test]
    public async Task AddAmenitySystem_Conflict()
    {
        // Arrange: Tạo mock repository
        var mockRepo = new Mock<IRepository<Amenity>>();
        mockRepo.Setup(repo => repo.GetAllAsync()).ReturnsAsync(new List<Amenity>
    {
        new Amenity { Name = "Free-Wifi" },
        new Amenity { Name = "Air-Conditioning" }
    });

        var controller = new AmenityController(mockRepo.Object);

        var request = new AddSystemAmenityRequest
        {
            AmenityNames = new List<string> { "Free-Wifi", "Air-Conditioning" } // Trùng với dữ liệu mock
        };

        var result = await controller.AddAmenitySystem(request);

        Assert.IsInstanceOf<ConflictObjectResult>(result);
        var conflictResult = result as ConflictObjectResult;
        Assert.AreEqual(409, conflictResult.StatusCode);
    }



    [Test]
    public async Task AddAmenitySystem_EmptyList()
    {
        // Arrange
        var request = new AddSystemAmenityRequest
        {
            AmenityNames = new List<string>()
        };

        var result = await _controller.AddAmenitySystem(request) as ObjectResult;

        Assert.IsNotNull(result);
        Assert.AreEqual(400, result.StatusCode);
    }

    [Test]
    public async Task AddAmenitySystem_NullRequest()
    {
        var result = await _controller.AddAmenitySystem(null) as ObjectResult;

        Assert.IsNotNull(result);
        Assert.AreEqual(400, result.StatusCode);
    }
}
