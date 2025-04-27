using MassTransit;

namespace Booking.Messaging
{
    public class MassTransitConfigurator
    {
        public static void ConfigureServices(IServiceCollection services)
        {
            services.AddMassTransit(x =>
            {
                x.AddConsumer<BookingCreatedConsumer>();
                x.UsingRabbitMq((context, cfg) =>
                {
                    cfg.Host("rabbitmq://localhost");
                    cfg.ReceiveEndpoint("booking-service", e =>
                    {
                        e.ConfigureConsumer<BookingCreatedConsumer>(context);
                    });
                });
            });
            services.AddMassTransitHostedService();
        }
    }
}
