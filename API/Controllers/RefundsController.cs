using BusinessObject.Entities;
using BusinessObject.Interfaces;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RefundsController(IRepository<Refunds> _refundsRepository, IEmailSender _emailSender) : Controller
    {
        [HttpGet("get-refunds")]
        public async Task<IActionResult> GetAll([FromQuery]string homeStayName)
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
                                                   s.Transaction.Booking.HomeStayName,
                                                   s.Transaction.Booking.HomeStayAddress,
                                                   s.Transaction.Booking.CheckInDate,
                                                   s.Transaction.Booking.CheckOutDate,
                                                   s.Transaction.Booking.TotalPrice,
                                                   s.Transaction.Booking.Status,
                                               },
                                               User = new
                                               {
                                                   FullName = s.Transaction.Booking.User.FullName,
                                                   PhoneNumber = s.Transaction.Booking.User.Phone,
                                                   Email = s.Transaction.Booking.User.Email,
                                                   Address = s.Transaction.Booking.User.Address,
                                                   Avatar = s.Transaction.Booking.User.Avatar
                                               }


                                           }).Where(x => x.Booking.HomeStayName.Equals(homeStayName)).ToList();
            return Ok(result);
        }
        [HttpPut("change-status")]
        public async Task<IActionResult> ChangeStatusRefund([FromQuery] long refundID)
        {
            var getRefund = await _refundsRepository.FindWithInclude()
                                                    .Include(x => x.Transaction)
                                                     .ThenInclude(x => x.Booking)
                                                     .ThenInclude(x => x.User)
                                                    .FirstOrDefaultAsync(x => x.RefundID == refundID);
            if (getRefund == null) {    
                
                return NotFound();
            }
            string subject = "[Thông Báo Hoàn Tiền]";
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
                                            <h2>Hoàn tiền thành công</h2>
                                          </div>
                                          <div class='content'>
                                            <p>Xin chào <span class='highlight'>{getRefund.Transaction.Booking.User.FullName}</span>,</p>
                                            <p>Chúng tôi xin thông báo rằng yêu cầu hoàn tiền của bạn đã được xử lý thành công.</p>
                                            <p>Thông tin chi tiết đơn đặt phòng:</p>
                                            <ul>
                                              <li><strong>Homestay:</strong> {getRefund.Transaction.Booking.HomeStayName}</li>
                                              <li><strong>Địa chỉ:</strong> {getRefund.Transaction.Booking.HomeStayAddress}</li>
                                              <li><strong>Ngày nhận phòng:</strong> {getRefund.Transaction.Booking.CheckInDate:dd/MM/yyyy}</li>
                                              <li><strong>Ngày trả phòng:</strong> {getRefund.Transaction.Booking.CheckOutDate:dd/MM/yyyy}</li>
                                              <li><strong>Giá trị hoàn tiền:</strong> {getRefund.Transaction.Booking.TotalPrice:#,##0} VND</li>
                                              <li><strong>Trạng thái đặt phòng:</strong> {getRefund.Transaction.Booking.Status}</li>
                                            </ul>
                                            <p>Chúng tôi hy vọng sẽ được phục vụ bạn vào những lần sau.</p>
                                            <p>Nếu bạn có bất kỳ câu hỏi nào, xin đừng ngần ngại liên hệ với chúng tôi.</p>
                                            <p>Trân trọng,<br/><strong>Đội ngũ {getRefund.Transaction.Booking.HomeStayName}</strong></p>
                                          </div>
                                          <div class='footer'>
                                            &copy; {DateTime.Now.Year} {getRefund.Transaction.Booking.HomeStayName}. Mọi quyền được bảo lưu.
                                          </div>
                                        </div>
                                      </body>
                                    </html>";

            await _emailSender.SendEmailAsync(getRefund.Transaction.Booking.User.Email, subject, emailBodyHtml);
            getRefund.Status = true;
            await _refundsRepository.UpdateAsync(getRefund);
            await _refundsRepository.SaveAsync();
            return Ok("Change Status Success");
        }
    }
}
