using Api.Models.Core;

namespace Api.Models.Flexibility.Payload;

/// <summary>
/// Query parameters for retrieving a list of flexibilities.
/// </summary>
public class FlexibilityApiParameters : QueryStringParametersApi
{
    /// <summary>
    /// Optional filter to return only active or inactive flexibilities.
    /// </summary>
    public bool? Active { get; set; }
}
