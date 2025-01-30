using MvcProject.DTO;
using MvcProject.Models;

namespace MvcProject.Data.Repositories;

public interface IProcessRepository
{
    public Task<ResultResponse> ProcessDepositAsync(int depositWithdrawId, Transaction transaction);
    public Task<ResultResponse> ProcessWithdrawAsync(int depositWithdrawId, Transaction transaction);
}
