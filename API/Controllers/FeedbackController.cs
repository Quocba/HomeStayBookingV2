using BusinessObject.DTO;
using BusinessObject.Entities;
using BusinessObject.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FeedbackController : ControllerBase
    {
        private readonly IRepository<FeedBack> _feedbackRepository;
        private readonly IRepository<HomeStay> _homeStayRepository;
        private readonly IEmailSender _emailSender;
        public FeedbackController(IRepository<FeedBack> feedbackRepository, IRepository<HomeStay> homestayRepository, IEmailSender emailSender)
        {
            _feedbackRepository = feedbackRepository;
            _homeStayRepository = homestayRepository;
            _emailSender = emailSender;
        }

        [HttpPost]
        public async Task<IActionResult> CreateFeedback(
            [FromHeader(Name = "X-User-Id")] Guid userId,
            [FromBody] FeedbackDTO feedbackDto)
        {
            if (userId == Guid.Empty) return BadRequest("User not found");
            try
            {
                var feedback = new FeedBack
                {
                    Id = Guid.NewGuid(),
                    UserID = userId,
                    HomeStay = await _homeStayRepository.GetByIdAsync(feedbackDto.HomestayID),
                    Rating = feedbackDto.Rating,
                    Description = feedbackDto.Description,
                    isDeleted = false
                };

                await _feedbackRepository.AddAsync(feedback);
                await _feedbackRepository.SaveAsync();

                return Ok(feedback);
            }
            catch (Exception ex) {
                return StatusCode(500, ex.ToString());
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> EditFeedback(
            Guid id,
            [FromHeader(Name = "X-User-Id")] Guid userId,
            [FromBody] FeedbackDTO feedbackDto)
        {
            try
            {
                if (userId == Guid.Empty || feedbackDto == null) return BadRequest();

                var feedback = await _feedbackRepository.GetByIdAsync(id);
                if (feedback == null || feedback.isDeleted)
                {
                    return NotFound("Feedback not found.");
                }

                if (feedback.UserID != userId)
                {
                    return Forbid("You can only edit your own feedback.");
                }


                if (feedbackDto.Rating < 1 || feedbackDto.Rating > 5)
                {
                    return BadRequest("Rating must be between 1 and 5.");
                }

                feedback.Rating = feedbackDto.Rating;
                feedback.Description = feedbackDto.Description;

                await _feedbackRepository.UpdateAsync(feedback);
                await _feedbackRepository.SaveAsync();

                return Ok(feedback);
            }
            catch (Exception ex) { 
                return StatusCode(500, ex.ToString());
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFeedback(
            Guid id,
            [FromHeader(Name = "X-User-Id")] Guid userId)
        {
            try
            {
                if (userId == Guid.Empty) return NotFound();
                var feedback = await _feedbackRepository.GetByIdAsync(id);
                if (feedback == null || feedback.isDeleted)
                {
                    return NotFound(new
                    {
                        Message = "Feedback not found."
                    });
                }

                if (feedback.UserID != userId)
                {
                    return Forbid();
                }

                feedback.isDeleted = true;

                await _feedbackRepository.UpdateAsync(feedback);
                await _feedbackRepository.SaveAsync();

                return Ok(feedback);
            }
            catch (Exception ex) {

                return StatusCode(500, ex.ToString());
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetFeedbackById(Guid id)
        {
            try
            {
                if (id == Guid.Empty) return BadRequest();
                var feedback = await _feedbackRepository.GetByIdAsync(id);
                if (feedback == null || feedback.isDeleted)
                {
                    return NotFound("Feedback not found.");
                }
                return Ok(feedback);
            }
            catch (Exception ex) { 
                return StatusCode(500, ex?.ToString());
            }
        }

        [HttpGet("get-feedback-by-home-stay")]
        public async Task<IActionResult> GetFeedBackByHomeStay([FromQuery]Guid homeStayID)
        {
            var getFeedbacks = await _feedbackRepository.FindWithInclude()
                                                        .Include(x => x.HomeStay)
                                                        .Include(u => u.User)
                                                        .Where(x => x.HomeStay.Id == homeStayID)
                                                        .ToListAsync();

            var response = getFeedbacks.Select(feedback => new
            {
                feedback.Id,
                feedback.Rating,
                feedback.Description,
                User = new
                {
                    feedback.User.Email,
                    feedback.User.FullName,
                    feedback.User.Avatar,
                    feedback.User.Phone,
                    feedback.User.Address
                },
                homeStayID = feedback.HomeStay.Id
            }).ToList();

            return Ok(response);
        }

        [HttpPost("reply-feedback-by-user-email")]
        public async Task<IActionResult> SendMailFeedBackByUserEmail([FromBody]ReplyFeedBackDTO request)
        {
            var getFeedBack = await _feedbackRepository.FindWithInclude()
                                                       .Include(x => x.User)
                                                       .Include(h => h.HomeStay)
                                                       .FirstOrDefaultAsync(x => x.Id == request.FeedBackID);

            var getHomeStay = await _homeStayRepository.GetByIdAsync(getFeedBack.HomeStay.Id);
            string subject = $"[{request.Subject}]";
            string emailBodyHtml = $@"
                                    <html>
                                      <head>
                                        <meta charset='UTF-8'>
                                        <style>
                                          body {{
                                            font-family: 'Segoe UI', Tahoma, sans-serif;
                                            background-color: #f8f9fa;
                                            margin: 0;
                                            padding: 0;
                                          }}
                                          .container {{
                                            max-width: 650px;
                                            margin: 50px auto;
                                            background-color: #ffffff;
                                            border-radius: 12px;
                                            overflow: hidden;
                                            box-shadow: 0 8px 24px rgba(0, 0, 0, 0.1);
                                          }}
                                          .header {{
                                            background-color: #1abc9c;
                                            color: white;
                                            padding: 30px 40px;
                                            text-align: center;
                                          }}
                                          .header h2 {{
                                            margin: 0;
                                            font-size: 24px;
                                          }}
                                          .content {{
                                            padding: 30px 40px;
                                            color: #343a40;
                                            line-height: 1.8;
                                          }}
                                          .content p {{
                                            margin: 10px 0;
                                            font-size: 16px;
                                          }}
                                          .highlight {{
                                            font-weight: 600;
                                            color: #1abc9c;
                                          }}
                                          .blockquote {{
                                            background-color: #f1f1f1;
                                            border-left: 5px solid #1abc9c;
                                            padding: 15px 20px;
                                            margin: 20px 0;
                                            font-style: italic;
                                            color: #555;
                                          }}
                                          .footer {{
                                            background-color: #f1f1f1;
                                            color: #777;
                                            font-size: 13px;
                                            text-align: center;
                                            padding: 20px;
                                          }}
                                        </style>
                                      </head>
                                      <body>
                                        <div class='container'>
                                          <div class='header'>
                                            <h2>{subject}</h2>
                                          </div>
                                          <div class='content'>
                                            <p>Xin chào <span class='highlight'>{getFeedBack.User.FullName}</span>,</p>
                                            <p>Chúng tôi đã nhận được phản hồi của bạn:</p>
                                            <div class='blockquote'>
                                              {getFeedBack.Description}
                                            </div>
                                            <p>Chân thành cảm ơn bạn đã dành thời gian đóng góp ý kiến cho <span class='highlight'>{getHomeStay.Name}</span>.</p>
                                            <p>Dưới đây là phản hồi từ chúng tôi:</p>
                                            <div class='blockquote'>
                                              {request.Message}
                                            </div>
                                            <p>Nếu bạn có bất kỳ câu hỏi hay thắc mắc nào, xin vui lòng liên hệ lại với chúng tôi.</p>
                                            <p>Trân trọng,<br/><strong>Đội ngũ {getHomeStay.Name}</strong></p>
                                          </div>
                                          <div class='footer'>
                                            &copy; {DateTime.Now.Year} {getHomeStay.Name}. Mọi quyền được bảo lưu.
                                          </div>
                                        </div>
                                      </body>
                                    </html>";
            await _emailSender.SendEmailAsync(getFeedBack.User.Email, subject, emailBodyHtml);

            return Ok(new { message = "Gửi phản hồi thành công đến người dùng." });
        }
    }
}
