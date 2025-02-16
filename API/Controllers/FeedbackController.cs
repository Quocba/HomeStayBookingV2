using BusinessObject.DTO;
using BusinessObject.Entities;
using BusinessObject.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FeedbackController : ControllerBase
    {
        private readonly IRepository<FeedBack> _feedbackRepository;
        private readonly IRepository<HomeStay> _homeStayRepository;

        public FeedbackController(IRepository<FeedBack> feedbackRepository, IRepository<HomeStay> homestayRepository)
        {
            _feedbackRepository = feedbackRepository;
            _homeStayRepository = homestayRepository;
        }

        [HttpPost]
        public async Task<IActionResult> CreateFeedback(
            [FromHeader(Name = "X-User-Id")] Guid userId,
            [FromBody] FeedbackDTO feedbackDto)
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

        [HttpPut("{id}")]
        public async Task<IActionResult> EditFeedback(
            Guid id,
            [FromHeader(Name = "X-User-Id")] Guid userId,
            [FromBody] FeedbackDTO feedbackDto)
        {
            var feedback = await _feedbackRepository.GetByIdAsync(id);
            if (feedback == null || feedback.isDeleted)
            {
                return NotFound("Feedback not found.");
            }

            if (feedback.UserID != userId)
            {
                return Forbid("You can only edit your own feedback.");
            }

            feedback.Rating = feedbackDto.Rating;
            feedback.Description = feedbackDto.Description;

            await _feedbackRepository.UpdateAsync(feedback);
            await _feedbackRepository.SaveAsync();

            return Ok(feedback);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFeedback(
            Guid id,
            [FromHeader(Name = "X-User-Id")] Guid userId)
        {
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

        [HttpGet("{id}")]
        public async Task<IActionResult> GetFeedbackById(Guid id)
        {
            var feedback = await _feedbackRepository.GetByIdAsync(id);
            if (feedback == null || feedback.isDeleted)
            {
                return NotFound("Feedback not found.");
            }
            return Ok(feedback);
        }
    }

}
