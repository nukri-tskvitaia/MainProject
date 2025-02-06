namespace CasinoApi.DTO;

public class CurrentBalanceResponse
{
    public CurrentBalanceModel Data { get; set; } = default!;
    public int StatusCode { get; set; }
    public string? ErrorMessage { get; set; }
}
