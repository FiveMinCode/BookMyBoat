﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using UserManagment.Model;

namespace UserManagement.API.DbContexts;

public partial class UserManagementContext : DbContext
{
    public UserManagementContext(DbContextOptions<UserManagementContext> options)
        : base(options)
    {
    }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserBooking> UserBookings { get; set; }

    public virtual DbSet<UserProfile> UserProfiles { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__User__3214EC07B2FCA6D8");

            entity.ToTable("User");

            entity.Property(e => e.Password).HasMaxLength(255);
            entity.Property(e => e.UserName).HasMaxLength(255);

            //entity.HasOne(d => d.Profile).WithMany(p => p.Users)
            //    .HasForeignKey(d => d.ProfileId)
            //    .OnDelete(DeleteBehavior.ClientSetNull)
            //    .HasConstraintName("FK__User__ProfileId__276EDEB3");
        });

        modelBuilder.Entity<UserBooking>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__UserBook__3214EC07A7EDF05E");

            entity.Property(e => e.BookingAmount).HasMaxLength(50);
            entity.Property(e => e.BookingId)
                .IsRequired()
                .HasMaxLength(50);
            entity.Property(e => e.BookingStatus).HasMaxLength(50);
            entity.Property(e => e.BookingType).HasMaxLength(50);

            entity.HasOne(d => d.User).WithMany(p => p.UserBookings)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__UserBooki__UserI__2A4B4B5E");
        });

        modelBuilder.Entity<UserProfile>(entity =>
        {
            entity.HasKey(e => e.ProfileId).HasName("PK__UserProf__290C88E4CFF1B1A2");

            entity.ToTable("UserProfile");

            entity.Property(e => e.Address).HasMaxLength(255);
            entity.Property(e => e.City).HasMaxLength(100);
            entity.Property(e => e.Country).HasMaxLength(50);
            entity.Property(e => e.Email)
                .IsRequired()
                .HasMaxLength(255);
            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(255);
            entity.Property(e => e.PhoneNumber)
                .IsRequired()
                .HasMaxLength(50);
            entity.Property(e => e.State).HasMaxLength(100);
            entity.Property(e => e.ZipCode).HasMaxLength(20);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}