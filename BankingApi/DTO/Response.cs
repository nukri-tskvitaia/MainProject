namespace BankingApi.DTO;

public class Response
{
    public string Status { get; set; } = default!;
    public string? PaymentUrl { get; set; }
}
