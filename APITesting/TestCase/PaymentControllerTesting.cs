using System.Linq.Expressions;
using API.Controllers;
using BusinessObject.Entities;
using BusinessObject.Exceptions;
using BusinessObject.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Moq;
using Newtonsoft.Json.Linq;
using PayOSService.Config;
using PayOSService.DTO;
using PayOSService.Services;
#pragma warning disable

namespace APITesting;

public class PaymentControllerTesting
{
    private Mock<IRepository<Booking>> _mockBookingRepo;
    private Mock<IRepository<Transaction>> _mockTransactionRepo;
    private Mock<IPayOSService> _mockPayOSService;
    private IOptions<PayOSConfig> _payosConfigOptions;
    private PaymentController _controller;
    [SetUp]
    public void Setup()
    {
        _mockBookingRepo = new Mock<IRepository<Booking>>();
        _mockTransactionRepo = new Mock<IRepository<Transaction>>();
        _mockPayOSService = new Mock<IPayOSService>();
        _payosConfigOptions = Options.Create(new PayOSConfig()); 

        _controller = new PaymentController(
            _mockBookingRepo.Object,
            _mockTransactionRepo.Object,
            _mockPayOSService.Object,
            _payosConfigOptions
        );
    }

    [Test]
    public void CreatePayment_BookingNotFound_ThrowsNotFoundException()
    {
        // Arrange
        var bookingId = Guid.NewGuid();
        _mockBookingRepo.Setup(r => r.GetByIdAsync(bookingId)).ReturnsAsync((Booking)null);

        // Act & Assert
        var ex = Assert.ThrowsAsync<NotFoundException>(() => _controller.CreatePayment(bookingId));
        Assert.That(ex.Message, Is.EqualTo("Booking not found"));
    }

    [Test]
    public async Task CreatePayment_TransactionWithLinkExists_ReturnsExistingPaymentLink()
    {
        // Arrange
        var bookingId = Guid.NewGuid();
        var booking = new Booking { Id = bookingId, TotalPrice = 100000 };

        var transaction = new Transaction
        {
            BookingID = bookingId,
            PaymentLink = "https://old-link.com"
        };

        _mockBookingRepo.Setup(r => r.GetByIdAsync(bookingId))
                        .ReturnsAsync(booking);

        // Quan trọng: trả về danh sách có 1 transaction đã có paymentLink
        _mockTransactionRepo.Setup(r => r.FindAsync(It.IsAny<Expression<Func<Transaction, bool>>>()))
                            .ReturnsAsync(new List<Transaction> { transaction }.AsQueryable());

        // Act
        var result = await _controller.CreatePayment(bookingId) as OkObjectResult;

        // Assert
        Assert.NotNull(result);

        var json = JObject.FromObject(result.Value);
        var actualLink = json["paymentLink"]?.ToString();

        Assert.AreEqual("https://old-link.com", actualLink);

        // Verify không tạo transaction mới
        _mockTransactionRepo.Verify(r => r.AddAsync(It.IsAny<Transaction>()), Times.Never);
        _mockTransactionRepo.Verify(r => r.SaveAsync(), Times.Never);
    }


    [Test]
    public async Task CreatePayment_NoExistingTransaction_CreatesNewTransaction()
    {
        // Arrange
        var bookingId = Guid.NewGuid();
        var booking = new Booking
        {
            Id = bookingId,
            TotalPrice = 200000
        };

        _mockBookingRepo.Setup(r => r.GetByIdAsync(bookingId))
                        .ReturnsAsync(booking);

        // Transaction không tồn tại
        _mockTransactionRepo.Setup(r => r.FindAsync(It.IsAny<Expression<Func<Transaction, bool>>>()))
                            .ReturnsAsync(new List<Transaction>().AsQueryable());

        // Mock tạo payment link mới
        var expectedLink = "https://new-payment-link.com";
        _mockPayOSService.Setup(s => s.CreatePaymentAsync(It.IsAny<CreatePaymentDTO>()))
                         .ReturnsAsync(expectedLink);

        _mockTransactionRepo.Setup(r => r.AddAsync(It.IsAny<Transaction>()))
                            .Returns(Task.CompletedTask);

        _mockTransactionRepo.Setup(r => r.SaveAsync())
                            .Returns(Task.CompletedTask);

        // Act
        var result = await _controller.CreatePayment(bookingId) as OkObjectResult;

        // Assert
        Assert.NotNull(result);

        // Ép kiểu về JObject để kiểm tra dynamic trả về
        var json = JObject.FromObject(result.Value);
        var actualLink = json["paymentLink"]?.ToString();

        Assert.AreEqual(expectedLink, actualLink);

        // Kiểm tra AddAsync và SaveAsync được gọi đúng
        _mockTransactionRepo.Verify(r => r.AddAsync(It.IsAny<Transaction>()), Times.Once);
        _mockTransactionRepo.Verify(r => r.SaveAsync(), Times.Once);
    }

