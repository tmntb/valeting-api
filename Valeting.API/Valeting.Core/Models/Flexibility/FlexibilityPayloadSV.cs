using Valeting.Core.Models.Core;

namespace Valeting.Core.Models.Flexibility;

public class GetFlexibilitySVRequest
{
    public Guid Id { get; set; }
}

public class GetFlexibilitySVResponse : ValetingOutputSV
{
    public FlexibilitySV Flexibility { get; set; }
}

public class PaginatedFlexibilitySVRequest
{
    public FlexibilityFilterSV Filter { get; set; }
}

public class PaginatedFlexibilitySVResponse : ValetingOutputSV
{
    public int TotalItems { get; set; }
    public int TotalPages { get; set; }
    public List<FlexibilitySV> Flexibilities { get; set; }
}