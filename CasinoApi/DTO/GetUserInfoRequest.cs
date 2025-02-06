namespace CasinoApi.DTO;

public class GetUserInfoRequest
{
    public string Token { get; set; } = default!;
    public string? Hash { get; set; }
}
