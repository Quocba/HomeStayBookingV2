using API.Controllers;
using BusinessObject.DTO;
using BusinessObject.Entities;
using BusinessObject.Interfaces;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;
#pragma warning disable

namespace APITesting;

public class FeedBackControllerTesting
{
    private Mock<IRepository<FeedBack>> _mockFeedbackRepo;
    private Mock<IRepository<HomeStay>> _mockHomestayRepo;
    private FeedbackController _controller;
    private Mock<IEmailSender> _mockEmailSender;
    [SetUp]
    public void Setup()
    {
        _mockFeedbackRepo = new Mock<IRepository<FeedBack>>();
        _mockHomestayRepo = new Mock<IRepository<HomeStay>>();
        _mockEmailSender = new Mock<IEmailSender>();
        _controller = new FeedbackController(_mockFeedbackRepo.Object, _mockHomestayRepo.Object, _mockEmailSender.Object);
    }


    [Test]
    public async Task CreateFeedback_ReturnsOkResult_WhenValidInput()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var homeStayId = Guid.NewGuid();
        var feedbackDto = new FeedbackDTO
        {
            HomestayID = homeStayId,
            Rating = 5,
            Description = "Great experience!"
        };

        var mockHomeStay = new HomeStay { Id = homeStayId, Name = "Sample HomeStay" };

        _mockHomestayRepo.Setup(repo => repo.GetByIdAsync(homeStayId)).ReturnsAsync(mockHomeStay);
        _mockFeedbackRepo.Setup(repo => repo.AddAsync(It.IsAny<FeedBack>())).Returns(Task.CompletedTask);
        _mockFeedbackRepo.Setup(repo => repo.SaveAsync()).Returns(Task.CompletedTask);

        // Act
        var result = await _controller.CreateFeedback(userId, feedbackDto);

        // Assert
        Assert.IsInstanceOf<OkObjectResult>(result);
        var okResult = result as OkObjectResult;
        Assert.IsInstanceOf<FeedBack>(okResult.Value);

