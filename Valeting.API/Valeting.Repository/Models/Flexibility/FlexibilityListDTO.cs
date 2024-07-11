using Valeting.Repository.Models.Core;

namespace Valeting.Repository.Models.Flexibility;

public class FlexibilityListDTO : ContentDTO
{
    public List<FlexibilityDTO> Flexibilities { get; set; }
}