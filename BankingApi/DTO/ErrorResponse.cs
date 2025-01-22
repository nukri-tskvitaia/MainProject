namespace BankingApi.DTO;

public class ErrorResponse
{
    public string Status { get; set; } = "Rejected";
    public string? ErrorCode { get; set; }
}