        var createdFeedback = okResult.Value as FeedBack;
        Assert.AreEqual(userId, createdFeedback.UserID);
        Assert.AreEqual(feedbackDto.Rating, createdFeedback.Rating);
        Assert.AreEqual(feedbackDto.Description, createdFeedback.Description);
        Assert.AreEqual(mockHomeStay, createdFeedback.HomeStay);
    }

    [Test]
    public async Task CreateFeedback_ReturnsBadRequest_WhenUserIdIsEmpty()
    {
        // Arrange
        var feedbackDto = new FeedbackDTO
        {
            HomestayID = Guid.NewGuid(),
            Rating = 4,
            Description = "Nice"
        };

        // Act
        var result = await _controller.CreateFeedback(Guid.Empty, feedbackDto);

        // Assert
        Assert.IsInstanceOf<BadRequestObjectResult>(result);
        var badRequest = result as BadRequestObjectResult;
        Assert.AreEqual("User not found", badRequest.Value);
    }

    [Test]
    public async Task CreateFeedback_ReturnsServerError_WhenExceptionThrown()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var feedbackDto = new FeedbackDTO
        {
            HomestayID = Guid.NewGuid(),
            Rating = 5,
            Description = "Great"
        };

        _mockHomestayRepo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ThrowsAsync(new Exception("DB error"));

        // Act
        var result = await _controller.CreateFeedback(userId, feedbackDto);

        // Assert
        Assert.IsInstanceOf<ObjectResult>(result);
        var serverError = result as ObjectResult;
        Assert.AreEqual(500, serverError.StatusCode);
        Assert.IsTrue(serverError.Value.ToString().Contains("DB error"));
    }

    [Test]
    public async Task EditFeedback_ReturnsOk_WhenSuccessful()
    {
        // Arrange
        var feedbackId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var feedbackDto = new FeedbackDTO { Rating = 4, Description = "Updated" };
        var existingFeedback = new FeedBack
        {
            Id = feedbackId,
            UserID = userId,
            Rating = 2,
            Description = "Old description",
            IsReply = false
        };

        _mockFeedbackRepo.Setup(r => r.GetByIdAsync(feedbackId)).ReturnsAsync(existingFeedback);
        _mockFeedbackRepo.Setup(r => r.UpdateAsync(It.IsAny<FeedBack>())).Returns(Task.CompletedTask);
        _mockFeedbackRepo.Setup(r => r.SaveAsync()).Returns(Task.CompletedTask);

        // Act
        var result = await _controller.EditFeedback(feedbackId, userId, feedbackDto);

        // Assert
        Assert.IsInstanceOf<OkObjectResult>(result);
        var okResult = result as OkObjectResult;
        var feedback = okResult.Value as FeedBack;
        Assert.AreEqual(feedbackDto.Rating, feedback.Rating);
        Assert.AreEqual(feedbackDto.Description, feedback.Description);
    }

    [Test]
    public async Task EditFeedback_ReturnsBadRequest_WhenUserIdIsEmpty()
    {
        // Arrange
        var feedbackDto = new FeedbackDTO { Rating = 4, Description = "Update" };

        // Act
        var result = await _controller.EditFeedback(Guid.NewGuid(), Guid.Empty, feedbackDto);

        // Assert
        Assert.IsInstanceOf<BadRequestResult>(result);
    }

    [Test]
    public async Task EditFeedback_ReturnsNotFound_WhenFeedbackNotExistsOrDeleted()
    {
        // Arrange
        var feedbackId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        _mockFeedbackRepo.Setup(r => r.GetByIdAsync(feedbackId)).ReturnsAsync((FeedBack)null);

        // Act
        var result = await _controller.EditFeedback(feedbackId, userId, new FeedbackDTO());

        // Assert
        Assert.IsInstanceOf<NotFoundObjectResult>(result);
        var notFound = result as NotFoundObjectResult;
        Assert.AreEqual("Feedback not found.", notFound.Value);
    }

    [Test]
    public async Task EditFeedback_ReturnsForbid_WhenUserIsNotOwner()
    {
        // Arrange
        var feedbackId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var feedback = new FeedBack
        {
            Id = feedbackId,
            UserID = Guid.NewGuid(), // khác user
            IsReply = false
        };

        _mockFeedbackRepo.Setup(r => r.GetByIdAsync(feedbackId)).ReturnsAsync(feedback);

        // Act
        var result = await _controller.EditFeedback(feedbackId, userId, new FeedbackDTO());

        // Assert
        Assert.IsInstanceOf<ForbidResult>(result);
    }

    [Test]
    public async Task EditFeedback_ReturnsServerError_WhenExceptionThrown()
    {
        // Arrange
        var feedbackId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var feedbackDto = new FeedbackDTO { Rating = 3, Description = "New" };

        _mockFeedbackRepo.Setup(r => r.GetByIdAsync(feedbackId)).ThrowsAsync(new Exception("Something failed"));

        // Act
        var result = await _controller.EditFeedback(feedbackId, userId, feedbackDto);

        // Assert
        Assert.IsInstanceOf<ObjectResult>(result);
        var error = result as ObjectResult;
        Assert.AreEqual(500, error.StatusCode);
        Assert.IsTrue(error.Value.ToString().Contains("Something failed"));
    }

    [Test]
    public async Task EditFeedback_ReturnsBadRequest_WhenRatingIsOutOfRange()
    {
        // Arrange
        var feedbackId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        var existingFeedback = new FeedBack
        {
            Id = feedbackId,
            UserID = userId,
            IsReply = false
        };

        _mockFeedbackRepo.Setup(r => r.GetByIdAsync(feedbackId)).ReturnsAsync(existingFeedback);

        var invalidRatings = new[] { 0, 6 };

        foreach (var rating in invalidRatings)
        {
            var feedbackDto = new FeedbackDTO
            {
                Rating = rating,
                Description = "Invalid rating"
            };

            // Act
            var result = await _controller.EditFeedback(feedbackId, userId, feedbackDto);

            // Assert
            Assert.IsInstanceOf<BadRequestObjectResult>(result);
            var badRequest = result as BadRequestObjectResult;
            Assert.AreEqual("Rating must be between 1 and 5.", badRequest.Value);
        }
    }


    [Test]
    public async Task DeleteFeedback_ReturnsNotFound_WhenUserIdIsEmpty()
    {
        // Act
        var result = await _controller.DeleteFeedback(Guid.NewGuid(), Guid.Empty);

        // Assert
        Assert.IsInstanceOf<NotFoundResult>(result);
    }

    [Test]
    public async Task DeleteFeedback_ReturnsNotFound_WhenFeedbackNotFound()
    {
        // Arrange
        _mockFeedbackRepo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync((FeedBack)null);

        // Act
        var result = await _controller.DeleteFeedback(Guid.NewGuid(), Guid.NewGuid());

        // Assert
        Assert.IsInstanceOf<NotFoundObjectResult>(result);
    }

    [Test]
    public async Task DeleteFeedback_ReturnsNotFound_WhenFeedbackIsDeleted()
    {
        // Arrange
        var feedback = new FeedBack { Id = Guid.NewGuid(), IsReply = true };
        _mockFeedbackRepo.Setup(r => r.GetByIdAsync(feedback.Id))
            .ReturnsAsync(feedback);

        // Act
        var result = await _controller.DeleteFeedback(feedback.Id, Guid.NewGuid());

        // Assert
        Assert.IsInstanceOf<NotFoundObjectResult>(result);
    }

    [Test]
    public async Task DeleteFeedback_ReturnsForbid_WhenUserIsNotOwner()
    {
        // Arrange
        var feedback = new FeedBack
        {
            Id = Guid.NewGuid(),
            UserID = Guid.NewGuid(),
            IsReply = false
        };
        _mockFeedbackRepo.Setup(r => r.GetByIdAsync(feedback.Id)).ReturnsAsync(feedback);

        // Act
        var result = await _controller.DeleteFeedback(feedback.Id, Guid.NewGuid());

        // Assert
        Assert.IsInstanceOf<ForbidResult>(result);
    }

    [Test]
    public async Task DeleteFeedback_ReturnsOk_WhenDeletedSuccessfully()
    {
        // Arrange
        var feedbackId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var feedback = new FeedBack
        {
            Id = feedbackId,
            UserID = userId,
            IsReply = false
        };

        _mockFeedbackRepo.Setup(r => r.GetByIdAsync(feedbackId)).ReturnsAsync(feedback);
        _mockFeedbackRepo.Setup(r => r.UpdateAsync(It.IsAny<FeedBack>())).Returns(Task.CompletedTask);
        _mockFeedbackRepo.Setup(r => r.SaveAsync()).Returns(Task.CompletedTask);

        // Act
        var result = await _controller.DeleteFeedback(feedbackId, userId);

        // Assert
        Assert.IsInstanceOf<OkObjectResult>(result);
        var okResult = result as OkObjectResult;
        var returnedFeedback = okResult.Value as FeedBack;
        Assert.IsTrue(returnedFeedback.IsReply);
    }

    [Test]
    public async Task DeleteFeedback_ReturnsServerError_WhenExceptionThrown()
    {
        // Arrange
        var feedbackId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        _mockFeedbackRepo.Setup(r => r.GetByIdAsync(feedbackId)).ThrowsAsync(new Exception("Unexpected error"));

        // Act
        var result = await _controller.DeleteFeedback(feedbackId, userId);

        // Assert
        Assert.IsInstanceOf<ObjectResult>(result);
        var objResult = result as ObjectResult;
        Assert.AreEqual(500, objResult.StatusCode);
        Assert.IsTrue(objResult.Value.ToString().Contains("Unexpected error"));
    }

    [Test]
    public async Task GetFeedbackById_ReturnsBadRequest_WhenIdIsEmpty()
    {
        // Act
        var result = await _controller.GetFeedbackById(Guid.Empty);

        // Assert
        Assert.IsInstanceOf<BadRequestResult>(result);
    }

    [Test]
    public async Task GetFeedbackById_ReturnsNotFound_WhenFeedbackIsNull()
    {
        // Arrange
        _mockFeedbackRepo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>()))
                         .ReturnsAsync((FeedBack)null);

        // Act
        var result = await _controller.GetFeedbackById(Guid.NewGuid());

        // Assert
        Assert.IsInstanceOf<NotFoundObjectResult>(result);
        var notFound = result as NotFoundObjectResult;
        Assert.AreEqual("Feedback not found.", notFound.Value);
    }

    [Test]
    public async Task GetFeedbackById_ReturnsNotFound_WhenFeedbackIsDeleted()
    {
        // Arrange
        var feedback = new FeedBack
        {
            Id = Guid.NewGuid(),
            IsReply = true
        };

        _mockFeedbackRepo.Setup(r => r.GetByIdAsync(feedback.Id))
                         .ReturnsAsync(feedback);

        // Act
        var result = await _controller.GetFeedbackById(feedback.Id);

        // Assert
        Assert.IsInstanceOf<NotFoundObjectResult>(result);
        var notFound = result as NotFoundObjectResult;
        Assert.AreEqual("Feedback not found.", notFound.Value);
    }

    [Test]
    public async Task GetFeedbackById_ReturnsOk_WhenFeedbackIsValid()
    {
        // Arrange
        var feedback = new FeedBack
        {
            Id = Guid.NewGuid(),
            IsReply = false,
            Description = "Nice place!",
            Rating = 5
        };

        _mockFeedbackRepo.Setup(r => r.GetByIdAsync(feedback.Id))
                         .ReturnsAsync(feedback);

        // Act
        var result = await _controller.GetFeedbackById(feedback.Id);

        // Assert
        Assert.IsInstanceOf<OkObjectResult>(result);
        var okResult = result as OkObjectResult;
        Assert.AreEqual(feedback, okResult.Value);
    }

    [Test]
    public async Task GetFeedbackById_ReturnsInternalServerError_WhenExceptionThrown()
    {
        // Arrange
        _mockFeedbackRepo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>()))
                         .ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await _controller.GetFeedbackById(Guid.NewGuid());

        // Assert
        Assert.IsInstanceOf<ObjectResult>(result);
        var objectResult = result as ObjectResult;
        Assert.AreEqual(500, objectResult.StatusCode);
        Assert.IsTrue(objectResult.Value.ToString().Contains("Database error"));
    }
}
