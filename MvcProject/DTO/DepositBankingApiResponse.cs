namespace MvcProject.DTO;

public class DepositBankingApiResponse
{
    public string Status { get; set; } = default!;
    public string? PaymentUrl { get; set; }
    public string? ErrorCode { get; set; }
}
