using BusinessObject.Entities;
using BusinessObject.Interfaces;
using DataAccess.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repositories
{
    public class BookingRepository(ApplicationDbContext context) : Repository<Booking>(context), IBookingRepository
    {
        public override async Task<IEnumerable<Booking>> GetAllAsync()
        {
            return await context.Bookings
                .Include(b => b.Calendars)
                .ThenInclude(c => c.HomeStay)
                .ToListAsync();
        }

        public async Task<IEnumerable<Booking>> GetHistory(Guid userId)
        {
            return await context.Bookings
                .Include(b => b.Calendars)
                .ThenInclude(c => c.HomeStay)
                .Where(b => b.UserID == userId)
                .ToListAsync();
        }
    }
}
