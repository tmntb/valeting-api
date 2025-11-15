using Api.Models.Core;

namespace Api.Models.Flexibility;

/// <summary>
/// HATEOAS links for a flexibility resource.
/// </summary>
public class FlexibilityApiLink
{
    /// <summary>
    /// Self-link of the flexibility resource.
    /// </summary>
    public LinkApi Self { get; set; }
}
