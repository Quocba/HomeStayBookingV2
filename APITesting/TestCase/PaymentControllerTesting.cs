using System.Linq.Expressions;
using API.Controllers;
using BusinessObject.Entities;
using BusinessObject.Exceptions;
using BusinessObject.Interfaces;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Moq;
using Newtonsoft.Json.Linq;
using PayOSService.Config;
using PayOSService.DTO;
using PayOSService.Services;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore;
#pragma warning disable

namespace APITesting;

public class PaymentControllerTesting
{
    private Mock<IRepository<Booking>> _mockBookingRepo;
    private Mock<IRepository<Transaction>> _mockTransactionRepo;
    private Mock<IPayOSService> _mockPayOSService;
    private Mock<IRepository<BusinessObject.Entities.Calendar>> _mockCalendarRepo;
    private IOptions<PayOSConfig> _payosConfigOptions;
    private Mock<IEmailSender> _emailSenderRepo;
    private PaymentController _controller;

    [SetUp]
    public void Setup()
    {
        _mockBookingRepo = new Mock<IRepository<Booking>>();
        _mockTransactionRepo = new Mock<IRepository<Transaction>>();
        _mockPayOSService = new Mock<IPayOSService>();
        _mockCalendarRepo = new Mock<IRepository<BusinessObject.Entities.Calendar>>();
        _emailSenderRepo = new Mock<IEmailSender>();

        _payosConfigOptions = Options.Create(new PayOSConfig
        {
            ReturnUrl = "https://example.com/payment-return"
        });

        _controller = new PaymentController(
            _mockBookingRepo.Object,
            _mockTransactionRepo.Object,
            _mockCalendarRepo.Object,
            _emailSenderRepo.Object,
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

        var json = JObject.FromObject(result.Value);
        var actualLink = json["paymentLink"]?.ToString();

        Assert.AreEqual(expectedLink, actualLink);

        _mockTransactionRepo.Verify(r => r.AddAsync(It.IsAny<Transaction>()), Times.Once);
        _mockTransactionRepo.Verify(r => r.SaveAsync(), Times.Once);
    }
}
