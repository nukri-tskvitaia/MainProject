namespace MvcProject.DTO;

public class WalletModel
{
    public string Id { get; set; } = default!;

    public string UserId { get; set; } = default!;

    public decimal Amount { get; set; }

    public decimal BlockedAmount { get; set; } = 0;

    public int Currency { get; set; }
}
