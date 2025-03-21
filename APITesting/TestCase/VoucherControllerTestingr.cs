using System.Linq.Expressions;
using API.Controllers;
using BusinessObject.DTO;
using BusinessObject.Entities;
using BusinessObject.Interfaces;
using Microsoft.AspNetCore.Mvc;
using MockQueryable;
using Moq;
#pragma warning disable

namespace APITesting;

public class VoucherControllerTestingr
{
    private Mock<IRepository<Voucher>> _voucherRepoMock;
    private Mock<IRepository<User>> _userRepoMock;
    private Mock<IRepository<UserVoucher>> _userVoucherRepoMock;
    private VoucherController _controller;

    [SetUp]
    public void Setup()
    {
        _voucherRepoMock = new Mock<IRepository<Voucher>>();
        _userRepoMock = new Mock<IRepository<User>>();
        _userVoucherRepoMock = new Mock<IRepository<UserVoucher>>();

        _controller = new VoucherController(
            _voucherRepoMock.Object,
            _userRepoMock.Object,
            _userVoucherRepoMock.Object
        );
    }

    [Test]
    public async Task CreateVoucher_ReturnsOk_WhenVoucherCreatedSuccessfully()
    {
        // Arrange
        var inputVoucher = new Voucher
        {
            Description = "Discount 10%",
            Discount = 10,
            StartDate = DateTime.Now,
            EndDate = DateTime.Now.AddDays(7),
            Image = "image.png",
            QuantityUsed = 0
        };

        // Giả lập không có code bị trùng lặp
        _voucherRepoMock
            .Setup(repo => repo.Find(It.IsAny<Expression<Func<Voucher, bool>>>()))
            .Returns(Enumerable.Empty<Voucher>().AsQueryable());

        _voucherRepoMock
            .Setup(repo => repo.AddAsync(It.IsAny<Voucher>()))
            .Returns(Task.CompletedTask);

        _voucherRepoMock
            .Setup(repo => repo.SaveAsync())
            .Returns(Task.CompletedTask);

        // Act
        var result = await _controller.CreateVoucher(inputVoucher) as OkObjectResult;

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(200, result.StatusCode);
        Assert.That(result.Value?.ToString(), Does.Contain("Create Voucher Success"));

        _voucherRepoMock.Verify(repo => repo.AddAsync(It.IsAny<Voucher>()), Times.Once);
        _voucherRepoMock.Verify(repo => repo.SaveAsync(), Times.Once);
    }

    [Test]
    public async Task EditVoucher_ReturnsOk_WhenVoucherIsUpdated()
    {
        // Arrange
        var voucherId = Guid.NewGuid();
        var existingVoucher = new Voucher
        {
            Id = voucherId,
            Code = "ABC123",
            Description = "Old Desc",
            Discount = 5,
            StartDate = DateTime.Now.AddDays(-1),
            EndDate = DateTime.Now.AddDays(10),
            Image = "old.jpg",
            isDeleted = false,
            QuantityUsed = 1
        };

        var voucherList = new List<Voucher> { existingVoucher };
        var mockQueryable = voucherList.AsQueryable().BuildMock(); // Magic here

        _voucherRepoMock
           .Setup(repo => repo.Find(It.IsAny<Expression<Func<Voucher, bool>>>()))
           .Returns(mockQueryable);

        _voucherRepoMock
            .Setup(repo => repo.UpdateAsync(It.IsAny<Voucher>()))
            .Returns(Task.CompletedTask);

        _voucherRepoMock
            .Setup(repo => repo.SaveAsync())
            .Returns(Task.CompletedTask);

        var editDto = new EditVoucherDTO
        {
            Description = "Updated Desc",
            Discount = 10,
            StartDate = DateTime.Now,
            EndDate = DateTime.Now.AddDays(5),
            Image = "new.jpg",
            QuantityUsed = 3,
            isDeleted = false
        };

        // Act
        var result = await _controller.EditVoucher(voucherId, editDto) as OkObjectResult;

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(200, result.StatusCode);
        Assert.That(result.Value?.ToString(), Does.Contain("Voucher updated successfully"));
    }

