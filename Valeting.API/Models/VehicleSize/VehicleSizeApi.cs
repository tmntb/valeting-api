using System.Text.Json.Serialization;

namespace Valeting.API.Models.VehicleSize;

public class VehicleSizeApi
{
    public Guid Id { get; set; }
    public string Description { get; set; }
    public bool Active { get; set; }
    [JsonPropertyName("_link")]
    public VehicleSizeApiLink Link { get; set; }
}