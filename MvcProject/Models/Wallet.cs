namespace MvcProject.Models;

public partial class Wallet
{
    public string Id { get; set; } = Guid.Empty.ToString();

    public string UserId { get; set; } = default!;

    public decimal CurrentBalance { get; set; }

    public int Currency { get; set; }

    public User User { get; set; } = default!;
}
