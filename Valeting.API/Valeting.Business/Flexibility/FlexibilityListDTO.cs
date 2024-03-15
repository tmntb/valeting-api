using Valeting.Business.Core;

namespace Valeting.Business.Flexibility;

public class FlexibilityListDTO : ContentDTO
{
    public List<FlexibilityDTO> Flexibilities { get; set; }
}