using Valeting.SwaggerDocumentation.Parameter;

namespace Valeting.ApiObjects
{
    public abstract class QueryStringParametersApi
    {
        [QueryParameter("The requested page number", "1", 1)]
        public int PageNumber { get; set; } = 1;
        [QueryParameter("The number of elements for the page request", "5", 1, 10)]
        public int PageSize { get; set; } = 10;
    }
}

