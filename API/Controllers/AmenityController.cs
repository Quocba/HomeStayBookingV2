using BusinessObject.DTO;
using BusinessObject.Entities;
using BusinessObject.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AmenityController(IRepository<Amenity> _amenityRepository, IRepository<HomestayAmenity> _homeStayAmentityRepository) :
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
            var amenityList = await _amenityRepository
                                  .FindWithInclude()
                                  .GroupBy(x => x.Name.ToLower())
                                  .Select(g => g.First())
                                  .ToListAsync();
            return Ok(amenityList);
        }

        [HttpPut("edit-amenity")]
        public async Task<IActionResult> EditAmentity([FromBody]EditAmentityDTO request)
        {
            try
            {
                if (request == null)
                {
                    return BadRequest("In valid input");
                }
                var checkUpdate = await _amenityRepository.GetByIdAsync(request.AmentityID);
                if (checkUpdate == null)
                {
                    return NotFound(new { Message = "Amentity Not Found" });
                }

                checkUpdate.Name = request.AmentityName ?? checkUpdate.Name;
                await _amenityRepository.SaveAsync();
                return Ok(new { Message = "Edit Amentity Success" });
            }
            catch (Exception ex) { 
            
                return StatusCode(500, ex.ToString());
            }
        }


        [HttpDelete("delete-amentity")]
        public async Task<IActionResult> DeleteAmentity([FromQuery] Guid AmentityID)
        {
            try
            {
                if (AmentityID == Guid.Empty)
                {
                    return BadRequest(new { Message = "AmentityID can be not empty" });
                }

                var getDelete = await _amenityRepository.GetByIdAsync(AmentityID);
                if (getDelete == null)
                {
                    return NotFound();
                }
                var relatedHomeStayAmenities = await _homeStayAmentityRepository.FindWithInclude()
                                                                                .Where(x => x.AmenityId == AmentityID)
                                                                                .ToListAsync();
                if (relatedHomeStayAmenities.Count > 0)
                {
                    _homeStayAmentityRepository.DeleteRange(relatedHomeStayAmenities);
                  await _homeStayAmentityRepository.SaveAsync();
                }

                await _amenityRepository.DeleteAsync(getDelete);
                await _amenityRepository.SaveAsync();
                return Ok(new { Message = "Delete Amentity Success" });
            }
            catch(Exception ex)
            {
                return StatusCode(500, ex.ToString());
            }
        }

    }
}
