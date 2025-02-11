using BusinessObject.DTO;
using BusinessObject.Entities;
using BusinessObject.Interfaces;
using BusinessObject.Wrappers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController(IUserRepository _userRepository) : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            return Ok(await _userRepository.GetUsers(new PagedRequest
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
            }));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get([FromRoute] Guid id)
        {
            return Ok(await _userRepository.GetUser(id));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] UpdateUserDTO request)
        {
            await _userRepository.UpdateUser(new User
            {
                Id = id,
                Email = request.Email,
                FullName = request.FullName,
                Address = request.Address,
                Phone = request.Phone,
                RoleId = request.RoleId,
            });

            await _userRepository.SaveAsync();
            return Ok(new { message = "Update user successful" });
        }
    }
}
