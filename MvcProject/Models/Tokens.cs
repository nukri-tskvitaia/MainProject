namespace MvcProject.Models;

public class Tokens
{
    public string UserId { get; set; } = default!;
    public string PublicToken { get; set; } = default!;

    public string? PrivateToken { get; set; }

    public bool IsPublicTokenValid { get; set; }

    public bool IsPrivateTokenValid { get; set; }

    public User User { get; set; } = default!;
}
