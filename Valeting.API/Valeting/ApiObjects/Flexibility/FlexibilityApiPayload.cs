using Valeting.ApiObjects.Core;

namespace Valeting.ApiObjects.Flexibility;

public class FlexibilityApiPaginatedResponse : PaginationApi
{
    public List<FlexibilityApi> Flexibilities { get; set; }
}

public class FlexibilityApiResponse
{
    public FlexibilityApi Flexibility { get; set; }
}

public class FlexibilityApiError : ErrorApi { }