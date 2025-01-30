namespace MvcProject.DTO;

public class TransactionModel
{
    public string Id { get; set; } = default!;

    public string? UserId { get; set; }

    public decimal Amount { get; set; }

    public string TransactionType { get; set; } = default!;

    public string Status { get; set; } = default!;

    public DateTime CreatedAt { get; set; }
}
