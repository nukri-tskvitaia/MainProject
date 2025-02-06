namespace CasinoApi.DTO;

public class UpdatedBalanceResponse
{
    public UpdatedBalanceModel Data { get; set; } = default!;
    public int StatusCode { get; set; }
    public string? ErrorMessage { get; set; }
}
