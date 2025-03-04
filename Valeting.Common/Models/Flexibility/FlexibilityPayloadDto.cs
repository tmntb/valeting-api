using Valeting.Common.Models.Core;

namespace Valeting.Common.Models.Flexibility;

public class GetFlexibilityDtoRequest
{
    public Guid Id { get; set; }
}

public class GetFlexibilityDtoResponse : ValetingOutputDto
{
    public FlexibilityDto Flexibility { get; set; }
}

public class PaginatedFlexibilityDtoRequest
{
    public FlexibilityFilterDto Filter { get; set; }
}

public class PaginatedFlexibilityDtoResponse : ValetingOutputDto
{
    public int TotalItems { get; set; }
    public int TotalPages { get; set; }
    public List<FlexibilityDto> Flexibilities { get; set; }
}