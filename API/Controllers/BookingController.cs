using System;
using System.Net.Mail;
using System.Net;
using BusinessObject.DTO;
using BusinessObject.Entities;
using BusinessObject.Interfaces;
using BusinessObject.Shares;
using DataAccess.Repositories;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using PayOSService.Services;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BookingController(IBookingRepository _bookingRepository,
                                   IRepository<Voucher> _voucherRepository,
                                   IUserRepository _userRepository,
                                   IRepository<HomeStay> _homeStayRepository,
                                   IEmailSender _emailSender,
                                   IPayOSService _payOSService,
                                   IConfiguration _configuration,
                                   IRepository<Calendar> _calendarRepository) : ControllerBase
    {
        [HttpGet("history")]
        public async Task<IActionResult> GetBookingHistory([FromHeader(Name = "X-User-Id")] Guid userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                return NotFound(new { Message = "User not found" });

            var bookings = await _bookingRepository.GetHistory(userId);

            if (!bookings.Any())
                return Ok(new { Message = "No booking history found." });

            var bookingHistory = bookings.Select(b => new
            {
                BookingID = b.Id,
                CheckInDate = b.CheckInDate.ToString("dd/MM/yyyy HH:mm"),
                CheckOutDate = b.CheckOutDate.ToString("dd/MM/yyyy HH:mm"),
                TotalPrice = b.TotalPrice.ToString("C", new System.Globalization.CultureInfo("vi-VN")),
                Status = b.Status
            }).OrderByDescending(b => b.CheckInDate).ToList();

            return Ok(bookingHistory);
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateBooking(
      [FromHeader(Name = "X-User-Id")] Guid userId,
      [FromBody] BookingDTO bookingDTO)
        {
            if (bookingDTO == null || !bookingDTO.Calenders.Any())
                return BadRequest(new { Message = "Invalid booking data" });

            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                return NotFound(new { Message = "User not found" });

            var calendars = await _calendarRepository.FindAsync(c =>
                bookingDTO.Calenders.Select(bc => bc.CalenderID).Contains(c.Id));

            if (!calendars.Any())
                return BadRequest(new { Message = "No valid calendar slots found" });

            var homeStay = await _homeStayRepository.GetByIdAsync(calendars.First().HomeStayID);
            if (homeStay == null)
                return BadRequest(new { Message = "HomeStay not found" });

            if (calendars.Any(c => c.BookingID != null))
                return BadRequest(new { Message = "Some selected dates are already booked" });

            var sortedDates = calendars.Select(c => c.Date).OrderBy(d => d).ToList();
            var firstDate = sortedDates.First();
            var lastDate = sortedDates.Last();
            var replaceCheckInDate = homeStay.CheckInTime.Replace("PM", "").Replace("AM", "");
            var replaceCheckOutDate = homeStay.CheckInTime.Replace("PM", "").Replace("AM", "");

            DateTime checkInDate = firstDate.Date.Add(TimeSpan.Parse(replaceCheckInDate));
            DateTime checkOutDate = lastDate.AddDays(1).Date.Add(TimeSpan.Parse(replaceCheckOutDate));

            if (firstDate == lastDate)
                checkOutDate = firstDate.Date.AddDays(1).Add(TimeSpan.Parse(homeStay.CheckOutTime.Replace("PM", "").Replace("AM", "")));

            decimal totalPrice = calendars.Sum(c => c.Price);

            if (!string.IsNullOrEmpty(bookingDTO.VoucherCode))
            {
                var vouchers = await _voucherRepository.GetAllAsync();
                var voucher = vouchers.FirstOrDefault(v =>
                    v.Code == bookingDTO.VoucherCode && DateUtility.GetCurrentDateTime() >= v.StartDate && DateUtility.GetCurrentDateTime() <= v.EndDate);

                if (voucher == null)
                    return BadRequest(new { Message = "Invalid or expired voucher" });

                decimal discountAmount = totalPrice * ((decimal)voucher.Discount / 100);
                totalPrice -= discountAmount;
            }

            var booking = new Booking
            {
                Id = Guid.NewGuid(),
                CheckInDate = checkInDate,
                CheckOutDate = checkOutDate,
                TotalPrice = totalPrice,
                UnitPrice = totalPrice / (checkOutDate - checkInDate).Days,
                Status = "Pending",
                UserID = userId
            };

            foreach (var calendar in calendars)
            {
                calendar.BookingID = booking.Id;
                calendar.isBooked = true;
              await  _calendarRepository.UpdateAsync(calendar);
            }

            // After booking change isBooked in home stay table is true
            await _bookingRepository.AddAsync(booking);
            await _calendarRepository.SaveAsync();
            await _bookingRepository.SaveAsync();

            if (!bookingDTO.IsOnline)
            {
                string url = _configuration["Base:Url"] ?? string.Empty;
                string confirmUrl = $"{url}/api/booking/confirm?bookingId={booking.Id}";

                string emailContent = $@"
        <p>Xin chào {user.FullName},</p>
        <p>Chi tiết đặt phòng:</p>
        <ul>
            <li><b>Check-in:</b> {booking.CheckInDate:dd/MM/yyyy HH:mm}</li>
            <li><b>Check-out:</b> {booking.CheckOutDate:dd/MM/yyyy HH:mm}</li>
            <li><b>Tổng tiền:</b> {booking.TotalPrice.ToString("C", new System.Globalization.CultureInfo("vi-VN"))}</li>
        </ul>
        <br>Click vào link để xác nhận đặt phòng: <a href='{confirmUrl}'>Xác nhận</a><br>
        <p>Chúng tôi mong được phục vụ bạn trong tương lai!</p>";

                await _emailSender.SendEmailAsync(user.Email, "Confirm Booking", emailContent);
            }

            return Ok(new
            {
                Message = "Booking created successfully!",
                BookingID = booking.Id,
                IsOnline = bookingDTO.IsOnline,
                CheckInDate = checkInDate,
                CheckOutDate = checkOutDate
            });
        }

            [HttpGet("confirm")]
        public async Task<IActionResult> ConfirmBooking([FromQuery] Guid bookingId)
        {
            var booking = await _bookingRepository.GetByIdAsync(bookingId);
            var user = await _userRepository.GetByIdAsync(booking.UserID);
            var homeStay = await _bookingRepository.FindWithInclude()
                                                   .Include(x => x.Calendars)
                                                   .ThenInclude(x => x.HomeStay)
                                                   .Where(x => x.Id == bookingId)
                                                   .Select(x => x.Calendars.Select(c => c.HomeStay).FirstOrDefault())
                                                   .FirstOrDefaultAsync();


            booking.Status = "Confirmed";
            await _emailSender.SendEmailAsync(user.Email, "Bạn đã đặt phòng thành công", SendMailBooking(homeStay, booking));
            await _bookingRepository.SaveAsync();

            return Redirect(_configuration["Base:UrlClient"]!);
        }
        [HttpPost("cancel")]
        public async Task<IActionResult> CancelBooking([FromQuery] Guid bookingId)
        {
            var booking = await _bookingRepository.GetByIdAsync(bookingId);
            if (booking == null)
                return NotFound(new { Message = "Booking not found" });

            if (booking.Status == "Paid")
                return BadRequest(new { Message = "Cannot cancel a paid booking" });

            if (booking.Status == "Canceled")
                return BadRequest(new { Message = "Booking is already canceled" });

            if ((booking.CheckInDate - DateTime.UtcNow).TotalDays < 1)
                return BadRequest(new { Message = "Cannot cancel less than 1 day before check-in" });

            var user = await _userRepository.GetByIdAsync(booking.UserID);
            if (user == null)
                return NotFound(new { Message = "User not found" });

            string baseUrl = _configuration["Base:Url"] ?? string.Empty;
            string confirmCancelUrl = $"{baseUrl}/api/booking/confirm-cancel?bookingId={booking.Id}";

            string emailContent = $@"
    <p>Xin chào {user.FullName},</p>
    <p>Bạn đã yêu cầu hủy đặt phòng. Vui lòng nhấn vào link bên dưới để xác nhận:</p>
    <p><a href='{confirmCancelUrl}'>Xác nhận hủy đặt phòng</a></p>
        <p>Chi tiết đặt phòng:</p>
        <ul>
            <li><b>Check-in:</b> {booking.CheckInDate:dd/MM/yyyy HH:mm}</li>
            <li><b>Check-out:</b> {booking.CheckOutDate:dd/MM/yyyy HH:mm}</li>
            <li><b>Tổng tiền:</b> {booking.TotalPrice.ToString("C", new System.Globalization.CultureInfo("vi-VN"))}</li>
        </ul>
    <p>Nếu bạn không thực hiện yêu cầu này, vui lòng bỏ qua email.</p>";

            await _emailSender.SendEmailAsync(user.Email, "Confirm Booking Cancellation", emailContent);

            return Ok(new { Message = "Cancellation request sent. Please check your email to confirm." });
        }

        [HttpGet("confirm-cancel")]
        public async Task<IActionResult> ConfirmCancelBooking([FromQuery] Guid bookingId)
        {
            var booking = await _bookingRepository.GetByIdAsync(bookingId);
            if (booking == null)
                return NotFound(new { Message = "Booking not found" });

            if (booking.Status == "Canceled")
                return BadRequest(new { Message = "Booking is already canceled" });

            booking.Status = "Canceled";

            var calendars = await _calendarRepository.FindAsync(c => c.BookingID == bookingId);
            foreach (var calendar in calendars)
            {
                calendar.BookingID = null;
            }

            await _calendarRepository.SaveAsync();
            await _bookingRepository.SaveAsync();

            return Ok(new { Message = "Booking successfully canceled!" });
        }


        [HttpPut("confirm-booking-status")]
        public async Task<IActionResult> ConfirmBookingStatus([FromQuery] Guid bookingID)
        {
            var getBooking = await _bookingRepository.GetByIdAsync(bookingID);
            if (getBooking != null && getBooking.Status.Equals("Pending"))
            {

                getBooking.Status = "Paid";
                await _bookingRepository.UpdateAsync(getBooking);
                await _bookingRepository.SaveAsync();
                return Ok(new { Message = "Update Booking Status Success" });
            }

            return NotFound();
        }

        [HttpGet("statistics-revenue-home-stay")]
        public async Task<IActionResult> HomeStayRevenueStatistics([FromQuery] Guid homeStayID, [FromQuery] int year)
        {
            var calendars = await _calendarRepository
                .FindWithInclude(c => c.Booking)
                .Include(h => h.HomeStay)
                .Where(c => c.HomeStayID == homeStayID && c.Booking != null)
                .ToListAsync();

            var bookingList = calendars
                .Select(c => c.Booking)
                .Where(b => b.CheckInDate.Year == year && b.Status == "Paid")
                .Distinct()
                .ToList();

            var totalWithMonth = new Dictionary<int, (decimal TotalRevenue, int BookingCount)>();

            for (int i = 1; i <= 12; i++)
            {
                totalWithMonth[i] = (0, 0);
            }

            foreach (var booking in bookingList)
            {
                var checkInMonth = booking.CheckInDate.Month;
                var currentData = totalWithMonth[checkInMonth];
                totalWithMonth[checkInMonth] = (currentData.TotalRevenue + booking.TotalPrice, currentData.BookingCount + 1);
            }

            var monthNames = new[]
            {
                "Jan", "Feb", "Mar", "Apr", "May", "Jun",
                "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"
             };

            var result = totalWithMonth.Select(entry => new
            {
                Month = monthNames[entry.Key - 1],
                Booking = entry.Value.BookingCount,
                Revenue = entry.Value.TotalRevenue
            }).ToList();

            return Ok(result);
        }


        [HttpGet("export")]
        public async Task<IActionResult> ExportBookingByHomeStayID([FromQuery] Guid homeStayID)
        {
            var calendars = await _calendarRepository
                .FindWithInclude(c => c.Booking, c => c.HomeStay) 
                .Where(c => c.HomeStayID == homeStayID && c.Booking != null)
                .ToListAsync();

            var bookings = calendars
                .Select(c => new
                {
                    c.Booking.Id,
                    c.Booking.CheckInDate,
                    c.Booking.CheckOutDate,
                    c.Booking.UnitPrice,
                    c.Booking.TotalPrice,
                    c.Booking.Status,
                    c.Booking.ReasonCancel,
                    HomeStayName = c.HomeStay.Name
                })
                .ToList();

            if (!bookings.Any())
            {
                return NotFound(new { Message = "No bookings found for this homestay." });
            }

            ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
            using var pck = new ExcelPackage();
            var ws = pck.Workbook.Worksheets.Add("Booking List");

            // Header row
            string[] headers = {
            "Booking ID", "Check-in Date", "Check-out Date", "Unit Price", "Total Price", "Status",
            "Cancellation Reason", "HomeStay Name"
        };

            for (int i = 0; i < headers.Length; i++)
            {
                ws.Cells[1, i + 1].Value = headers[i];
            }

            ws.Cells[1, 1, 1, headers.Length].Style.Font.Bold = true;
            ws.Cells[1, 1, 1, headers.Length].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            ws.Cells[1, 1, 1, headers.Length].Style.Fill.PatternType = ExcelFillStyle.Solid;
            ws.Cells[1, 1, 1, headers.Length].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);

            // Add data rows
            int row = 2;
            foreach (var booking in bookings)
            {
                ws.Cells[row, 1].Value = booking.Id.ToString();
                ws.Cells[row, 2].Value = booking.CheckInDate.ToString("dd-MM-yyyy");
                ws.Cells[row, 3].Value = booking.CheckOutDate.ToString("dd-MM-yyyy");
                ws.Cells[row, 4].Value = $"{booking.UnitPrice} VND";
                ws.Cells[row, 5].Value = $"{booking.TotalPrice} VND";
                ws.Cells[row, 6].Value = booking.Status;
                ws.Cells[row, 7].Value = booking.ReasonCancel;
                ws.Cells[row, 8].Value = booking.HomeStayName;
                row++;
            }

            ws.Cells.AutoFitColumns();

            // Save to memory stream
            var stream = new MemoryStream();
            pck.SaveAs(stream);
            stream.Position = 0; // 🔹 Đảm bảo stream bắt đầu từ đầu

            var fileName = $"BookingList_{homeStayID}.xlsx";
            return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }

        [HttpGet("analyze-revenue-system-home-stay")]
        public async Task<IActionResult> AnalyzeRevenueSystemHomeStay()
        {
            var revenueList =  (_bookingRepository.FindWithInclude(b => b.Calendars))
                .SelectMany(b => b.Calendars, (b, c) => new { b, c.HomeStay })
                .GroupBy(h => new { h.HomeStay.Id, h.HomeStay.Name, h.HomeStay.MainImage, h.HomeStay.Address })
                .Select(g => new
                {
                    HomeStayID = g.Key.Id,
                    MainImage = g.Key.MainImage,
                    HomeStayName = g.Key.Name,
                    Address = g.Key.Address,
                    TotalRevenue = g.Sum(x => x.b.TotalPrice)
                })
                .ToList();

            return Ok(revenueList);
        }

        [HttpGet("get-booking-by-home-stay")]
        public async Task<IActionResult> GetBookingByHomeStay([FromQuery] Guid homeStayID)
        {
            var listBooking = await _bookingRepository.FindWithInclude()
                                                .Include(x => x.Calendars)
                                                .ThenInclude(x => x.HomeStay)
                                                .Include(u => u.User)
                                                .Where(h => h.Calendars.Any(x => x.HomeStayID == homeStayID))
                                                .ToListAsync();
            var response = listBooking.Select(booking => new
            {
                booking.Id,
                booking.CheckInDate,
                booking.CheckOutDate,
                booking.TotalPrice,
                booking.UnitPrice,
                booking.Status,
                booking.ReasonCancel,
                booking.isDeleted,
                User = new 
                {
                    FullName = booking.User.FullName,
                    Email = booking.User.Email,
                    Phone = booking.User.Phone,
                    Address = booking.User.Address,
                    Avatar = booking.User.Avatar,
                    Gender = booking.User.Gender,
                    CitizenID = booking.User.CitizenID
                }
            }).ToList();
            return Ok(response);
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
                    </div>
                </div>
            </body>
            </html>";
        }
    }
}

