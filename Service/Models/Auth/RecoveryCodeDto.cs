using Service.Models.User;

namespace Service.Models.Auth;

public class RecoveryCodeDto
{
    /// <summary>
    /// 
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public UserDto User { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public string CodeHash { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public DateTime? UsedAt { get; set; }
}
