using API.Controllers;
using BusinessObject.DTO;
using BusinessObject.Entities;
using BusinessObject.Exceptions;
using BusinessObject.Interfaces;
using DataAccess.Handlers;
using DataAccess.Services;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Moq;
#pragma warning disable
namespace APITesting.TestCase;

public class AuthControllerTesting
{
    private Mock<IUserRepository> _mockUserRepo;
    private Mock<ITokenService> _mockTokenService;
    private Mock<IPasswordHasher> _mockPasswordHasher;
    private Mock<IEmailSender> _mockEmailSender;
    private Mock<IConfiguration> _mockConfiguration;
    private AuthController _controller;

    [SetUp]
    public void Setup()
    {
        _mockUserRepo = new Mock<IUserRepository>();
        _mockTokenService = new Mock<ITokenService>();
        _mockPasswordHasher = new Mock<IPasswordHasher>();
        _mockEmailSender = new Mock<IEmailSender>();
        _mockConfiguration = new Mock<IConfiguration>();


        _controller = new AuthController(
            _mockUserRepo.Object,
            _mockTokenService.Object,
            _mockPasswordHasher.Object,
            _mockEmailSender.Object,
            _mockConfiguration.Object,
            null
        );
    }

    [Test]
    public async Task GetUser_ValidUserId_ReturnsOk()
    {
        // Arrange
        var userId = Guid.Parse("84C1A775-7F5D-4BD1-B8D5-E42DC1172F76");

        // Act
        var result = await _controller.Get(userId) as OkObjectResult;

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(200, result.StatusCode);
    }
    [Test]
    public async Task Register_NewUser_ReturnsOk()
    {

        var registerDto = new RegisterDTO
        {
            Id = Guid.NewGuid(),
            FullName = "Test User",
            Email = "test@example.com",
            Password = "Password123",
            Phone = "0123456789",
            Address = "Test Address",
            RoleId = 1
        };
        _mockUserRepo.Setup(repo => repo.GetByEmailAsync(registerDto.Email)).ReturnsAsync((User)null);
        _mockPasswordHasher.Setup(hasher => hasher.HashPassword(registerDto.Password)).Returns("hashed_password");
        _mockUserRepo.Setup(repo => repo.GenerateCodeConfirmEmail(It.IsAny<Guid>())).ReturnsAsync("confirmation_code");
        _mockConfiguration.Setup(config => config["Base:Url"]).Returns("http://localhost");

        // Act
        var result = await _controller.Register(registerDto) as OkObjectResult;

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(200, result.StatusCode);
        Assert.IsNotNull(result.Value);
    }

    [Test]
    public void Register_EmailAlreadyExists_ThrowsException()
    {
        // Arrange
        var registerDto = new RegisterDTO
        {
            Id = Guid.NewGuid(),
            FullName = "Test User",
            Email = "test@example.com",
            Password = "Password123",
            Phone = "0123456789",
            Address = "Test Address",
            RoleId = 1
        };
        _mockUserRepo.Setup(repo => repo.GetByEmailAsync(registerDto.Email)).ReturnsAsync(new User());

        // Act & Assert
        Assert.ThrowsAsync<InvalidCredentialsException>(async () => await _controller.Register(registerDto));
    }

    [Test]
    public async Task Register_MissingFields_Return400()
    {
        var registerDto = new RegisterDTO
        {
            FullName = null,
            Email = null,
            Phone = null,
            Address = null,
            Password = null,
            RoleId = 0
        };


        var result = await _controller.Register(registerDto) as ObjectResult;

        Assert.IsNotNull(result);
        Assert.AreEqual(400, result.StatusCode);
    }


    [Test]
    public async Task Login_ValidCredentials_ReturnsOk()
    {
        // Arrange
        var loginDto = new LoginDTO { Email = "test@example.com", Password = "Password123" };
        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = loginDto.Email,
            FullName = "Test User",
            PasswordHash = "hashed_password",
            IsEmailConfirmed = true,
            Role = new Role { Name = "Admin" }
        };

