namespace Service.Models.Email;

/// <summary>
/// Represents an outbound email message.
/// </summary>
public class EmailDto
{
    /// <summary>
    /// Recipient email address.
    /// </summary>
    public string To { get; init; } = string.Empty;

    /// <summary>
    /// Email subject.
    /// </summary>
    public string Subject { get; init; } = string.Empty;

    /// <summary>
    /// Email HTML body.
    /// </summary>
    public string Body { get; init; } = string.Empty;
}
