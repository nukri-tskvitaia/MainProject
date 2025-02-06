namespace CasinoApi.DTO;

public class TokenResponse
{
    public PrivateTokenResponse Data { get; set; } = default!;
    public int StatusCode { get; set; } = default!;
    public string? ErrorMessage { get; set; }
}
