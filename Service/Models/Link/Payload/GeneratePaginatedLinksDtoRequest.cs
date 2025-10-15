using Microsoft.AspNetCore.Http;
using Service.Models.Core;

namespace Service.Models.Link.Payload;

public class GeneratePaginatedLinksDtoRequest
{
    public FilterDto Filter { get; set; }
    public int TotalPages { get; set; }
    public HttpRequest Request { get; set; }
}
