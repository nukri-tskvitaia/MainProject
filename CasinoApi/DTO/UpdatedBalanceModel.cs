namespace CasinoApi.DTO;

public class UpdatedBalanceModel
{
    public string TransactionId { get; set; } = default!;
    public decimal? UpdatedBalance { get; set; }
}
