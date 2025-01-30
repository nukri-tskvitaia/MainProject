using MvcProject.DTO;

namespace MvcProject.Services;

public interface IBankingApiService
{
    public Task<DepositBankingApiResponse> DepositBankingApiAsync(int depositWithdrawId, decimal amount);
    public Task<WithdrawBankingApiResponse> WithdrawBankingApiAsync(int depositWithdrawId, decimal amount);
}
