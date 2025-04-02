using System.Drawing;
using BusinessObject.DTO;
using BusinessObject.Entities;
using BusinessObject.Interfaces;
using BusinessObject.Shares;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HomeStayController(IRepository<HomeStay> _homeStayRepository,
        IRepository<User> _userRepository,
        IRepository<HomeStayImage> _homeStayImageRepository,
        IRepository<Calendar> _calendarRepository,
        IWebHostEnvironment _eviroment,
        IRepository<Amenity> _amenityRepository,
        IRepository<HomestayAmenity> _homeStayAmenity,
        HttpClient _httpClient,
        IRepository<HomeStayFacility> _homestayFacility,
        IRepository<Facility> _facilityRepository
            ) : ControllerBase
    {


        [HttpPost("add-home-stay-facility")]
        public async Task<IActionResult> AddHomeStayFacility(AddHomeStayFacilityDTO request)
        {
            try
            {
                if (request == null) return BadRequest();
                HomeStayFacility addFacility = new HomeStayFacility
                {
                    FacilityID = request.FacilityID,
                    HomeStayID = request.HomeStayID,
                };
                await _homestayFacility.AddAsync(addFacility);
                await _homestayFacility.SaveAsync();
                return Ok(new { Message = "Add Facility Success" });
            }
            catch (Exception ex) {
                return StatusCode(500, ex);
            }
        }

        [HttpDelete("delete-home-stay-facility")]
        public async Task<IActionResult> DeleteHomeStayFacility([FromQuery] Guid HomeStayID, Guid FacilityID)
        {
            try
            {
                if (HomeStayID == Guid.Empty || FacilityID == Guid.Empty) return BadRequest();

                var checkDelete = await _homestayFacility.Find(h => h.HomeStayID == HomeStayID && h.FacilityID == FacilityID)
                                                        .FirstOrDefaultAsync();
                if (checkDelete != null)
                {
                    await _homestayFacility.DeleteAsync(checkDelete);
                    await _homestayFacility.SaveAsync();
                    return Ok(new { Message = "Delete Amenity Success" });
                }
                return new NotFoundResult();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.ToString());
            }
        }

        [HttpGet("search-autocomplete-agoda")]
        public async Task<IActionResult> GetBookingHotel([FromQuery] String city)
        {
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri($"https://agoda-com.p.rapidapi.com/hotels/auto-complete?query={city}"),
                Headers =
            {
                { "x-rapidapi-key", "52bcd7f98bmsh281c68c3f9d7c44p169949jsn21d2e2049b30" },
                { "x-rapidapi-host", "agoda-com.p.rapidapi.com" },
            },
            };

            using (var response = await _httpClient.SendAsync(request))
            {
                response.EnsureSuccessStatusCode();
                var body = await response.Content.ReadAsStringAsync();
                return Ok(body);
            }

        }

        [HttpGet("search-hotel-by-location")]
        public async Task<IActionResult> GetHotelByLocation([FromQuery] string location)
        {
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri($"https://booking-com.p.rapidapi.com/v1/hotels/locations?name={location}&locale=en-gb"),
                Headers ={
                    { "x-rapidapi-key", "52bcd7f98bmsh281c68c3f9d7c44p169949jsn21d2e2049b30" },
                    { "x-rapidapi-host", "booking-com.p.rapidapi.com" }
                }
            };
            using (var reponse = await _httpClient.SendAsync(request))
            {
                reponse.EnsureSuccessStatusCode();
                var responseBody = await reponse.Content.ReadAsStringAsync();
                return Ok(responseBody);
            }
        }

        [HttpPost("add-home-stay")]
        public async Task<IActionResult> AddHomeStay([FromHeader(Name = "X-User-Id")] Guid userID, [FromBody] AddHomeStayRequest request)
        {
            try
            {
                if (request == null) return BadRequest();
                var getHomeStay = await _homeStayRepository.FindWithInclude()
                    .FirstOrDefaultAsync(x => x.Name.Trim().ToLower() == request.Name.Trim().ToLower() && x.UserID == userID);

                if (getHomeStay != null)
                {
                    return Conflict(new { Message = "Home Stay Name Already Exists" });
                }
                Guid homeStayID = Guid.NewGuid();
                HomeStay createHomeStay = new HomeStay
                {
                    Id = homeStayID,
                    MainImage = request.MainImage,
                    Name = request.Name,
                    Description = request.Description,
                    Address = request.Address,
                    CheckInTime = request.CheckInTime,
                    CheckOutTime = request.CheckOutTime,
                    isDeleted = request.IsDeleted,
                    City = request.City,
                    OpenIn = request.OpenIn,
                    Standar = request.Standar,
                    UserID = userID
                };
                await _homeStayRepository.AddAsync(createHomeStay);

                foreach (var item in request.Images)
                {

                    Guid imageID = Guid.NewGuid();
                    HomeStayImage addImage = new HomeStayImage
                    {
                        Id = imageID,
                        Image = item,
                        HomeStay = createHomeStay,
                        isDeleted = false
                    };
                    await _homeStayImageRepository.AddAsync(addImage);
                }

                await _homeStayRepository.SaveAsync();


                return Ok(new { message = "Add Home Stay Success." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.ToString());
            }
        }

        [HttpPost("add-home-stay-amenity")]
        public async Task<IActionResult> AddHomeStayAmennity([FromBody] AddAmenityDTO request)
        {
            try
            {
                if (request == null) return BadRequest();

                var getHomeStay = await _homeStayRepository.GetByIdAsync(request.HomeStayID);

                if (getHomeStay == null) return NotFound();
                foreach (var amenity in request.AmenityName)
                {
                    var getAmenity = await _amenityRepository.Find(n => n.Name.Equals(amenity)).FirstOrDefaultAsync();
                    var existingAmenity = await _homeStayAmenity
                                               .Find(x => x.HomeStayID == getHomeStay.Id && x.AmenityId == getAmenity.Id)
                                               .FirstOrDefaultAsync();
                    if (existingAmenity != null)
                    {
                        return Conflict();

                    }
                    HomestayAmenity addAmenity = new HomestayAmenity
                    {
                        AmenityId = getAmenity.Id,
                        HomeStayID = getHomeStay.Id,
                    };
                    await _homeStayAmenity.AddAsync(addAmenity);
                    await _homeStayAmenity.SaveAsync();
                }
                return Ok(new { Message = "Add Amentity Success" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred", Error = ex.Message });
            }
        }

        [HttpPut("edit-home-stay-information")]
        public async Task<IActionResult> EditHomeStay([FromBody] EditHomeStayInforRequest request)
        {
            try
            {
                if (request == null) return BadRequest();


                var getHomeStay = await _homeStayRepository.GetByIdAsync(request.HomeStayID);

                if (getHomeStay == null)
                {
                    return NotFound();
                }

                if (!request.Name.Equals(getHomeStay.Name, StringComparison.OrdinalIgnoreCase))
                {
                    var checkAlreadyName = await _homeStayRepository.FindWithInclude()
                        .FirstOrDefaultAsync(x => x.Name.Equals(request.Name) && x.UserID == getHomeStay.UserID);

                    if (checkAlreadyName != null)
                    {
                        return Conflict(new { Message = "Home Stay Name Already Exists" });
                    }
                }

                getHomeStay.Name = request.Name ?? getHomeStay.Name;
                getHomeStay.MainImage = request.MainImage ?? getHomeStay.MainImage;
                getHomeStay.Standar = getHomeStay.Standar = request.Standar != 0 ? request.Standar : getHomeStay.Standar;
                getHomeStay.CheckOutTime = request.CheckOutTime ?? getHomeStay.CheckOutTime;
                getHomeStay.CheckInTime = request.CheckInTime ?? getHomeStay.CheckInTime;
                getHomeStay.Address = request.Address ?? getHomeStay.Address;
                getHomeStay.OpenIn = request.OpenIn = request.OpenIn != 0 ? request.OpenIn : getHomeStay.OpenIn;
                getHomeStay.Description = request.Description ?? getHomeStay.Description;
                getHomeStay.City = request.City ?? getHomeStay.City;
                await _homeStayRepository.UpdateAsync(getHomeStay);
                await _homeStayRepository.SaveAsync();
                return Ok(new { Message = "Update Home Stay Success" });
            }
            catch (Exception ex) {
                return StatusCode(500, new { Message = "An error occurred", Error = ex.Message });
            }
        }

        [HttpDelete("delete-home-stay")]
        public async Task<IActionResult> DeleteHomeStay([FromQuery] Guid homeStayID)
        {
            try
            {
                if (homeStayID == Guid.Empty) return BadRequest();

                var checkDelete = await _homeStayRepository.GetByIdAsync(homeStayID);

                if (checkDelete != null && checkDelete.isDeleted == false)
                {
                    checkDelete.isDeleted = true;
                    await _homeStayRepository.UpdateAsync(checkDelete);
                    await _homeStayRepository.SaveAsync();
                    return Ok(new { Message = "Delete Success" });
                }
                else if (checkDelete != null && checkDelete.isDeleted == true)
                {
                    checkDelete.isDeleted = false;
                    await _homeStayRepository.UpdateAsync(checkDelete);
                    await _homeStayRepository.SaveAsync();
                    return Ok(new { Message = "Restore Home Stay Success" });
                }
                return NotFound();
            }
            catch (Exception ex) {

                return StatusCode(500, ex.ToString());
            }
        }

        [HttpDelete("delete-home-stay-amenity")]
        public async Task<IActionResult> DeleteHomeStayAmenity([FromQuery] Guid HomeStayID, Guid AmenityID)
        {
            try
            {
                if (HomeStayID == Guid.Empty || AmenityID == Guid.Empty) return BadRequest();

                var checkDelete = await _homeStayAmenity.Find(h => h.HomeStayID == HomeStayID && h.AmenityId == AmenityID)
                                                        .FirstOrDefaultAsync();
                if (checkDelete != null)
                {
                    checkDelete.isDeleted = true;
                    await _homeStayAmenity.DeleteAsync(checkDelete);
                    await _homeStayAmenity.SaveAsync();
                    return Ok(new { Message = "Delete Amenity Success" });
                }
                return NotFound();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.ToString());
            }
        }

        [HttpPost("get-all-home-stay")]
        public async Task<IActionResult> GetAllHomeStay([FromBody] FilterDTO request)
        {
            var query = _homeStayRepository
                .FindWithInclude(h => h.Calendars!)
                .Include(h => h.HomestayAmenities!)
                .ThenInclude(ha => ha.Amenity)
                .Include(hf => hf.HomestayFacilities)
                .ThenInclude(fa => fa.Facility)
                .AsQueryable();

            if (request.Standard is { Count: > 0 })
            {
                query = query.Where(h => request.Standard.Contains(h.Standar));
            }

            if (request.AmenityNames is { Count: > 0 })
            {
                query = query.Where(h =>
                    h.HomestayAmenities!.Any(ha => request.AmenityNames.Contains(ha.Amenity.Name)));
            }


            if (request.MinPrice.HasValue || request.MaxPrice.HasValue)
            {
                query = query.Where(h => h.Calendars!.Any(c =>
                    (!request.MinPrice.HasValue || c.Price >= request.MinPrice.Value) &&
                    (!request.MaxPrice.HasValue || c.Price <= request.MaxPrice.Value)
                ));
            }

            var listHomeStay = await query.ToListAsync();

            if (!listHomeStay.Any())
            {
                return NotFound();
            }

            var response = listHomeStay.Select(h => new
            {
                h.Id,
                h.Name,
                h.MainImage,
                h.Address,
                h.City,
                h.CheckInTime,
                h.CheckOutTime,
                h.OpenIn,
                h.Description,
                h.Standar,
                h.isDeleted,

                Calendar = h.Calendars!.Select(c => new
                {
                    c.Id,
                    c.Date,
                    c.Price,
                    c.isBooked
                }).ToList(),

                Amenities = h.HomestayAmenities!
                    .Select(ha => new
                    {
                        ha.Amenity.Id,
                        ha.Amenity.Name
                    }).ToList(),
                Facility = h.HomestayFacilities!.Select(hf => new
                {
                    hf.FacilityID,
                    hf.Facility.Name,
                    hf.Facility.Description
                }).ToList()
            }).ToList();

            return Ok(response);
        }

        [HttpGet("get-home-stay-detail")]
        public async Task<IActionResult> GetHomeStayDetail([FromQuery] Guid homeStayID)
        {
            var getDetail = await _homeStayRepository
                .FindWithInclude(h => h.Calendars)
                .Include(h => h.HomestayAmenities!)
                .ThenInclude(ha => ha.Amenity)
                .Include(hs => hs.HomestayImages!)
                .Include(h => h.HomestayFacilities!)
                .ThenInclude(h => h.Facility)
                .Include(x => x.FeedBacks)
                .ThenInclude(x => x.User)
                .FirstOrDefaultAsync(h => h.Id == homeStayID);

            if (getDetail == null)
            {
                return NotFound();
            }

            var response = new
            {
                getDetail.Id,
                getDetail.Name,
                getDetail.MainImage,
                getDetail.Address,
                getDetail.City,
                getDetail.CheckInTime,
                getDetail.CheckOutTime,
                getDetail.OpenIn,
                getDetail.Description,
                getDetail.Standar,
                getDetail.isDeleted,
                Calendar = _calendarRepository
                    .FindWithInclude(c => c.HomeStay)
                    .Where(c => c.HomeStay.Id == homeStayID)
                    .Select(c => new
                    {
                        c.Id,
                        c.Date,
                        c.Price,
                        c.isDeleted,
                        c.isBooked
                    }).ToList(),
                HomeStayImage = getDetail.HomestayImages!.Select(image => new
                {
                    Image = image.Image,
                }),
                Amenities = getDetail.HomestayAmenities!.Select(ha => new
                {
                    ha.Amenity.Id,
                    ha.Amenity.Name
                }).ToList(),
                Facility = getDetail.HomestayFacilities!.Select(hf => new
                {
                    hf.FacilityID,
                    hf.Facility.Name,
                    hf.Facility.Description
                }).ToList(),
                Feeback = getDetail.FeedBacks!.Select(fb => new
                {
                    fb.User.FullName,
                    fb.User.Avatar,
                    fb.User.Email,
                    fb.Rating,
                    fb.Description,
                    fb.IsReply
                }).ToList()
            };
            return Ok(response);
        }

        [HttpGet("filter-home-stay-with-status")]
        public async Task<IActionResult> FilterHomeStayWithStatus([FromQuery] bool status)
        {
            var filter = await _homeStayRepository.Find(x => x.isDeleted == status).ToListAsync();
            if (filter.Any())
            {
                return Ok(filter);

            }
            return NotFound();
        }

        [HttpPost("add-home-stay-image")]
        public async Task<IActionResult> AddHomeStayImage([FromBody] HomeStayImageDTO request)
        {
            try
            {
                if (request == null) return BadRequest();
                var getHomeStay = await _homeStayRepository.GetByIdAsync(request.HomeStayID);
                foreach (var item in request.Images)
                {

                    Guid imageID = Guid.NewGuid();
                    HomeStayImage addImage = new HomeStayImage
                    {
                        Id = imageID,
                        Image = item,
                        HomeStay = getHomeStay,
                        isDeleted = false
                    };
                    await _homeStayImageRepository.AddAsync(addImage);
                }
                await _homeStayImageRepository.SaveAsync();
                return Ok(new { Message = "Add Image Success" });
            }
            catch (Exception ex) {
                return StatusCode(500, ex.ToString());
            }
        }

        [HttpGet("get-home-stay-image")]
        public async Task<IActionResult> GetHomeStayImage([FromQuery] Guid homeStayID)
        {
            try
            {
                var listImage = await _homeStayImageRepository.FindWithInclude(h => h.HomeStay)
                                          .Where(h => h.HomeStayID == homeStayID)
                                          .ToListAsync();
                if (listImage.Any())
                {
                    var response = listImage.Select(image => new
                    {
                        image.Id,
                        image.Image,

                    }).ToList();
                    return Ok(response);
                }
                return NotFound();
            }
            catch (Exception ex) {
                return StatusCode(500, ex.ToString());
            }
        }

        [HttpDelete("delete-home-stay-image")]
        public async Task<IActionResult> DeleteHomeStayImage([FromBody] DeleteHomeStayImageDTO request)
        {
            try
            {
                if (request == null || request.ImageIds == null || !request.ImageIds.Any())
                {
                    return BadRequest(new { Message = "Invalid request data." });
                }
                var imagesToDelete = await _homeStayImageRepository
                    .Find(h => request.ImageIds.Contains(h.Id))
                    .ToListAsync();

                if (!imagesToDelete.Any())
                {
                    return NotFound(new { Message = "No matching images found." });
                }

                _homeStayImageRepository.DeleteRange(imagesToDelete);
                await _homeStayImageRepository.SaveAsync();

                return Ok(new { Message = "Images deleted successfully" });
            }
            catch (Exception ex) {
                return StatusCode(500, ex.ToString());
            }
        }


        [HttpGet("get-city-list")]
        public async Task<IActionResult> GetAllCity()
        {
            var homeStayList = await _homeStayRepository.GetAllAsync();
            var city = homeStayList.Select(c => c.City).Distinct().ToList();
            return Ok(city);

        }

        [HttpGet("search-by-city")]
        public async Task<IActionResult> SearchByCity([FromQuery]string city)
        {
            var getHomeStay = await _homeStayRepository
                                    .FindWithInclude(h => h.Calendars!)
                                    .Include(h => h.HomestayAmenities!)
                                    .ThenInclude(ha => ha.Amenity)
                                    .Include(f => f.HomestayFacilities!)
                                    .ThenInclude(hf => hf.Facility)
                                    .Include(f => f.FeedBacks)
                                    .Where(x => x.City.Equals(city) && x.isDeleted == false).ToListAsync();
            var response = getHomeStay.Select(h => new
            {
                h.Id,
                h.Name,
                h.MainImage,
                h.Address,
                h.City,
                h.CheckInTime,
                h.CheckOutTime,
                h.OpenIn,
                h.Description,
                h.Standar,
                h.isDeleted,


                Calendar = h.Calendars!.Select(c => new
                {
                    c.Id,
                    c.Date,
                    c.Price,
                    c.isBooked
                }).ToList(),

                Amenities = h.HomestayAmenities!
                 .Select(ha => new
                 {
                     ha.Amenity.Id,
                     ha.Amenity.Name
                 }).ToList(),
                Facility = h.HomestayFacilities!.Select(hf => new
                {
                    hf.FacilityID,
                    hf.Facility.Name,
                    hf.Facility.Description
                }).ToList()
            }).ToList();

            return Ok(response);
        }

        [HttpGet("get-home-stay-by-user")]
        public async Task<IActionResult> GetHomeStayByUser([FromQuery] Guid userID, [FromQuery] FilterDTO filter)
        {
            var query = _homeStayRepository
                .FindWithInclude(h => h.Calendars!)
                .Include(h => h.HomestayAmenities!)
                .ThenInclude(ha => ha.Amenity)
                .Include(hf => hf.HomestayFacilities)
                .ThenInclude(fa => fa.Facility)
                .Include(t => t.TTlockAccuonts)
                .Where(u => u.UserID == userID)
                .AsQueryable();

            if (filter.Standard is { Count: > 0 })
            {
                query = query.Where(h => filter.Standard.Contains(h.Standar));
            }

            if (filter.AmenityNames is { Count: > 0 })
            {
                query = query.Where(h => h.HomestayAmenities!.Any(ha => filter.AmenityNames.Contains(ha.Amenity.Name)));
            }

            if (filter.MinPrice.HasValue || filter.MaxPrice.HasValue)
            {
                query = query.Where(h => h.Calendars!.Any(c =>
                    (!filter.MinPrice.HasValue || c.Price >= filter.MinPrice.Value) &&
                    (!filter.MaxPrice.HasValue || c.Price <= filter.MaxPrice.Value)
                ));
            }

            if (filter.SearchText != null)
            {
                query = query.Where(x => x.Name.Contains(filter.SearchText));
            }

            var listHomeStay = await query.ToListAsync();

            if (!listHomeStay.Any())
            {
                return NotFound();
            }

            var response = listHomeStay.Select(h => new
            {
                h.Id,
                h.Name, 
                h.MainImage,
                h.Address,
                h.City,
                h.CheckInTime,
                h.CheckOutTime,
                h.OpenIn,
                h.Description,
                h.Standar,
                h.isDeleted,
                Calendar = h.Calendars!.Select(c => new { c.Id, c.Date, c.Price, c.isBooked }).ToList(),
                Amenities = h.HomestayAmenities!.Select(ha => new { ha.Amenity.Id, ha.Amenity.Name }).ToList(),
                Facility = h.HomestayFacilities!.Select(hf => new { hf.FacilityID, hf.Facility.Name, hf.Facility.Description }).ToList(),
                TTlockAccuont = h.TTlockAccuonts.Select(ta => new { ta.TTLockID, ta.TTLockUserName, ta.Password })
            }).ToList();

            return Ok(response);
        }


        [HttpGet("search-home-stay")]
        public async Task<IActionResult> SearchHomeStay([FromQuery]SearchHomeStayDTO request)
        {
            var homeStays = await _homeStayRepository
                .FindWithInclude()
                .Include(h => h.Calendars!)
                    .ThenInclude(c => c.Booking)
                .Include(h => h.HomestayAmenities!)
                    .ThenInclude(ha => ha.Amenity)
                .Include(h => h.HomestayFacilities!)
                    .ThenInclude(fa => fa.Facility)
                .Where(h => h.Calendars.Any(c =>
                    c.isBooked == false &&
                    c.Date >= request.CheckInDate &&
                    c.Date <= request.CheckOutDate
                    && c.HomeStay.City == request.City
                ))
                .ToListAsync();

            var response = homeStays.Select(h => new
            {
                h.Id,
                h.Name,
                h.MainImage,
                h.Address,
                h.City,
                h.CheckInTime,
                h.CheckOutTime,
                h.OpenIn,
                h.Description,
                h.Standar,
                h.isDeleted,

                Calendar = h.Calendars!.Select(c => new
                {
                    c.Id,
                    c.Date,
                    c.Price,
                    c.isBooked
                }).ToList(),

                Amenities = h.HomestayAmenities!
                   .Select(ha => new
                   {
                       ha.Amenity.Id,
                       ha.Amenity.Name
                   }).ToList(),
                Facility = h.HomestayFacilities!.Select(hf => new
                {
                    hf.FacilityID,
                    hf.Facility.Name,
                    hf.Facility.Description
                }).ToList()
            }).Where(x => x.isDeleted == false).ToList();

            return Ok(response);

        }

    }
}

