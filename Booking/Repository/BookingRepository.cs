using Booking.Entities;
using Microsoft.EntityFrameworkCore;

namespace Booking.Repository
{
    public class BookingRepository : IBookingRepository
    {
        private readonly BookingDbContext _context;

        public BookingRepository(BookingDbContext context)
        {
            _context = context;
        }

        public async Task<BookingEntity> AddAsync(BookingEntity booking)
        {
            _context.Bookings.Add(booking);
            await _context.SaveChangesAsync();
            return booking;
        }

        public async Task<IEnumerable<BookingEntity>> GetAllAsync()
        {
            return await _context.Bookings.ToListAsync();
        }
    }
}
