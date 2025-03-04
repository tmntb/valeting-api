using Valeting.Models.Core;

namespace Valeting.Models.Flexibility;

public class FlexibilityApiPaginatedResponse : PaginationApi
{
    public List<FlexibilityApi> Flexibilities { get; set; }
}

public class FlexibilityApiResponse
{
    public FlexibilityApi Flexibility { get; set; }
}

public class FlexibilityApiError : ErrorApi { }