    [Test   ]
    public async Task ReceiveVoucher_ReturnsOk_WhenValidRequest()
    {
        var userId = Guid.NewGuid();
        var voucherId = Guid.NewGuid();

        var user = new User { Id = userId };
        var voucher = new Voucher { Id = voucherId };
        var request = new ReceiveVoucherRequest { UserID = userId, VoucherID = voucherId };

        _userRepoMock.Setup(r => r.Find(It.IsAny<Expression<Func<User, bool>>>()))
            .Returns(new List<User> { user }.AsQueryable().BuildMock());

        _voucherRepoMock.Setup(r => r.Find(It.IsAny<Expression<Func<Voucher, bool>>>()))
            .Returns(new List<Voucher> { voucher }.AsQueryable().BuildMock());

        _userVoucherRepoMock.Setup(r => r.Find(It.IsAny<Expression<Func<UserVoucher, bool>>>()))
            .Returns(new List<UserVoucher>().AsQueryable().BuildMock());

        _userVoucherRepoMock.Setup(r => r.AddAsync(It.IsAny<UserVoucher>())).Returns(Task.CompletedTask);
        _userVoucherRepoMock.Setup(r => r.SaveAsync()).Returns(Task.CompletedTask);

        var result = await _controller.ReceiveVoucher(request);

        var okResult = result as OkObjectResult;
        Assert.IsNotNull(okResult);
        Assert.AreEqual(200, okResult.StatusCode);
        Assert.IsTrue(okResult.Value.ToString().Contains("Recive Voucher Success"));
    }


    [Test]
    public async Task ReceiveVoucher_ReturnsConflict_WhenAlreadyReceived()
    {
        var userId = Guid.NewGuid();
        var voucherId = Guid.NewGuid();

        var request = new ReceiveVoucherRequest { UserID = userId, VoucherID = voucherId };
        var existing = new UserVoucher { UserID = userId, VoucherID = voucherId };

        _userRepoMock.Setup(r => r.Find(It.IsAny<Expression<Func<User, bool>>>()))
            .Returns(new List<User> { new User { Id = userId } }.AsQueryable().BuildMock());

        _voucherRepoMock.Setup(r => r.Find(It.IsAny<Expression<Func<Voucher, bool>>>()))
            .Returns(new List<Voucher> { new Voucher { Id = voucherId } }.AsQueryable().BuildMock());

        _userVoucherRepoMock.Setup(r => r.Find(It.IsAny<Expression<Func<UserVoucher, bool>>>()))
            .Returns(new List<UserVoucher> { existing }.AsQueryable().BuildMock());

        var result = await _controller.ReceiveVoucher(request);

        var conflict = result as ObjectResult;
        Assert.IsNotNull(conflict);
        Assert.AreEqual(409, conflict.StatusCode);
        Assert.IsTrue(conflict.Value.ToString().Contains("already received"));
    }

    [Test]
    public async Task ReceiveVoucher_Returns500_WhenUserNotFound()
    {
        var userId = Guid.NewGuid();
        var voucherId = Guid.NewGuid();
        var request = new ReceiveVoucherRequest { UserID = userId, VoucherID = voucherId };

        _userRepoMock.Setup(r => r.Find(It.IsAny<Expression<Func<User, bool>>>()))
            .Returns(new List<User>().AsQueryable().BuildMock());

        _voucherRepoMock.Setup(r => r.Find(It.IsAny<Expression<Func<Voucher, bool>>>()))
            .Returns(new List<Voucher> { new Voucher { Id = voucherId } }.AsQueryable().BuildMock());

        _userVoucherRepoMock.Setup(r => r.Find(It.IsAny<Expression<Func<UserVoucher, bool>>>()))
            .Returns(new List<UserVoucher>().AsQueryable().BuildMock());

        var result = await _controller.ReceiveVoucher(request);

        var error = result as ObjectResult;
        Assert.AreEqual(500, error.StatusCode);
        Assert.IsTrue(error.Value.ToString().Contains("Object reference"));
    }

