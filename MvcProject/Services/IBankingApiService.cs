using MvcProject.DTO;

namespace MvcProject.Services;

public interface IBankingApiService
{
    public Task<DepositBankingApiResponse> DepositBankingApiAsync(string depositWithdrawId, decimal amount);
}
