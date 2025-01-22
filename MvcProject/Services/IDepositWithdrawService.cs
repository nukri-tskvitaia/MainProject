namespace MvcProject.Services;

public interface IDepositWithdrawService
{
    public Task<string?> AddDepositWithdrawAsync(string userId, decimal amount);
}
