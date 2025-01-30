namespace MvcProject.DTO;

public class DepositWithdrawRequestModel
{
    public int? Id { get; set; }
    public string? UserId { get; set; }

    public string TransactionType { get; set; } = default!;

    public decimal Amount { get; set; }

    public string Status { get; set; } = default!;

    public DateTime? CreatedAt { get; set; }
}
