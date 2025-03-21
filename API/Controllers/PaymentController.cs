using BusinessObject.Entities;
using BusinessObject.Exceptions;
using BusinessObject.Interfaces;
using BusinessObject.Shares;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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

            if (code == "00" && status == "PAID")
            {
                booking.Status = "Paid";
            }

            await _bookingRepository.UpdateAsync(booking);
            await _bookingRepository.SaveAsync();

            return Redirect(_payOSConfig.ClientRedirectUrl);
        }



    }
}
