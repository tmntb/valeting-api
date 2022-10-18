using System.Text.Json.Serialization;

namespace Valeting.ApiObjects.VehicleSize
{
    public class VehicleSizeApi
    {
        public Guid Id { get; set; }
        public string Description { get; set; }
        public bool Actice { get; set; }
        [JsonPropertyName("_link")]
        public VehicleSizeApiLink Link { get; set; }
    }
}
