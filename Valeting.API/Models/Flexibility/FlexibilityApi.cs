using System.Text.Json.Serialization;

namespace Valeting.API.Models.Flexibility;

public class FlexibilityApi
{
    public Guid Id { get; set; }
    public string Description { get; set; }
    public bool Active { get; set; }
    [JsonPropertyName("_link")]
    public FlexibilityApiLink Link { get; set; }
}