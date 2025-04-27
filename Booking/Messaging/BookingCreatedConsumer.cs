using MassTransit;

namespace Booking.Messaging
{
    public class BookingCreatedConsumer : IConsumer<IBookingCreated>
    {
        public async Task Consume(ConsumeContext<IBookingCreated> context)
        {
            Console.WriteLine($"Processing Booking: {context.Message.CustomerName}");
            // Save the message to a database or log file
            await LogMessageAsync(context.Message);
        }

        private async Task LogMessageAsync(IBookingCreated message)
        {
            using (StreamWriter writer = new StreamWriter("processed_messages.log", true))
            {
                await writer.WriteLineAsync($"{DateTime.UtcNow} - Processed Booking: {message.BookingId}");
            }
        }
    }
}
