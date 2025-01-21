using MvcProject.Models;

namespace MvcProject.Data.Repositories;

public interface IDepositWithdrawRequestRepository
{
    public Task<string?> CreateAsync(DepositWithdrawRequest request);
    public Task<IEnumerable<DepositWithdrawRequest>> GetAllAsync();
}
