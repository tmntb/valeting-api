using Valeting.Services.Objects.Core;

namespace Valeting.Services.Objects.Link;

public class GenerateSelfUrlSVRequest
{
    public string BaseUrl { get; set; }
    public string Path { get; set; }
    public Guid Id { get; set; } = default;
}

public class GenerateSelfUrlSVResponse : ValetingOutputSV
{
    public string Self { get; set; }
}

public class GeneratePaginatedLinksSVRequest
{
    public string BaseUrl { get; set; }
    public string Path { get; set; }
    public string QueryString { get; set; }
    public int PageNumber { get; set; }
    public int TotalPages { get; set; }
    public object Filter { get; set; }
}

public class GeneratePaginatedLinksSVResponse : ValetingOutputSV
{
    public string Self { get; set; }
    public string Prev { get; set; }
    public string Next { get; set; }
}
