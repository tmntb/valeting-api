using Microsoft.AspNetCore.Http;

namespace Service.Models.Link.Payload;

public class GenerateSelfLinkDtoRequest
{
    public Guid Id { get; set; } = default;
    public string Path { get; set; }
    public HttpRequest Request { get; set; }
}
