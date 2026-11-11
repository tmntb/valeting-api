using Common.Enums;

namespace Api.Context;

public sealed class ValetingApiContext
{
    public Guid? UserId { get; set; }

    public string? Name { get; set; }

    public string? Email { get; set; }

    public RoleEnum? Role { get; set; }
    
    public string? ClientIpAddress { get; set; }
}
