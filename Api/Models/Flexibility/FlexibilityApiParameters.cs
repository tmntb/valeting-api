using Api.Models.Core;

namespace Api.Models.Flexibility;

public class FlexibilityApiParameters : QueryStringParametersApi
{
    public bool? Active { get; set; }
}