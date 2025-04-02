using BusinessObject.Entities;
using BusinessObject.Exceptions;
using BusinessObject.Interfaces;
using BusinessObject.Shares;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using PayOSService.Config;
using PayOSService.DTO;
using PayOSService.Services;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController(IRepository<Booking> _bookingRepository,
                                   IRepository<Transaction> _transactionRepository,
                                   IRepository<Calendar> _calendarRepository,
                                   IEmailSender _emailSender,
                                   IPayOSService _payOSService,
                                   IOptions<PayOSConfig> payosConfigOptions) : ControllerBase
    {
        private readonly PayOSConfig _payOSConfig = payosConfigOptions.Value;

        [HttpPost]
        public async Task<IActionResult> CreatePayment(Guid bookingID)
        {
            string paymentLink = "";
            var booking = await _bookingRepository.GetByIdAsync(bookingID) ?? throw new NotFoundException("Booking not found");

            var transactions = await _transactionRepository.FindAsync(c =>
                c.BookingID == bookingID);

            var transaction = transactions.FirstOrDefault();

            if (transaction != null)
            {
                if (!string.IsNullOrEmpty(transaction.PaymentLink)) return Ok(new
                {
                    paymentLink = transaction.PaymentLink
                });
            }

            var code = IdUtility.GetNewID();

            paymentLink = await _payOSService.CreatePaymentAsync(new CreatePaymentDTO()
            {
                OrderCode = code,
                Content = "Thanh toan",
                RequiredAmount = (int)booking.TotalPrice,
            });

            await _transactionRepository.AddAsync(new Transaction()
            {
                ID = code,
                BookingID = bookingID,
                PaymentLink = paymentLink,
                Amount = booking.TotalPrice,
                Date = DateTime.UtcNow
            });

            await _transactionRepository.SaveAsync();

            return Ok(new
            {
                paymentLink
            });
        }

        [HttpGet("payment-return")]
        public async Task<IActionResult> PaymentReturn(
        [FromQuery] string code,
        [FromQuery] string id,
        [FromQuery] string cancel,
        [FromQuery] string status,
        [FromQuery] string orderCode)
        {
            long orderId = long.Parse(orderCode);
            var transaction = await _transactionRepository.GetByIdAsync(orderId) ?? throw new NotFoundException("Transaction not found");
            var booking = await _bookingRepository.GetByIdAsync(transaction.BookingID) ?? throw new NotFoundException("Booking not found");
            var getUser = await _bookingRepository.FindWithInclude()
                                                  .Include(x => x.User)
                                                  .FirstOrDefaultAsync(x => x.UserID == booking.UserID);
            var homeStay = await _bookingRepository.FindWithInclude()
                                                      .Include(x => x.Calendars)
                                                      .ThenInclude(x => x.HomeStay)
                                                      .Where(x => x.Id == booking.Id)
                                                      .Select(x => x.Calendars.Select(c => c.HomeStay).FirstOrDefault())
                                                      .FirstOrDefaultAsync();
            if (code == "00" && status == "PAID")
            {
                booking.Status = "Paid";
                await _emailSender.SendEmailAsync(getUser.User.Email, "Bạn đã đặt phòng thành công", SendMailBooking(homeStay, booking));
            }

            else
            {
                var relatedCalendars = await _calendarRepository
                .FindWithInclude()
                .Where(c => c.BookingID == booking.Id)
                .ToListAsync();
                foreach (var calendar in relatedCalendars)
                {
                    calendar.BookingID = null;
                    calendar.isBooked = false;
                    await _calendarRepository.UpdateAsync(calendar);
                }
                await _emailSender.SendEmailAsync(getUser.User.Email, "Thao Tác Đặt Phòng Thất Bại", SendMailBooking(homeStay, booking));

                await _calendarRepository.SaveAsync();
                booking.Status = "Payment Cancelled";
                
            }

            await _bookingRepository.UpdateAsync(booking);
            await _bookingRepository.SaveAsync();

            return Ok(new { success = true, redirectUrl = _payOSConfig.ReturnUrl });
        }

        public static String SendMailBooking(HomeStay homeStay, Booking booking)
        {

            return
            $@"
            <!DOCTYPE html>
            <html lang=""en"">
   <html lang=""en"">
            <head>
                <meta charset=""UTF-8"">
                <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
                <title>Booking Confirmation</title>
                <style>
                    body {{
                        font-family: Arial, sans-serif;
                        background-color: #f9f9f9;
                        margin: 0;
                        padding: 0;
                    }}
                    .container {{
                        background-color: #FFFAF0;
                        padding: 20px;
                        border-radius: 8px;
                        box-shadow: 0 0 10px rgba(0,0,0,0.1);
                        width: 100%;
                        max-width: 600px;
                        box-sizing: border-box;
                        margin: auto;
                    }}
                    .hotel-info {{
                        text-align: center;
                        margin-bottom: 20px;
                    }}
                    .hotel-image {{
                        width: 100%;
                        height: auto;
                        border-radius: 8px;
                    }}
                    .room-type {{
                        font-size: 18px;
                        font-weight: bold;
                    }}
                    .details, .cancellation {{
                        font-size: 14px;
                        color: #666;
                    }}
                    .cancellation span {{
                        color: #d9534f;
                    }}
                    .form-group {{
                        margin-bottom: 15px;
                    }}
                    .form-group label {{
                        display: block;
                        margin-bottom: 5px;
                    }}
                    .form-group input, .form-group select, .form-group button {{
                        width: 100%;
                        padding: 10px;
                        border: 1px solid #ccc;
                        border-radius: 4px;
                        box-sizing: border-box;
                    }}
                    .price-detail {{
                        text-align: right;
                        font-size: 14px;
                        color: #666;
                    }}
         
                    @media (max-width: 768px) {{
                        .price-detail {{
                            text-align: left;
                        }}
                    }}
                </style>
            </head>
            <body>
                <div class=""container"">
                    <div class=""hotel-info"">
                        <img src=""{homeStay.MainImage}"" alt=""Hotel Image"" class=""hotel-image"">
                        <h2>{homeStay.Name} - {homeStay.Address}</h2>
                    </div>
                    <div class=""form-group"">
                        <label for=""checkin"">Check-in</label>
                        <input type=""date"" id=""checkin"" name=""checkin"" value=""{booking.CheckInDate:yyyy-MM-dd}"" disabled readonly required>
                    </div>
                   <div class=""form-group"">
                        <label for=""checkin"">Check-Out</label>
                        <input type=""date"" id=""checkin"" name=""checkin"" value=""{booking.CheckOutDate:yyyy-MM-dd}"" disabled readonly required>
                    </div>
                    <div class=""form-group"">
                        <label for=""fullname"">Fullname</label>
                        <input type=""text"" id=""fullname"" name=""fullname"" value=""{booking.User.FullName}"" disabled readonly  required>
                    </div>
                    <div class=""form-group"">
                        <label for=""email"">Email</label>
                        <input type=""email"" id=""email"" name=""email"" value=""{booking.User.Email}"" disabled readonly required>
                    </div>
                    <div class=""form-group"">
                        <label for=""phone"">Phone</label>  
                        <input type=""tel"" id=""phone"" name=""phone"" value=""{booking.User.Phone}"" disabled readonly required>
                    </div>
                    <div class=""price-detail"">
                        <p>Check-In-Date: <span>{booking.CheckInDate.ToString("dd/MM/yyy")} $</span></p>
                        <p>Check-Out-Date: <span>{booking.CheckOutDate.ToString("dd/MM/yyy")} $</span></p>
                        <p>Total: <span>{booking.TotalPrice} $</span></p>
                        <p>Total: <span>{booking.Status} $</span></p>
                    </div>
                </div>
            </body>
            </html>";
        }

    }
}
    