namespace CasinoApi.DTO;

public class CancelBetRequest
{
    public string Token { get; set; } = default!;
    public decimal Amount { get; set; }
    public string TransactionId { get; set; } = default!;
    public int BetTypeId { get; set; }
    public int GameId { get; set; }
    public int ProductId { get; set; }
    public int RoundId { get; set; }
    public string? Hash { get; set; }
    public string? Currency {  get; set; }
    public string BetTransactionId { get; set; } = default!;
    public int? CampaignId { get; set; }
    public string? CampaignName { get; set; }

}
