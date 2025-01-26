namespace UserManagement.Client.ViewModels
{
    public class UserBookings
    {
        public int Id { get; set; }
        public int UserId { get; set; }

        public string BookingId { get; set; }

        public DateOnly? BookingDate { get; set; }

        public TimeOnly? BookingTime { get; set; }

        public string BookingStatus { get; set; }

        public string BookingType { get; set; }

        public string BookingAmount { get; set; }

        public string BookingDescription { get; set; }
    }
}