    [Test]
    public async Task PaymentReturn_WithSuccessCodeAndStatus_UpdatesBookingAndRedirects()
    {
        // Arrange
        var orderId = 999L;
        var bookingId = Guid.NewGuid();

        var transaction = new Transaction
        {
            ID = orderId,
            BookingID = bookingId
        };

        var booking = new Booking
        {
            Id = bookingId,
            Status = "Pending"
        };

        _mockTransactionRepo
            .Setup(r => r.GetByIdAsync(orderId))
            .ReturnsAsync(transaction);

        _mockBookingRepo
            .Setup(r => r.GetByIdAsync(bookingId))
            .ReturnsAsync(booking);

        _mockBookingRepo
            .Setup(r => r.UpdateAsync(It.IsAny<Booking>()))
            .Returns(Task.CompletedTask);

        _mockBookingRepo
            .Setup(r => r.SaveAsync())
            .Returns(Task.CompletedTask);

        var expectedRedirectUrl = "https://client-redirect.com";
        var config = new PayOSConfig
        {
            ClientRedirectUrl = expectedRedirectUrl
        };

        _payosConfigOptions = Options.Create(config);
        _controller = new PaymentController(
            _mockBookingRepo.Object,
            _mockTransactionRepo.Object,
            _mockPayOSService.Object,
            _payosConfigOptions
        );

        // Act
        var result = await _controller.PaymentReturn(
            code: "00",
            id: "abc123",
            cancel: "false",
            status: "PAID",
            orderCode: orderId.ToString()
        );

        // Assert
        Assert.IsInstanceOf<RedirectResult>(result);
        var redirectResult = result as RedirectResult;
        Assert.NotNull(redirectResult);
        Assert.AreEqual(expectedRedirectUrl, redirectResult.Url);

        // Kiểm tra booking được cập nhật đúng
        Assert.AreEqual("Paid", booking.Status);
        _mockBookingRepo.Verify(r => r.UpdateAsync(booking), Times.Once);
        _mockBookingRepo.Verify(r => r.SaveAsync(), Times.Once);
    }

    [Test]
    public async Task PaymentReturn_WithFailedCodeOrStatus_DoesNotUpdateBooking_StillRedirects()
    {
        // Arrange
        var orderId = 1001L;
        var bookingId = Guid.NewGuid();

        var transaction = new Transaction
        {
            ID = orderId,
            BookingID = bookingId
        };

        var booking = new Booking
        {
            Id = bookingId,
            Status = "Pending"
        };

        _mockTransactionRepo
            .Setup(r => r.GetByIdAsync(orderId))
            .ReturnsAsync(transaction);

        _mockBookingRepo
            .Setup(r => r.GetByIdAsync(bookingId))
            .ReturnsAsync(booking);

        _mockBookingRepo
            .Setup(r => r.UpdateAsync(It.IsAny<Booking>()))
            .Returns(Task.CompletedTask);

        _mockBookingRepo
            .Setup(r => r.SaveAsync())
            .Returns(Task.CompletedTask);

        var expectedRedirectUrl = "https://client-redirect.com";
        var config = new PayOSConfig
        {
            ClientRedirectUrl = expectedRedirectUrl
        };

        _payosConfigOptions = Options.Create(config);
        _controller = new PaymentController(
            _mockBookingRepo.Object,
            _mockTransactionRepo.Object,
            _mockPayOSService.Object,
            _payosConfigOptions
        );

        // Act
        var result = await _controller.PaymentReturn(
            code: "99",                  // code khác "00"
            id: "abc123",
            cancel: "true",
            status: "FAILED",            // status khác "PAID"
            orderCode: orderId.ToString()
        );

        // Assert
        Assert.IsInstanceOf<RedirectResult>(result);
        var redirectResult = result as RedirectResult;
        Assert.NotNull(redirectResult);
        Assert.AreEqual(expectedRedirectUrl, redirectResult.Url);

        // Status không bị đổi thành "Paid"
        Assert.AreNotEqual("Paid", booking.Status);

        // Vẫn gọi Update & Save để lưu trạng thái hiện tại
        _mockBookingRepo.Verify(r => r.UpdateAsync(booking), Times.Once);
        _mockBookingRepo.Verify(r => r.SaveAsync(), Times.Once);
    }




}
