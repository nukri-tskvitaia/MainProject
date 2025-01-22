using MvcProject.Data.Repositories;
using MvcProject.Models;

namespace MvcProject.Services;

public class DepositWithdrawService : IDepositWithdrawService
{
    private readonly IDepositWithdrawRequestRepository _request;

    public DepositWithdrawService(IDepositWithdrawRequestRepository request)
    {
        _request = request;
    }

    public async Task<string?> AddDepositWithdrawAsync(string userId, decimal amount)
    {
        var transaction = new DepositWithdrawRequest
        {
            UserId = userId,
            TransactionType = "Deposit",
            Amount = amount,
            Status = "Pending",
            CreatedAt = DateTime.UtcNow,
        };
        
        return await _request.CreateAsync(transaction);
    }
}
