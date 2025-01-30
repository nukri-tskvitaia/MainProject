using MvcProject.DTO;

namespace MvcProject.Data.Repositories;

public interface IDepositWithdrawRequestRepository
{
    public Task<CreateDepositWithdrawResponse> CreateDepositAsync(DepositWithdrawRequestModel request);
    public Task<CreateDepositWithdrawResponse> CreateWithdrawAsync(DepositWithdrawRequestModel request);
    public Task<DepositWithdrawRequestModel?> GetByIdAsync(int id);
    public Task<IEnumerable<DepositWithdrawRequestModel>> GetPendingRequestsAsync(string value, string type);
    public Task<decimal> GetAmountAsync(int transactionId);
    public Task<string?> GetUserIdAsync(int transactionId);
    public Task<UpdateDepositWithdrawResponse> UpdateStatusAsync(int transactionId, string status);
}
