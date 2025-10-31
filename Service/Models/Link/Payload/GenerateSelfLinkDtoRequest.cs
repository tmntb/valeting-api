using Microsoft.AspNetCore.Http;

namespace Service.Models.Link.Payload;

/// <summary>
/// Represents the information required to generate a self-referencing URL for a resource.
/// </summary>
public class GenerateSelfLinkDtoRequest
{
    /// <summary>
    /// Identifier of the resource. If default, the generated URL will not include an ID segment.
    /// </summary>
    public Guid Id { get; set; } = default;

    /// <summary>
    /// Optional custom path to use instead of the request's path.
    /// </summary>
    public string Path { get; set; }

    /// <summary>
    /// The HTTP request used to construct the base URL.
    /// </summary>
    public HttpRequest Request { get; set; }
}
