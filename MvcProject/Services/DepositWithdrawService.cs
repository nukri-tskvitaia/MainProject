using MvcProject.Data.Repositories;
using MvcProject.DTO;

namespace MvcProject.Services;

public class DepositWithdrawService : IDepositWithdrawService
{
    private readonly IDepositWithdrawRequestRepository _request;

    public DepositWithdrawService(IDepositWithdrawRequestRepository request)
    {
        _request = request;
    }
    
    // Done
    public async Task<CreateDepositWithdrawResponse> AddDepositAsync(string userId, decimal amount)
    {
        var transaction = new DepositWithdrawRequestModel
        {
            UserId = userId,
            TransactionType = "Deposit",
            Amount = amount,
            Status = "Pending",
        };
        var response = await _request.CreateDepositAsync(transaction);

        return response;
    }

    public async Task<CreateDepositWithdrawResponse> AddWithdrawAsync(string userId, decimal amount)
    {
        var transaction = new DepositWithdrawRequestModel
        {
            UserId = userId,
            TransactionType = "Withdraw",
            Amount = amount,
            Status = "Pending",
        };
        var response = await _request.CreateWithdrawAsync(transaction);

        return response;
    }
}
