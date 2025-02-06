using CasinoApi.DTO;

namespace CasinoApi.Repositories;

public interface ICasinoRepository
{
    public Task<UpdatedBalanceResponse> MakeBetAsync(BetRequest bet);
    public Task<UpdatedBalanceResponse> WinAsync(WinRequest win);
    public Task<UpdatedBalanceResponse> CancelBetAsync(CancelBetRequest cancelBet);
    public Task<UpdatedBalanceResponse> ChangeWinAsync(ChangeWinRequest changeWin);
    public Task<CurrentBalanceResponse> GetBalanceAsync(GetBalanceRequest getBalance);
    public Task<UserInfoResponse> GetPlayerInfoAsync(GetUserInfoRequest userInfo);
}
