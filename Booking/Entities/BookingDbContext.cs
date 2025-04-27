using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace Booking.Entities
{
    public class BookingDbContext: DbContext
    {
        public BookingDbContext(DbContextOptions<BookingDbContext> options) : base(options) { }
        public DbSet<BookingEntity> Bookings { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<BookingEntity>().ToTable("Bookings");

            modelBuilder.Entity<BookingEntity>().HasKey(b => b.Id);

            modelBuilder.Entity<BookingEntity>()
                .Property(b => b.CustomerName)
                .IsRequired()
                .HasMaxLength(100);

            modelBuilder.Entity<BookingEntity>()
                .Property(b => b.ServiceType)
                .IsRequired()
                .HasMaxLength(50);

            modelBuilder.Entity<BookingEntity>()
                .Property(b => b.Timestamp)
                .IsRequired();
        }
    }
}
