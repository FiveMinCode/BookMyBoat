namespace Booking.Messaging
{
    public interface IBookingCreated
    {
        Guid BookingId { get; }
        DateTime Timestamp { get; }
        string CustomerName { get; }
        string ServiceType { get; }
    }
}
