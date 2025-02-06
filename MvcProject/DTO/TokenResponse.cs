namespace MvcProject.DTO;

public class TokenResponse
{
    public string PublicToken { get; set; } = default!;
    public int StatusCode { get; set; }
    public string? ErrorMessage { get; set; }
}
