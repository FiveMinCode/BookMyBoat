namespace Booking.Entities
{
    public class BookingEntity
    {
        public Guid Id { get; set; }
        public string CustomerName { get; set; }
        public string ServiceType { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
