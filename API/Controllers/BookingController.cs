using System;
using BusinessObject.DTO;
using BusinessObject.Entities;
using BusinessObject.Interfaces;
using DataAccess.Repositories;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using PayOSService.Services;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BookingController(IRepository<Booking> _bookingRepository,
                                   IRepository<Voucher> _voucherRepository,
                                   IUserRepository _userRepository,
                                   IRepository<HomeStay> _homeStayRepository,
                                   IEmailSender _emailSender,
                                   IPayOSService _payOSService) : ControllerBase
    {

        [HttpPut("confirm-booking-status")]
        public async Task<IActionResult> ConfirmBookingStatus([FromQuery] Guid bookingID)
        {
            var getBooking = await _bookingRepository.GetByIdAsync(bookingID);
            if (getBooking != null && getBooking.Status.Equals("Booked"))
            {

                getBooking.Status = "Payment Completed";
                await _bookingRepository.UpdateAsync(getBooking);
                await _bookingRepository.SaveAsync();
                return Ok(new { Message = "Update Booking Status Success" });
            }

            return NotFound();
        }

        //[HttpGet("statistics-revenue-home-stay")]
        //public async Task<IActionResult> HomeStayRevenueStatistics([FromQuery] Guid homeStayID, [FromQuery] int year)
        //{
        //    var bookingList = await _bookingRepository
        //        .FindAsync(b => b.HomeStay.Id == homeStayID && b.CheckInDate.Year == year && b.Status.Equals("Payment Completed"));

        //    var totalWithMonth = new Dictionary<int, (decimal TotalRevenue, int BookingCount)>();

        //    for (int i = 1; i <= 12; i++)
        //    {
        //        totalWithMonth[i] = (0, 0);
        //    }

        //    foreach (var booking in bookingList)
        //    {
        //        var checkInMonth = booking.CheckInDate.Month;
        //        var currentData = totalWithMonth[checkInMonth];
        //        totalWithMonth[checkInMonth] = (currentData.TotalRevenue + booking.TotalPrice, currentData.BookingCount + 1);
        //    }

        //    var monthNames = new[]
        //    {
        //        "Jan", "Feb", "Mar", "Apr", "May", "Jun",
        //        "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"
        //     };

        //    var result = totalWithMonth.Select(entry => new
        //    {
        //        Month = monthNames[entry.Key - 1],
        //        Booking = entry.Value.BookingCount,
        //        Revenue = entry.Value.TotalRevenue
        //    }).ToList();

        //    return Ok(result);
        //}


        //[HttpGet("export")]
        //public IActionResult ExportBookingByHomeStayID([FromQuery] Guid homeStayID)
        //{
        //        var bookings = _bookingRepository
        //            .FindWithInclude(b => b.HomeStay)
        //            .Where(b => b.HomeStay.Id == homeStayID && !b.isDeleted)
        //            .Select(b => new
        //            {
        //                b.Id,
        //                b.CheckInDate,
        //                b.CheckOutDate,
        //                b.UnitPrice,
        //                b.TotalPrice,
        //                b.Status,
        //                b.ReasonCancel,
        //                HomeStayName = b.HomeStay.Name
        //            })
        //            .ToList();

        //        if (!bookings.Any())
        //        {
        //            return NotFound(new { Message = "No bookings found for this homestay." });
        //        }

        //        ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
        //        using var pck = new ExcelPackage();
        //        var ws = pck.Workbook.Worksheets.Add("Booking List");

        //        // Header row
        //        string[] headers = {
        //    "Booking ID", "Check-in Date", "Check-out Date", "Unit Price", "Total Price", "Status",
        //    "Cancellation Reason", "HomeStay Name"
        //};

        //        for (int i = 0; i < headers.Length; i++)
        //        {
        //            ws.Cells[1, i + 1].Value = headers[i];
        //        }

        //        ws.Cells[1, 1, 1, headers.Length].Style.Font.Bold = true;
        //        ws.Cells[1, 1, 1, headers.Length].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
        //        ws.Cells[1, 1, 1, headers.Length].Style.Fill.PatternType = ExcelFillStyle.Solid;
        //        ws.Cells[1, 1, 1, headers.Length].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);

        //        // Add data rows
        //        int row = 2;
        //        foreach (var booking in bookings)
        //        {
        //            ws.Cells[row, 1].Value = booking.Id.ToString();
        //            ws.Cells[row, 2].Value = booking.CheckInDate.ToString("dd-MM-yyyy");
        //            ws.Cells[row, 3].Value = booking.CheckOutDate.ToString("dd-MM-yyyy");
        //            ws.Cells[row, 4].Value = $"{booking.UnitPrice} VND";
        //            ws.Cells[row, 5].Value = $"{booking.TotalPrice} VND";
        //            ws.Cells[row, 6].Value = booking.Status;
        //            ws.Cells[row, 7].Value = booking.ReasonCancel;
        //            ws.Cells[row, 8].Value = booking.HomeStayName;
        //            row++;
        //        }

        //        ws.Cells.AutoFitColumns();

        //        // Save to memory stream
        //        var stream = new MemoryStream();
        //        pck.SaveAs(stream);
        //        stream.Position = 0; // 🔹 Đảm bảo stream bắt đầu từ đầu

        //        var fileName = $"BookingList_{homeStayID}.xlsx";
        //        return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        //    }
        }
    }

