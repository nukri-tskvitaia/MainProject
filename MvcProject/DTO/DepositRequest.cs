namespace MvcProject.DTO;

public class DepositRequest
{
    public string Id { get; set; } = default!;
    public decimal Amount { get; set; }
    public string MerchantId { get; set; } = default!;
    public string Hash { get; set; } = default!;
}
