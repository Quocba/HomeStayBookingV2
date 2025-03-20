using BusinessObject.DTO;
using BusinessObject.Entities;
using BusinessObject.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AmenityController(IRepository<Amenity> _amenityRepository) :
        ControllerBase
    {

        [HttpPost("add-system-amenity")]
        public async Task<IActionResult> AddAmenitySystem([FromBody] AddSystemAmenityRequest request)
        {
            if(request == null || request.AmenityNames.Count == 0)
            {
                return BadRequest(new ProblemDetails
                {
                    Type = "https://tools.ietf.org/html/rfc9110#section-15.5.1",
                    Title = "Bad Request",
                    Status = 400,
                    Detail = "AmenityNames cannot be empty.",
                    Extensions = { ["traceId"] = Guid.NewGuid().ToString() }
                });
            }
            var existingAmenityNames = (await _amenityRepository
                .GetAllAsync())
                .Select(a => a.Name.ToLower())
                .ToList();


            List<string> duplicateNames = new List<string>();
            List<Amenity> newAmenities = new List<Amenity>();

            foreach (var amenityName in request.AmenityNames)
            {
                if (existingAmenityNames.Contains(amenityName.ToLower()))
                {
                    duplicateNames.Add(amenityName);
                }
                else
                {
                    newAmenities.Add(new Amenity
                    {
                        Id = Guid.NewGuid(),
                        Name = amenityName
                    });
                }
            }

            if (newAmenities.Any())
            {
                await _amenityRepository.AddRangeAsync(newAmenities);
                await _amenityRepository.SaveAsync();
            }

            if (duplicateNames.Any())
            {
                return Conflict(new
                {
                    Message = "Some amenities already exist.",
                    Duplicates = duplicateNames
                });
            }

            return Ok(new { Message = "Add Amenity Success" });
        }


        [HttpGet("get-all-system-amenity")]
        public async Task<IActionResult> GetAllSystemAmenity()
        {
            var amenityList = await _amenityRepository.GetAllAsync();
            return Ok(amenityList);
        }
    }
}
