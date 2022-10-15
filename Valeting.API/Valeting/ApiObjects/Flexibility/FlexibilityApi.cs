using System.Text.Json.Serialization;

namespace Valeting.ApiObjects.Flexibility
{
    public class FlexibilityApi
    {
        public Guid Id { get; set; }
        public string Description { get; set; }
        public bool Active { get; set; }
        [JsonPropertyName("_links")]
        public FlexibilityApiLink Links { get; set; }
    }
}
