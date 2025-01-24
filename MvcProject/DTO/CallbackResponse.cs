namespace MvcProject.DTO;

public class CallbackResponse
{
    public string TransactionId { get; set; } = default!;
    public decimal Amount { get; set; }
    public string Status { get; set; } = default!;
    public string Hash { get; set; } = default!;
    public string ClientId { get; set; } = default!;
}
