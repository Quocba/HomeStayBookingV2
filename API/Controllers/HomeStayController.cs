using BusinessObject.DTO;
using BusinessObject.Entities;
using BusinessObject.Interfaces;
using BusinessObject.Shares;
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
        IWebHostEnvironment _eviroment) : ControllerBase
    {
        [HttpPost("add-home-stay")]
        public async Task<IActionResult> AddHomeStay([FromForm]AddHomeStayRequest request)
        {

            Guid userID = Guid.Parse("ddb60060-fd7f-482d-8c43-3d2534aafe19");
            var user = await _userRepository.GetByIdAsync(userID);
            var mainImageTask = Task.Run(() => Util.SaveImage(request.MainImage, _eviroment));
            Guid homeStayID = Guid.NewGuid();
                HomeStay createHomeStay = new HomeStay
                {
                    Id = homeStayID,
                    MainImage = await mainImageTask,
                    Name = request.Name,
                    Description = request.Description,
                    Address = request.Address,
                    CheckInTime = request.CheckInTime,
                    CheckOutTime = request.CheckOutTime,
                    isBooked = request.isBlocked,
                    isDeleted = request.isDeleted,
                    City = request.City,
                    OpenIn = request.OpenIn,
                    Standar = request.Standar,
                    User = user
                };
                await _homeStayRepository.AddAsync(createHomeStay);
                var calendarTask = _calendarRepository.AddAsync(new Calendar
                {
                    Date = request.Date,
                    Price = request.Price,
                    isDeleted = request.isDeleted,
                    Stay = createHomeStay
                });

            foreach (var image in request.Images)
                {
                    Guid imageID = new Guid();
                    HomeStayImage addImage = new HomeStayImage
                    {
                        Id = imageID,
                        Image = Util.SaveImage(image, _eviroment),
                        HomeStay = createHomeStay
                    };

                    await _homeStayImageRepository.AddAsync(addImage);
                }
                    await _homeStayImageRepository.SaveAsync();


            return Ok(new { message = "Add Home Stay Success." });
            }
  
        }
}

