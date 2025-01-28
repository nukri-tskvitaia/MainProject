using MvcProject.DTO;
using MvcProject.Models;

namespace MvcProject.Data.Repositories;

public interface IWalletRepository
{
    public Task<bool> CreateWalletAsync(Wallet wallet);
    public Task<decimal> GetUserBalanceAsync(string userId);
    public Task<char> GetCurrencyAsync(string userId);
    public Task<bool> DepositBalanceAsync(string userId, decimal balance);
    public Task<bool> WithdrawBalanceAsync(string userId, decimal balance, decimal blockedAmount);
    public Task<WalletBallanceRequest> CheckAvailableBalanceAsync(string userId);
}
