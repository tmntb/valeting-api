namespace Valeting.Repository.Entities;

public partial class ApplicationUser
{
    public Guid Id { get; set; }
    public string Username { get; set; } = null!;
    public string Password { get; set; } = null!;
    public string Salt { get; set; } = null!;
}