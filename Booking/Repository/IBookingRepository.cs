using Booking.Entities;

namespace Booking.Repository
{
    public interface IBookingRepository
    {
        Task<BookingEntity> AddAsync(BookingEntity booking);
        Task<IEnumerable<BookingEntity>> GetAllAsync();
    }
}
