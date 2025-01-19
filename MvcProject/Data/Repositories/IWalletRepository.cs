using MvcProject.Models;

namespace MvcProject.Data.Repositories;

public interface IWalletRepository
{
    public Task<Guid> CreateWalletAsync(Wallet wallet);
    public Task<decimal> GetUserBalanceAsync(string userId);
    public Task<char> GetCurrencyAsync(string userId);
}