    [Test]
    public async Task ReceiveVoucher_Returns500_WhenVoucherNotFound()
    {
        var userId = Guid.NewGuid();
        var voucherId = Guid.NewGuid();
        var request = new ReceiveVoucherRequest { UserID = userId, VoucherID = voucherId };

        _userRepoMock.Setup(r => r.Find(It.IsAny<Expression<Func<User, bool>>>()))
            .Returns(new List<User> { new User { Id = userId } }.AsQueryable().BuildMock());

        _voucherRepoMock.Setup(r => r.Find(It.IsAny<Expression<Func<Voucher, bool>>>()))
            .Returns(new List<Voucher>().AsQueryable().BuildMock());

        _userVoucherRepoMock.Setup(r => r.Find(It.IsAny<Expression<Func<UserVoucher, bool>>>()))
            .Returns(new List<UserVoucher>().AsQueryable().BuildMock());

        var result = await _controller.ReceiveVoucher(request);

        var error = result as ObjectResult;
        Assert.AreEqual(500, error.StatusCode);
        Assert.IsTrue(error.Value.ToString().Contains("Object reference"));
    }

    [Test]
    public async Task ReceiveVoucher_Returns500_WhenExceptionThrown()
    {
        var userId = Guid.NewGuid();
        var voucherId = Guid.NewGuid();
        var request = new ReceiveVoucherRequest { UserID = userId, VoucherID = voucherId };

        var user = new User { Id = userId };
        var voucher = new Voucher { Id = voucherId };

        _userRepoMock.Setup(r => r.Find(It.IsAny<Expression<Func<User, bool>>>()))
            .Returns(new List<User> { user }.AsQueryable().BuildMock());

        _voucherRepoMock.Setup(r => r.Find(It.IsAny<Expression<Func<Voucher, bool>>>()))
            .Returns(new List<Voucher> { voucher }.AsQueryable().BuildMock());

        _userVoucherRepoMock.Setup(r => r.Find(It.IsAny<Expression<Func<UserVoucher, bool>>>()))
            .Returns(new List<UserVoucher>().AsQueryable().BuildMock());

        _userVoucherRepoMock.Setup(r => r.AddAsync(It.IsAny<UserVoucher>()))
            .Throws(new Exception("Unexpected DB error"));

        var result = await _controller.ReceiveVoucher(request);

        var error = result as ObjectResult;
        Assert.AreEqual(500, error.StatusCode);
        Assert.AreEqual("Unexpected DB error", error.Value);
    }

    [Test]
    public async Task GetVouchers_ReturnsAllVouchers_WhenOnlyValidIsFalse()
    {
        var vouchers = new List<Voucher>
    {
        new Voucher { Id = Guid.NewGuid(), Code = "ABC", Description = "Desc", Discount = 10, StartDate = DateTime.UtcNow.AddDays(-1), EndDate = DateTime.UtcNow.AddDays(5), Image = "img1.jpg", isDeleted = false },
        new Voucher { Id = Guid.NewGuid(), Code = "XYZ", Description = "Desc2", Discount = 20, StartDate = DateTime.UtcNow.AddDays(-3), EndDate = DateTime.UtcNow.AddDays(1), Image = "img2.jpg", isDeleted = false }
    };

        _voucherRepoMock.Setup(r => r.Find(It.IsAny<Expression<Func<Voucher, bool>>>()))
            .Returns(vouchers.AsQueryable().BuildMock());

        var result = await _controller.GetVouchers(false);

        var okResult = result as OkObjectResult;
        Assert.IsNotNull(okResult);
        Assert.AreEqual(200, okResult.StatusCode);

        var list = okResult.Value as IEnumerable<dynamic>;
        Assert.AreEqual(2, list.Count());
    }

    [Test]
    public async Task GetVouchers_ReturnsOnlyValidVouchers_WhenOnlyValidIsTrue()
    {
        var vouchers = new List<Voucher>
    {
        new Voucher { Id = Guid.NewGuid(), Code = "VALID", Description = "Still Valid", Discount = 15, StartDate = DateTime.UtcNow.AddDays(-1), EndDate = DateTime.UtcNow.AddDays(2), Image = "img.jpg", isDeleted = false },
        new Voucher { Id = Guid.NewGuid(), Code = "EXPIRED", Description = "Expired", Discount = 25, StartDate = DateTime.UtcNow.AddDays(-10), EndDate = DateTime.UtcNow.AddDays(-1), Image = "img2.jpg", isDeleted = false }
    };

        _voucherRepoMock.Setup(r => r.Find(It.IsAny<Expression<Func<Voucher, bool>>>()))
            .Returns(vouchers.AsQueryable().BuildMock());

        var result = await _controller.GetVouchers(true);

        var okResult = result as OkObjectResult;
        Assert.IsNotNull(okResult);
        Assert.AreEqual(200, okResult.StatusCode);
    }

