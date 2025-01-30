namespace MvcProject.Models;

public partial class Transaction
{
    public string Id { get; set; } = Guid.Empty.ToString();

    public string? UserId { get; set; }

    public string TransactionType { get; set; } = default!;

    public decimal Amount { get; set; }

    public string Status { get; set; } = default!;

    public DateTime CreatedAt { get; set; }

    public User? User { get; set; }
}