        _mockUserRepo.Setup(repo => repo.GetByEmailAsync(loginDto.Email)).ReturnsAsync(user);
        _mockPasswordHasher.Setup(hasher => hasher.VerifyPassword(loginDto.Password, user.PasswordHash)).Returns(true);
        _mockTokenService.Setup(service => service.GenerateJwtToken(user)).Returns("mocked_jwt_token");
        _mockTokenService.Setup(service => service.GenerateRefreshToken(64)).Returns("mocked_refresh_token");
        _mockUserRepo
        .Setup(repo => repo.SaveRefreshToken(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<DateTime>()))
        .ReturnsAsync(true);
        _mockUserRepo.Setup(repo => repo.SaveAsync()).Returns(Task.CompletedTask);

        // Act
        var result = await _controller.Login(loginDto) as OkObjectResult;

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(200, result.StatusCode);
        var response = result.Value as LoginResponse;
        Assert.IsNotNull(response);
        Assert.AreEqual(loginDto.Email, response.Email);
        Assert.AreEqual("mocked_jwt_token", response.AccessToken);
        Assert.AreEqual("mocked_refresh_token", response.RefreshToken);
    }


    [Test]
    public void Login_InvalidCredentials_ThrowsException()
    {
        // Arrange
        var loginDto = new LoginDTO { Email = "wrong@example.com", Password = "WrongPassword" };
        _mockUserRepo.Setup(repo => repo.GetByEmailAsync(loginDto.Email)).ReturnsAsync((User)null);

        // Act & Assert
        Assert.ThrowsAsync<InvalidCredentialsException>(async () => await _controller.Login(loginDto));
    }

    [Test]
    public void Login_UnconfirmedEmail_ThrowsException()
    {
        // Arrange
        var loginDto = new LoginDTO { Email = "test@example.com", Password = "password" };
        var user = new User
        {
            Email = "test@example.com",
            PasswordHash = "hashed_password",
            IsEmailConfirmed = false
        };

        _mockUserRepo.Setup(repo => repo.GetByEmailAsync(loginDto.Email)).ReturnsAsync(user);
        _mockPasswordHasher.Setup(hasher => hasher.VerifyPassword(loginDto.Password, user.PasswordHash)).Returns(true);

        // Act & Assert
        Assert.ThrowsAsync<InvalidCredentialsException>(async () => await _controller.Login(loginDto));
    }

    [Test]
    public void Login_WrongPassword_ThrowsException()
    {
        // Arrange
        var loginDto = new LoginDTO { Email = "test@example.com", Password = "wrong_password" };
        var user = new User
        {
            Email = "test@example.com",
            PasswordHash = "hashed_password",
            IsEmailConfirmed = true
        };

        _mockUserRepo.Setup(repo => repo.GetByEmailAsync(loginDto.Email)).ReturnsAsync(user);
        _mockPasswordHasher.Setup(hasher => hasher.VerifyPassword(loginDto.Password, user.PasswordHash)).Returns(false);

        // Act & Assert
        Assert.ThrowsAsync<InvalidCredentialsException>(async () => await _controller.Login(loginDto));
    }

    [Test]
    public void Login_EmailNotFound_ThrowsException()
    {
        // Arrange
        var loginDto = new LoginDTO { Email = "notfound@example.com", Password = "password" };
        _mockUserRepo.Setup(repo => repo.GetByEmailAsync(loginDto.Email)).ReturnsAsync((User)null);

        // Act & Assert
        Assert.ThrowsAsync<InvalidCredentialsException>(async () => await _controller.Login(loginDto));
    }



    [Test]
    public async Task ConfirmEmail_ValidCode_RedirectsToFrontend()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var code = "valid_code";

        _mockUserRepo.Setup(repo => repo.IsVerifyCode(userId, code)).ReturnsAsync(true);
        _mockUserRepo.Setup(repo => repo.ConfirmEmail(userId)).ReturnsAsync(true);
        _mockUserRepo.Setup(repo => repo.SaveAsync()).Returns(Task.CompletedTask);

        // Act
        var result = await _controller.ConfirmEmail(userId, code) as RedirectResult;

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual("http://localhost:3000", result.Url);
    }


    [Test]
    public async Task ConfirmEmail_InvalidCode_ReturnsRedirectWithoutConfirming()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var code = "invalid_code";

        _mockUserRepo.Setup(repo => repo.IsVerifyCode(userId, code)).ReturnsAsync(false);

        // Act
        var result = await _controller.ConfirmEmail(userId, code) as RedirectResult;

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual("http://localhost:3000", result.Url);
        _mockUserRepo.Verify(repo => repo.ConfirmEmail(It.IsAny<Guid>()), Times.Never);
        _mockUserRepo.Verify(repo => repo.SaveAsync(), Times.Once);
    }

    [Test]
    public async Task ReSendConfirmEmail_UserExists_EmailSent_ReturnsOk()
    {
        // Arrange
        var email = "sondche161827@fpt.edu.vn";
        var userId = Guid.NewGuid();
        var command = new ResendConfirmEmailDTO { Email = email };
        var user = new User { Id = userId, Email = email };
        var verificationCode = "valid_code";
        var expectedUrl = $"http://localhost:3000/api/auth/confirm-email?userId={userId}&code={verificationCode}";


        _mockUserRepo.Setup(repo => repo.GetByEmailAsync(email)).ReturnsAsync(user);

        _mockUserRepo.Setup(repo => repo.GenerateCodeConfirmEmail(userId)).ReturnsAsync(verificationCode);


        _mockUserRepo.Setup(repo => repo.SaveAsync()).Returns(Task.CompletedTask);

        _mockEmailSender
            .Setup(sender => sender.SendEmailAsync(email, "Confirm email", expectedUrl))
            .Returns(Task.CompletedTask);


        var result = await _controller.ReSendConfirmEmail(command) as OkObjectResult;


        Assert.NotNull(result);
        Assert.AreEqual(200, result.StatusCode);
    }

    [Test]
    public void ReSendConfirmEmail_UserNotFound_ThrowsNotFoundException()
    {
        // Arrange
        var email = "notfound@example.com";
        var command = new ResendConfirmEmailDTO { Email = email };

        _mockUserRepo.Setup(repo => repo.GetByEmailAsync(email)).ReturnsAsync((User)null);

        // Act & Assert
        var ex = Assert.ThrowsAsync<NotFoundException>(() => _controller.ReSendConfirmEmail(command));
        Assert.AreEqual("User not found", ex.Message);
    }

    [Test]
    public async Task ReSendConfirmEmail_EmailSendingFails_ReturnsException()
    {
        // Arrange
        var email = "test@example.com";
        var userId = Guid.NewGuid();
        var command = new ResendConfirmEmailDTO { Email = email };
        var user = new User { Id = userId, Email = email };

        _mockUserRepo.Setup(repo => repo.GetByEmailAsync(email)).ReturnsAsync(user);
        _mockUserRepo.Setup(repo => repo.GenerateCodeConfirmEmail(userId)).ReturnsAsync("valid_code");
        _mockUserRepo.Setup(repo => repo.SaveAsync()).Returns(Task.CompletedTask);
        _mockEmailSender.Setup(sender => sender.SendEmailAsync(email, "Confirm email", It.IsAny<string>()))
                        .ThrowsAsync(new Exception("Email service error"));

        // Act & Assert
        var ex = Assert.ThrowsAsync<Exception>(() => _controller.ReSendConfirmEmail(command));
        Assert.AreEqual("Email service error", ex.Message);
    }


    [Test]
    public async Task ChangePassword_UserNotFound_ThrowsInvalidCredentialsException()
    {
        // Arrange
        _mockUserRepo.Setup(repo => repo.GetByEmailAsync(It.IsAny<string>())).ReturnsAsync((User)null);

        var command = new ChangePasswordDTO
        {
            Email = "nonexistent@example.com",
            OldPassword = "OldPass123",
            NewPassword = "NewPass456"
        };

        // Act & Assert
        Assert.ThrowsAsync<InvalidCredentialsException>(
            async () => await _controller.ChangePassword(command)
        );
    }

    [Test]
    public void ChangePassword_WrongOldPassword_ThrowsInvalidCredentialsException()
    {
        // Arrange
        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = "user@example.com",
            PasswordHash = "hashed_OldPass123",
            IsEmailConfirmed = true
        };

        _mockUserRepo.Setup(repo => repo.GetByEmailAsync(user.Email))
            .ReturnsAsync(user);

        _mockPasswordHasher.Setup(hasher => hasher.VerifyPassword("WrongPass", user.PasswordHash))
            .Returns(false);

        var command = new ChangePasswordDTO
        {
            Email = user.Email,
            OldPassword = "WrongPass",
            NewPassword = "NewPass456"
        };

        // Act & Assert
        Assert.ThrowsAsync<InvalidCredentialsException>(
            async () => await _controller.ChangePassword(command)
        );
    }

    [Test]
    public void ChangePassword_EmailNotConfirmed_ThrowsInvalidCredentialsException()
    {
        // Arrange
        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = "user@example.com",
            PasswordHash = "hashed_OldPass123",
            IsEmailConfirmed = false
        };

        _mockUserRepo.Setup(repo => repo.GetByEmailAsync(user.Email))
            .ReturnsAsync(user);

        _mockPasswordHasher.Setup(hasher => hasher.VerifyPassword("OldPass123", user.PasswordHash))
            .Returns(true);

        var command = new ChangePasswordDTO
        {
            Email = user.Email,
            OldPassword = "OldPass123",
            NewPassword = "NewPass456"
        };

        // Act & Assert
        Assert.ThrowsAsync<InvalidCredentialsException>(
            async () => await _controller.ChangePassword(command)
        );
    }

    [Test]
    public async Task ChangePassword_Success_ReturnsOk()
    {
        // Arrange
        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = "user@example.com",
            PasswordHash = "hashed_OldPass123",
            IsEmailConfirmed = true
        };

        _mockUserRepo.Setup(repo => repo.GetByEmailAsync(user.Email))
            .ReturnsAsync(user);

        _mockPasswordHasher.Setup(hasher => hasher.VerifyPassword("OldPass123", user.PasswordHash))
            .Returns(true);

        _mockPasswordHasher.Setup(hasher => hasher.HashPassword("NewPass456"))
            .Returns("hashed_NewPass456");

        _mockUserRepo.Setup(repo => repo.ChangePassword(user.Id, "hashed_NewPass456"))
            .Returns(Task.FromResult(true));

        _mockUserRepo.Setup(repo => repo.SaveAsync())
            .Returns(Task.CompletedTask);

        var command = new ChangePasswordDTO
        {
            Email = user.Email,
            OldPassword = "OldPass123",
            NewPassword = "NewPass456"
        };

        // Act
        var result = await _controller.ChangePassword(command) as OkObjectResult;

        // Assert
        Assert.NotNull(result);
        Assert.AreEqual(200, result.StatusCode);
    }

    [Test]
    public async Task ChangePassword_Success_CallsChangePasswordAndSave()
    {
        // Arrange
        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = "user@example.com",
            PasswordHash = "hashed_OldPass123",
            IsEmailConfirmed = true
        };

        _mockUserRepo.Setup(repo => repo.GetByEmailAsync(user.Email))
            .ReturnsAsync(user);

        _mockPasswordHasher.Setup(hasher => hasher.VerifyPassword("OldPass123", user.PasswordHash))
            .Returns(true);

        _mockPasswordHasher.Setup(hasher => hasher.HashPassword("NewPass456"))
            .Returns("hashed_NewPass456");
        _mockUserRepo.Setup(repo => repo.ChangePassword(user.Id, "hashed_NewPass456"))
            .Returns(Task.FromResult(true));

        _mockUserRepo.Setup(repo => repo.SaveAsync())
            .Returns(Task.CompletedTask);

        var command = new ChangePasswordDTO
        {
            Email = user.Email,
            OldPassword = "OldPass123",
            NewPassword = "NewPass456"
        };

        // Act
        await _controller.ChangePassword(command);

        // Assert
        _mockUserRepo.Verify(repo => repo.ChangePassword(user.Id, "hashed_NewPass456"), Times.Once);
        _mockUserRepo.Verify(repo => repo.SaveAsync(), Times.Once);
    }

    [Test]
    public async Task ForgotPassword_UserNotFound_ThrowsNotFoundException()
    {
        // Arrange
        var command = new ForgotPasswordDTO { Email = "nonexistent@example.com" };

        _mockUserRepo.Setup(repo => repo.GetByEmailAsync(command.Email))
                     .ReturnsAsync((User)null);

        // Act & Assert
        var ex = Assert.ThrowsAsync<NotFoundException>(async () => await _controller.ForgotPassword(command));

        Assert.That(ex.Message, Is.EqualTo("User not found"));
    }

    [Test]
    public async Task ForgotPassword_ValidUser_SendsEmailSuccessfully()
    {
        // Arrange
        var user = new User { Id = Guid.NewGuid(), Email = "test@example.com" };
        var command = new ForgotPasswordDTO { Email = user.Email };
        string confirmationCode = "123456";
        string clientUrl = "https://example.com";

        _mockUserRepo.Setup(repo => repo.GetByEmailAsync(user.Email))
                     .ReturnsAsync(user);

        _mockUserRepo.Setup(repo => repo.GenerateCodeConfirmEmail(user.Id))
                     .ReturnsAsync(confirmationCode);

        _mockUserRepo.Setup(repo => repo.SaveAsync())
                     .Returns(Task.CompletedTask);

        _mockConfiguration.Setup(config => config["Base:UrlClient"])
                          .Returns(clientUrl);

        _mockEmailSender.Setup(sender => sender.SendEmailAsync(
            user.Email,
            "Forgot password",
            It.Is<string>(content => content.Contains(confirmationCode))
        )).Returns(Task.CompletedTask);

        // Act
        var result = await _controller.ForgotPassword(command) as OkObjectResult;

        // Assert
        Assert.NotNull(result);
        Assert.AreEqual(200, result.StatusCode);
    }

    [Test]
    public async Task ResetPassword_Successful_ReturnsOk()
    {
        // Arrange
        var command = new ResetPasswordDTO
        {
            UserId = Guid.NewGuid(),
            Code = "valid_code",
            Password = "NewPass@123"
        };

        _mockUserRepo.Setup(repo => repo.IsVerifyCode(command.UserId, command.Code))
                     .ReturnsAsync(true);
        _mockPasswordHasher.Setup(hasher => hasher.HashPassword(command.Password))
                           .Returns("hashed_password");
        _mockUserRepo.Setup(repo => repo.ChangePassword(command.UserId, It.IsAny<string>()))
                  .ReturnsAsync(true);
        _mockUserRepo.Setup(repo => repo.SaveAsync()).Returns(Task.CompletedTask);

        // Act
        var result = await _controller.ResetPassword(command) as OkObjectResult;

        // Assert
        Assert.NotNull(result);
        Assert.AreEqual(200, result.StatusCode);
    }

    [Test]
    public async Task ResetPassword_InvalidCode_ReturnsBadRequest()
    {
        // Arrange
        var command = new ResetPasswordDTO
        {
            UserId = Guid.NewGuid(),
            Code = "invalid_code",
            Password = "NewPass@123"
        };

        _mockUserRepo.Setup(repo => repo.IsVerifyCode(command.UserId, command.Code))
                     .ReturnsAsync(false); // Giả lập mã xác nhận sai

        // Act
        var result = await _controller.ResetPassword(command) as BadRequestObjectResult;

        // Assert
        Assert.NotNull(result); // Đảm bảo không null
        Assert.AreEqual(400, result.StatusCode);
        Assert.AreEqual("Invalid code", result.Value);
    }

    [Test]
    public async Task ResetPassword_UserNotFound_ReturnsNotFound()
    {
        // Arrange
        var command = new ResetPasswordDTO
        {
            UserId = Guid.NewGuid(),
            Code = "valid_code",
            Password = "NewPass@123"
        };

        _mockUserRepo.Setup(repo => repo.IsVerifyCode(command.UserId, command.Code))
                     .ReturnsAsync(true);
        _mockUserRepo.Setup(repo => repo.ChangePassword(command.UserId, It.IsAny<string>()))
                     .Throws(new NotFoundException("User not found"));

        // Act
        var result = await _controller.ResetPassword(command) as NotFoundObjectResult;

        // Assert
        Assert.NotNull(result);
        Assert.AreEqual(404, result.StatusCode);
    }

    [Test]
    public async Task ResetPassword_ChangePasswordFails_ReturnsInternalServerError()
    {
        // Arrange
        var command = new ResetPasswordDTO
        {
            UserId = Guid.NewGuid(),
            Code = "valid_code",
            Password = "NewPass@123"
        };

        _mockUserRepo.Setup(repo => repo.IsVerifyCode(command.UserId, command.Code))
                     .ReturnsAsync(true); // Mã xác nhận hợp lệ

        _mockUserRepo.Setup(repo => repo.ChangePassword(It.IsAny<Guid>(), It.IsAny<string>()))
                     .ThrowsAsync(new Exception("Database error")); // Giả lập lỗi DB

        // Act
        var result = await _controller.ResetPassword(command) as ObjectResult;

        // Assert
        Assert.NotNull(result);
        Assert.AreEqual(500, result.StatusCode);
        Assert.AreEqual("Internal Server Error: Database error", result.Value);
    }


    [Test]
    public async Task AccessToken_ValidRefreshToken_ReturnsNewAccessToken()
    {
        // Arrange
        var command = new AccessTokenDTO { RefreshToken = "valid_refresh_token" };
        var user = new User { Email = "test@example.com", FullName = "Test User" };

        _mockUserRepo.Setup(repo => repo.GetUserByRefreshToken(command.RefreshToken))
                     .ReturnsAsync(user);

        _mockTokenService.Setup(service => service.GenerateJwtToken(It.IsAny<User>()))
                         .Returns("new_access_token");

        // Act
        var result = await _controller.AccessToken(command) as OkObjectResult;

        // Debugging Output
        Console.WriteLine($"Result: {result}");
        Console.WriteLine($"Response: {result?.Value}");

        // Assert
        Assert.NotNull(result, "Result should not be null");
        Assert.AreEqual(200, result.StatusCode, "StatusCode should be 200");

        var response = result.Value as LoginResponse;
        Assert.NotNull(response, "Response should not be null");
        Assert.AreEqual("test@example.com", response.Email, "Email should match");
        Assert.AreEqual("Test User", response.FullName, "FullName should match");
        Assert.AreEqual("new_access_token", response.AccessToken, "AccessToken should match");
        Assert.AreEqual("valid_refresh_token", response.RefreshToken, "RefreshToken should match");
    }



    [Test]
    public async Task AccessToken_InvalidRefreshToken_ThrowsInvalidCredentialsException()
    {
        // Arrange
        var command = new AccessTokenDTO { RefreshToken = "invalid_refresh_token" };

        _mockUserRepo.Setup(repo => repo.GetUserByRefreshToken(command.RefreshToken))
                     .ReturnsAsync((User)null); // Giả lập không tìm thấy user

        // Act & Assert
        var ex = Assert.ThrowsAsync<InvalidCredentialsException>(async () => await _controller.AccessToken(command));

        Assert.That(ex.Message, Is.EqualTo("Invalid refresh token"));
    }


    [Test]
    public async Task AccessToken_TokenGenerationFails_ThrowsException()
    {
        // Arrange
        var command = new AccessTokenDTO { RefreshToken = "valid_refresh_token" };
        var user = new User { Email = "test@example.com", FullName = "Test User" };

        _mockUserRepo.Setup(repo => repo.GetUserByRefreshToken(command.RefreshToken))
                     .ReturnsAsync(user);

        _mockTokenService.Setup(service => service.GenerateJwtToken(user))
                         .Throws(new Exception("Token generation failed"));

        // Act & Assert
        var ex = Assert.ThrowsAsync<Exception>(async () => await _controller.AccessToken(command));

        Assert.That(ex.Message, Is.EqualTo("Token generation failed"));
    }

    [Test]
    public async Task AccessToken_NullRefreshToken_ThrowsArgumentNullException()
    {
        // Arrange
        var command = new AccessTokenDTO { RefreshToken = null };

        // Act & Assert
        var ex = Assert.ThrowsAsync<ArgumentNullException>(async () =>
            await _controller.AccessToken(command));

        Assert.AreEqual("RefreshToken", ex.ParamName);
    }


    [Test]
    public async Task Block_ValidUserId_ReturnsOk()
    {
        // Arrange
        var userId = Guid.NewGuid();
        _mockUserRepo.Setup(repo => repo.ChangeIsDeletedUser(userId, true))
                     .ReturnsAsync(true);
        _mockUserRepo.Setup(repo => repo.SaveAsync()).Returns(Task.CompletedTask);

        // Act
        var result = await _controller.Block(userId) as OkObjectResult;

        // Assert
        Assert.NotNull(result);
        Assert.AreEqual(200, result.StatusCode);
    }


    [Test]
    public async Task Block_InvalidUserId_ReturnsBadRequest()
    {
        // Arrange
        var invalidUserId = Guid.Empty;

        // Act
        var result = await _controller.Block(invalidUserId) as BadRequestObjectResult;

        // Assert
        Assert.NotNull(result);
        Assert.AreEqual(400, result.StatusCode);
    }





}
