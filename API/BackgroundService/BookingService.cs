using BusinessObject.Entities;
using BusinessObject.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace API.BackgroundService
{
    public class BookingService : IHostedService, IDisposable
    {
        private Timer _timer;
        private readonly ILogger<BookingService> _logger;
        private readonly IServiceScopeFactory _scopeFactory;

        public BookingService(ILogger<BookingService> logger, IServiceScopeFactory scopeFactory)
        {
            _logger = logger;
            _scopeFactory = scopeFactory;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Booking Status Service Is Running");

            _timer = new Timer(async _ => await SetIsBookedForFalse(), null, TimeSpan.Zero, TimeSpan.FromDays(1));

            return Task.CompletedTask;
        }

        private async Task SetIsBookedForFalse()
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                try
                {
                    var calendarRepository = scope.ServiceProvider.GetRequiredService<IRepository<Calendar>>();
                    var homeStayRepository = scope.ServiceProvider.GetRequiredService<IRepository<HomeStay>>();

                    DateTime today = DateTime.UtcNow.Date;

                    var expiredCalendars = await calendarRepository.FindWithInclude()
                        .Where(c => c.Booking != null && c.Booking.CheckOutDate < today && !c.Booking.isDeleted)
                        .Include(c => c.HomeStay)
                        .ToListAsync();

                    if (!expiredCalendars.Any()) return;

                    foreach (var calendar in expiredCalendars)
                    {
                        calendar.isBooked = false;
                        await calendarRepository.UpdateAsync(calendar);
                    }

                    await calendarRepository.SaveAsync();
                    _logger.LogInformation("Updated isBooked status for expired bookings.");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error updating isBooked status.");
                }
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Booking Status Service is stopping.");
            _timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}
