namespace CasinoApi.DTO;

public class UserInfoResponse
{
    public UserInfoModel Data { get; set; } = default!;
    public int StatusCode { get; set; }
    public string? ErrorMessage { get; set; }
}
