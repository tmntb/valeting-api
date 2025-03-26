using Microsoft.AspNetCore.Http;
using Valeting.Common.Models.Core;

namespace Valeting.Common.Models.Link;

public class GenerateSelfUrlDtoRequest
{
    public HttpRequest Request { get; set; }
    public string Path { get; set; }
    public Guid Id { get; set; } = default;
}

public class GenerateSelfUrlDtoResponse
{
    public string Self { get; set; }
}

public class GeneratePaginatedLinksDtoRequest
{
    public HttpRequest Request { get; set; }
    public int TotalPages { get; set; }
    public FilterDto Filter { get; set; }
}

public class GeneratePaginatedLinksDtoResponse
{
    public string Self { get; set; }
    public string Prev { get; set; }
    public string Next { get; set; }
}
