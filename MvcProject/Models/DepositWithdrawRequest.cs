namespace MvcProject.Models;

public partial class DepositWithdrawRequest
{
    public int Id { get; set; }

    public string? UserId { get; set; }

    public string TransactionType { get; set; } = default!;

    public decimal Amount { get; set; }

    public string Status { get; set; } = default!;

    public DateTime CreatedAt { get; set; }

    public User? User { get; set; }
}
