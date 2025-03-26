using BusinessObject.Entities;
using BusinessObject.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RefundsController(IRepository<Refunds> _refundsRepository) : Controller
    {
        [HttpGet("get-refunds")]
        public async Task<IActionResult> GetAll()
        {
            var result = _refundsRepository.FindWithInclude()
                                           .Include(x => x.Transaction)
                                           .ThenInclude(x => x.Booking)
                                           .ThenInclude(u => u.User)
                                           .Where(x => x.Transaction.Booking.Status.Equals("Canceled"))
                                           .Select(s => new
                                           {
                                               s.RefundID,
                                               s.TransactionID,
                                               RefundStatus = s.Status,
                                               s.Transaction.Amount,
                                               CreatedDate = s.Transaction.Date,
                                               Booking = new
                                               {
                                                   s.Transaction.Booking.CheckInDate,
                                                   s.Transaction.Booking.CheckOutDate,
                                                   s.Transaction.Booking.TotalPrice,

                                               },
                                               User = new
                                               {
                                                   FullName = s.Transaction.Booking.User.FullName,
                                                   PhoneNumber = s.Transaction.Booking.User.Phone,
                                                   Email = s.Transaction.Booking.User.Email,
                                                   Address = s.Transaction.Booking.User.Address,
                                                   Avatar = s.Transaction.Booking.User.Avatar
                                               }


                                           }).ToList();
            return Ok(result);
        }
        [HttpPut("change-status")]
        public async Task<IActionResult> ChangeStatusRefund([FromQuery] long refundID)
        {
            var getRefund = await _refundsRepository.GetByIdAsync(refundID);
            if (getRefund == null) { 
                
                return NotFound();
            }

            getRefund.Status = true;
            await _refundsRepository.UpdateAsync(getRefund);
            await _refundsRepository.SaveAsync();
            return Ok("Change Status Success");
        }
    }
}
