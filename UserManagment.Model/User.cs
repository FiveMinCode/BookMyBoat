﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace UserManagment.Model;

public partial class User
{
    public int Id { get; set; }

    public int ProfileId { get; set; }

    public string UserName { get; set; }

    public string Password { get; set; }

    public virtual UserProfile Profile { get; set; }

    public virtual ICollection<UserBooking> UserBookings { get; set; } = new List<UserBooking>();
}