using Booking.Entities;
using Booking.Messaging;
using Booking.Repository;
using MassTransit;
using Microsoft.AspNetCore.Mvc;

namespace Booking.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class BookingController : ControllerBase
    {
        private readonly IBus _bus;
        private readonly IBookingRepository _bookingRepository;

        public BookingController(IBus bus, IBookingRepository bookingRepository)
        {
            _bus = bus;
            _bookingRepository = bookingRepository;
        }

        [HttpPost]
        public async Task<IActionResult> CreateBooking([FromBody] BookingEntity request)
        {
            var savedBooking = await _bookingRepository.AddAsync(request);
            await _bus.Publish<IBookingCreated>(new
            {
                BookingId = savedBooking.Id,
                Timestamp = DateTime.UtcNow,
                CustomerName = savedBooking.CustomerName,
                ServiceType = savedBooking.ServiceType
            });
            return CreatedAtAction(nameof(CreateBooking), new { id = savedBooking.Id }, savedBooking);
        }

    }
}
