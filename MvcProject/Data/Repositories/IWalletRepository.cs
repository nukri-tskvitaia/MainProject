namespace MvcProject.Data.Repositories;

public interface IWalletRepository
{
    public Task<decimal> GetUserBalanceAsync(string userId);
}
