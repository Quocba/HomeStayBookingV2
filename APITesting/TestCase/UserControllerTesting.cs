using API.Controllers;
using BusinessObject.DTO;
using BusinessObject.Entities;
using BusinessObject.Interfaces;
using BusinessObject.Wrappers;
using Microsoft.AspNetCore.Mvc;
using Moq;
#pragma warning disable
namespace APITesting;

public class UserControllerTesting
{
    private Mock<IUserRepository> _userRepositoryMock;
    private UserController _controller;

    [SetUp]
    public void Setup()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _controller = new UserController(_userRepositoryMock.Object);
    }

    [Test]
    public async Task GetAll_ReturnsOk_WithPagedUserList()
    {
        // Arrange
        var mockUsers = new List<UserListResponse>
            {
                new UserListResponse
                {
                    Id = Guid.NewGuid(),
                    FullName = "Nguyễn Văn A",
                    Email = "a@example.com",
                    Role = "User"
                },
                new UserListResponse
                {
                    Id = Guid.NewGuid(),
                    FullName = "Trần Thị B",
                    Email = "b@example.com",
                    Role = "Admin"
                }
            };


        var pagedResponse = new PagedResponse<List<UserListResponse>>(
            mockUsers,
            1,
            10,
            1
        );

        _userRepositoryMock
            .Setup(repo => repo.GetUsers(It.IsAny<PagedRequest>()))
            .ReturnsAsync(pagedResponse);

        // Act
        var result = await _controller.GetAll(1, 10) as OkObjectResult;

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(200, result.StatusCode);

        var data = result.Value as PagedResponse<List<UserListResponse>>;
        Assert.IsNotNull(data);
        Assert.AreEqual(2, data.Data.Count);
        Assert.AreEqual("Nguyễn Văn A", data.Data[0].FullName);
        Assert.AreEqual("Trần Thị B", data.Data[1].FullName);
    }

    [Test]
    public async Task Get_ReturnsOk_WithUserDetail()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var mockUser = new UserResponse
        {
            Id = userId,
            FullName = "Nguyễn Văn C",
            Email = "c@example.com",
            Phone = "0111222333",
            Address = "Đà Nẵng",
            Avatar = "http://example.com/avatar3.jpg",
            RoleId = 1,
            Role = "Admin"
        };

        _userRepositoryMock
            .Setup(repo => repo.GetUser(userId))
            .ReturnsAsync(mockUser); // Ensure mockUser is User type

        // Act
        var result = await _controller.Get(userId) as OkObjectResult;

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(200, result.StatusCode);

        var returnedUser = result.Value as UserResponse;
        Assert.IsNotNull(returnedUser);
        Assert.AreEqual("Nguyễn Văn C", returnedUser.FullName);
        Assert.AreEqual("c@example.com", returnedUser.Email);
    }

    [Test]
    public async Task Update_ReturnsOk_WhenUserIsUpdated()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var updateDto = new UpdateUserDTO
        {
            Email = "test@example.com",
            FullName = "Nguyễn Văn A",
            Address = "Hà Nội",
            Phone = "0123456789",
            RoleId = 1,
            Avatar = "http://example.com/avatar.jpg"
        };

        _userRepositoryMock
            .Setup(repo => repo.UpdateUser(It.IsAny<User>()))
            .Returns(Task.FromResult(true));

        _userRepositoryMock
            .Setup(repo => repo.SaveAsync())
            .Returns(Task.CompletedTask);

        // Act
        var result = await _controller.Update(userId, updateDto) as OkObjectResult;

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(200, result.StatusCode);

        dynamic response = result.Value;

        _userRepositoryMock.Verify(repo => repo.UpdateUser(It.Is<User>(u =>
            u.Id == userId &&
            u.Email == updateDto.Email &&
            u.FullName == updateDto.FullName &&
            u.Address == updateDto.Address &&
            u.Phone == updateDto.Phone &&
            u.RoleId == updateDto.RoleId &&
            u.Avatar == updateDto.Avatar
        )), Times.Once);

        _userRepositoryMock.Verify(repo => repo.SaveAsync(), Times.Once);
    }


}

