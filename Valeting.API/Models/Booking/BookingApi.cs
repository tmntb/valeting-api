﻿using System.Text.Json.Serialization;
using Valeting.API.Models.Flexibility;
using Valeting.API.Models.VehicleSize;

namespace Valeting.API.Models.Booking;

public class BookingApi
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public DateTime BookingDate { get; set; }
    public FlexibilityApi Flexibility { get; set; }
    public VehicleSizeApi VehicleSize { get; set; }
    public int ContactNumber { get; set; }
    public string Email { get; set; }
    public bool? Approved { get; set; }
    [JsonPropertyName("_link")]
    public BookingApiLink Link { get; set; }
}