using Valeting.Common.Models.Core;

namespace Valeting.Common.Models.Flexibility;

public class FlexibilityListDto : ContentDto
{
    public List<FlexibilityDto> Flexibilities { get; set; }
}