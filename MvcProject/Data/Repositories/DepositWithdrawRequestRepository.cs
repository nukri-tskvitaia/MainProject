using Dapper;
using MvcProject.Models;
using System.Data;

namespace MvcProject.Data.Repositories;

public class DepositWithdrawRequestRepository : IDepositWithdrawRequestRepository
{
    private readonly IDbConnection _dbConnection;

    public DepositWithdrawRequestRepository(IDbConnection dbConnection)
    {
        _dbConnection = dbConnection;
    }

    public async Task<string?> CreateAsync(DepositWithdrawRequest request)
    {
        string query = @"
            INSERT INTO DepositWithdrawRequests (UserId, TransactionType, Amount, Status, CreatedAt)
            VALUES (@UserId, @TransactionType, @Amount, @Status, @CreatedAt);
            SELECT CAST(SCOPE_IDENTITY() as int)";

        return await _dbConnection.ExecuteScalarAsync<string>(query, new
        {
            request.UserId,
            request.TransactionType,
            request.Amount,
            request.Status,
            request.CreatedAt,
        });
    }

    public async Task<IEnumerable<DepositWithdrawRequest>> GetAllAsync()
    {
        string query = @"
            SELECT * FROM DepositWithdrawRequests";

        return await _dbConnection.QueryAsync<DepositWithdrawRequest>(query);
    }
}
