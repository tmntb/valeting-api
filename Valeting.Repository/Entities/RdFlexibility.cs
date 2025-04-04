﻿using System.ComponentModel.DataAnnotations;

namespace Valeting.Repository.Entities;

public partial class RdFlexibility
{
    public RdFlexibility()
    {
        Bookings = new HashSet<Booking>();
    }

    public Guid Id { get; set; }
    public string Description { get; set; } = null!;
    public bool Active { get; set; }

    public virtual ICollection<Booking> Bookings { get; set; }
}