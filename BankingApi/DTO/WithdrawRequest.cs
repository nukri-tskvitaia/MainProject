namespace BankingApi.DTO;

public record WithdrawRequest
{
    public int Id { get; set; } = default!;
    public decimal Amount { get; set; }
    public string MerchantID { get; set; } = default!;
    // public string UsersAccountNumber { get; set; } = string.Empty;
    // public string UsersFullName { get; set; } = string.Empty;
    public string Hash { get; set; } = default!;
}
