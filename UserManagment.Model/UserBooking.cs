﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace UserManagment.Model;

public partial class UserBooking
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

    public virtual User User { get; set; }
}