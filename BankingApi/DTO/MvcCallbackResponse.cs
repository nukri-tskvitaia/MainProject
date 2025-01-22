namespace BankingApi.DTO;

public class MvcCallbackResponse
{
    public string TransactionId { get; set; } = default!;
    public decimal Amount { get; set; }
    public string Status { get; set; } = default!;
}
