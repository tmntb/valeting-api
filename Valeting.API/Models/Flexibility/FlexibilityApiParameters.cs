using Valeting.API.Models.Core;

namespace Valeting.API.Models.Flexibility;

public class FlexibilityApiParameters : QueryStringParametersApi
{
    public bool? Active { get; set; }
}