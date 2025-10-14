using Api.Models.Core;

namespace Api.Models.Flexibility;

public class FlexibilityApiPaginatedResponse : PaginationApi
{
    public List<FlexibilityApi> Flexibilities { get; set; }
}

public class FlexibilityApiResponse
{
    public FlexibilityApi Flexibility { get; set; }
}
