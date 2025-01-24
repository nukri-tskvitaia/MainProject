namespace MvcProject.Services;

public interface IDepositWithdrawService
{
    public Task<string?> AddDepositAsync(string userId, decimal amount);
    public Task<string?> AddWithdrawAsync(string userId, decimal amount);
}
