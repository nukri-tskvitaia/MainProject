using MvcProject.DTO;

namespace MvcProject.Services;

public interface IDepositWithdrawService
{
    public Task<CreateDepositWithdrawResponse> AddDepositAsync(string userId, decimal amount);
    public Task<CreateDepositWithdrawResponse> AddWithdrawAsync(string userId, decimal amount);
}
