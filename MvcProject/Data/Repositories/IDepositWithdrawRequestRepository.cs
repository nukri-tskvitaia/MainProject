using MvcProject.DTO;

namespace MvcProject.Data.Repositories;

public interface IDepositWithdrawRequestRepository
{
    public Task<string?> CreateAsync(DepositWithdrawRequestModel request);
    public Task<DepositWithdrawRequestModel?> GetByIdAsync(int id);
    public Task<IEnumerable<DepositWithdrawRequestModel>> GetPendingRequestsAsync(string value, string type);
    public Task<decimal> GetAmountAsync(string transactionId);
    public Task<string?> GetUserIdAsync(string transactionId);
    public Task<bool> UpdateStatusAsync(string transactionId, string status);
    public Task<bool> DeleteAsync(string transactionId);
}