    [Test]
    public async Task GetVouchers_ReturnsNotFound_WhenNoVouchersExist()
    {
        _voucherRepoMock.Setup(r => r.Find(It.IsAny<Expression<Func<Voucher, bool>>>()))
            .Returns(new List<Voucher>().AsQueryable().BuildMock());

        var result = await _controller.GetVouchers();

        var notFound = result as ObjectResult;
        Assert.IsNotNull(notFound);
        Assert.AreEqual(404, notFound.StatusCode);
        Assert.IsTrue(notFound.Value.ToString().Contains("No vouchers found"));
    }

    [Test]
    public async Task GetUserVouchers_ReturnsNotFound_WhenUserNotExist()
    {
        var userId = Guid.NewGuid();

        _userRepoMock.Setup(r => r.Find(It.IsAny<Expression<Func<User, bool>>>()))
            .Returns(new List<User>().AsQueryable().BuildMock());

        var result = await _controller.GetUserVouchers(userId);

        var notFound = result as NotFoundObjectResult;
        Assert.IsNotNull(notFound);
        Assert.AreEqual(404, notFound.StatusCode);
        Assert.IsTrue(notFound.Value.ToString().Contains("User not found"));
    }

    [Test]
    public async Task GetUserVouchers_ReturnsValidVouchers_WhenUserHasVouchers()
    {
        var userId = Guid.NewGuid();
        var voucher = new Voucher
        {
            Id = Guid.NewGuid(),
            Code = "SAVE10",
            Description = "10% Off",
            Discount = 10,
            StartDate = DateTime.UtcNow.AddDays(-1),
            EndDate = DateTime.UtcNow.AddDays(5),
            Image = "voucher.png",
            isDeleted = false
        };
        var userVoucher = new UserVoucher
        {
            UserID = userId,
            isUsed = false,
            voucher = voucher,
            VoucherID = voucher.Id
        };

        _userRepoMock.Setup(r => r.Find(It.IsAny<Expression<Func<User, bool>>>()))
            .Returns(new List<User> { new User { Id = userId } }.AsQueryable().BuildMock());

        _userVoucherRepoMock.Setup(r => r.Find(It.IsAny<Expression<Func<UserVoucher, bool>>>()))
            .Returns(new List<UserVoucher> { userVoucher }.AsQueryable().BuildMock());

        var result = await _controller.GetUserVouchers(userId);

        var okResult = result as OkObjectResult;
        Assert.IsNotNull(okResult);
        Assert.AreEqual(200, okResult.StatusCode);

        var vouchers = okResult.Value as IEnumerable<dynamic>;
        Assert.IsNotNull(vouchers);
        Assert.AreEqual(1, vouchers.Count());
    }


    [Test]
    public async Task GetUserVouchers_ReturnsNotFound_WhenUserHasNoValidVoucher()
    {
        var userId = Guid.NewGuid();

        _userRepoMock.Setup(r => r.Find(It.IsAny<Expression<Func<User, bool>>>()))
            .Returns(new List<User> { new User { Id = userId } }.AsQueryable().BuildMock());

        _userVoucherRepoMock.Setup(r => r.Find(It.IsAny<Expression<Func<UserVoucher, bool>>>()))
            .Returns(new List<UserVoucher>().AsQueryable().BuildMock());

        var result = await _controller.GetUserVouchers(userId);

        var notFound = result as NotFoundObjectResult;
        Assert.IsNotNull(notFound);
        Assert.AreEqual(404, notFound.StatusCode);
        Assert.IsTrue(notFound.Value.ToString().Contains("No valid vouchers found"));
    }



}
