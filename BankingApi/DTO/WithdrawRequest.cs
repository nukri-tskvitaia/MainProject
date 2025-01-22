namespace BankingApi.DTO;

public record WithdrawRequest
{
    public string TransactionID { get; set; } = default!;
    public int Amount { get; set; }
    public string MerchantID { get; set; } = default!;
    public string UsersAccountNumber { get; set; } = string.Empty;
    public string UsersFullName { get; set; } = string.Empty;
    public string Hash { get; set; } = default!;
}
