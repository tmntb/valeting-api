using Valeting.Models.Core;

namespace Valeting.Models.Flexibility;

public class FlexibilityApiParameters : QueryStringParametersApi
{
    public bool? Active { get; set; }
}