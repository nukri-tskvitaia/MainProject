namespace MvcProject.Models;

public partial class Transaction
{
    public int Id { get; set; }

    public string? UserId { get; set; }

    public decimal Amount { get; set; }

    public string Status { get; set; } = default!;

    public DateTime CreatedAt { get; set; }

    public User? User { get; set; }
}
