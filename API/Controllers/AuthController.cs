using BusinessObject.DTO;
using BusinessObject.Entities;
using BusinessObject.Exceptions;
using BusinessObject.Interfaces;
using BusinessObject.Shares;
using DataAccess.Handlers;
using DataAccess.Repositories;
using DataAccess.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController(IUserRepository _userRepository, ITokenService _tokenService
        , IPasswordHasher _passwordHasher
        , IEmailSender _emailSender
        , IConfiguration _configuration) : ControllerBase
    {
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDTO command)
        {
            var existingUser = await _userRepository.GetByEmailAsync(command.Email);

            if (existingUser != null) throw new InvalidCredentialsException("User already exists");

            Guid userId = Guid.NewGuid();

            var user = new User
            {
                Id = userId,
                FullName = command.FullName,
                Email = command.Email,
                Phone = command.Phone,
                Address = command.Address,
                PasswordHash = _passwordHasher.HashPassword(command.Password),
                RoleId = command.RoleId
            };

            await _userRepository.AddAsync(user);

            string code = await _userRepository.GenerateCodeConfirmEmail(userId);

            await _userRepository.SaveAsync();

            string url = _configuration["Base:Url"] ?? string.Empty;
            string content = url + "/api/auth/confirm-email?userId=" + userId + "&code=" + code;

            await _emailSender.SendEmailAsync(command.Email, "Confirm email", content);
            return Ok(new { message = "User registered successfully." });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO command)
        {
            var user = await _userRepository.GetByEmailAsync(command.Email);

            if (user == null || !_passwordHasher.VerifyPassword(command.Password, user.PasswordHash)) throw new InvalidCredentialsException("Invalid credentials");

            if (!user.IsEmailConfirmed) throw new InvalidCredentialsException("Please confirm email");

            var accessToken = _tokenService.GenerateJwtToken(user);
            var refreshToken = _tokenService.GenerateRefreshToken(64);

            await _userRepository.SaveRefreshToken(user.Id, refreshToken, DateUtility.GetCurrentDateTime().AddDays(1));
            await _userRepository.SaveAsync();

            var response = new LoginResponse
            {
                Email = user.Email,
                FullName = user.FullName,
                AccessToken = accessToken,
                RefreshToken = refreshToken
            };
            if (response == null) return Unauthorized();
            return Ok(response);
        }

        [HttpGet("confirm-email")]
        public async Task<IActionResult> ConfirmEmail([FromQuery] Guid userId, [FromQuery] string code)
        {
            var confirm = await _userRepository.IsVerifyCode(userId, code);

            if (confirm) await _userRepository.ConfirmEmail(userId);

            await _userRepository.SaveAsync();
            return Ok(confirm);
        }

        [HttpPost("resend-confirm")]
        public async Task<IActionResult> ReSendConfirmEmail([FromBody] ResendConfirmEmailDTO command)
        {
            var existingUser = await _userRepository.GetByEmailAsync(command.Email);

            if (existingUser == null) throw new NotFoundException("User not found");

            string code = await _userRepository.GenerateCodeConfirmEmail(existingUser.Id);

            await _userRepository.SaveAsync();

            string url = _configuration["Base:Url"] ?? string.Empty;
            string content = url + "/api/auth/confirm-email?userId=" + existingUser.Id + "&code=" + code;

            await _emailSender.SendEmailAsync(existingUser.Email, "Confirm email", content);
            return Ok(new { message = "Registration successful" }); 
        }

        [HttpPut("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDTO command)
        {
            var user = await _userRepository.GetByEmailAsync(command.Email);

            if (user == null || !_passwordHasher.VerifyPassword(command.OldPassword, user.PasswordHash))
                throw new InvalidCredentialsException("Invalid credentials");

            if (!user.IsEmailConfirmed) throw new InvalidCredentialsException("Please confirm email");

            await _userRepository.ChangePassword(user.Id, _passwordHasher.HashPassword(command.NewPassword));

            await _userRepository.SaveAsync();
            return Ok(new {message = "Change password successful" });
        }

        [HttpPut("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDTO command)
        {
            var existingUser = await _userRepository.GetByEmailAsync(command.Email);

            if (existingUser == null) throw new NotFoundException("User not found");

            string code = await _userRepository.GenerateCodeConfirmEmail(existingUser.Id);

            await _userRepository.SaveAsync();

            string url = _configuration["Base:UrlClient"] ?? string.Empty;
            string content = url + "/api/auth/reset-password?userId=" + existingUser.Id + "&code=" + code;

            await _emailSender.SendEmailAsync(existingUser.Email, "Forgot password", content);
            return Ok(new { message = "Email sent" });
        }

        [HttpPut("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDTO command)
        {
            var confirm = await _userRepository.IsVerifyCode(command.UserId, command.Code);

            if (confirm) await _userRepository.ChangePassword(command.UserId, _passwordHasher.HashPassword(command.Password));

            await _userRepository.SaveAsync();
            return Ok(new { message = "Reset password successful" });
        }

        [HttpPut("access-token")]
        public async Task<IActionResult> AccessToken([FromBody] AccessTokenDTO command)
        {
            var user = await _userRepository.GetUserByRefreshToken(command.RefreshToken);

            if (user == null) throw new InvalidCredentialsException("Invalid refresh token");

            var accessToken = _tokenService.GenerateJwtToken(user);

            return Ok(new LoginResponse
            {
                Email = user.Email,
                FullName = user.FullName,
                AccessToken = accessToken,
                RefreshToken = command.RefreshToken
            });
        }

        [HttpPut("block")]
        [Authorize(Policy = "AdminPolicy")]
        public async Task<IActionResult> Block([FromQuery] Guid id)
        {
            await _userRepository.ChangeIsDeletedUser(id, true);

            await _userRepository.SaveAsync();
            return Ok(new { message = "Block successful" });
        }

        [HttpPut("unblock")]
        [Authorize(Policy = "AdminPolicy")]
        public async Task<IActionResult> UnBlock([FromQuery] Guid id)
        {
            await _userRepository.ChangeIsDeletedUser(id, false);

            await _userRepository.SaveAsync();
            return Ok(new { message = "UnBlock successful" });
        }
    }
}
