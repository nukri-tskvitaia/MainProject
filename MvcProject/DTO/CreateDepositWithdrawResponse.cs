namespace MvcProject.DTO;

public class CreateDepositWithdrawResponse
{
    public int? DepositWithdrawId { get; set; }
    public string Status { get; set; } = default!;
    public string? ErrorMessage { get; set; }
}