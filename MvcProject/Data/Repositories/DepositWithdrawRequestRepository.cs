using Dapper;
using MvcProject.DTO;
using System.Data;

namespace MvcProject.Data.Repositories;

public class DepositWithdrawRequestRepository : IDepositWithdrawRequestRepository
{
    private readonly IDbConnection _dbConnection;

    public DepositWithdrawRequestRepository(IDbConnection dbConnection)
    {
        _dbConnection = dbConnection;
    }

    public async Task<string?> CreateAsync(DepositWithdrawRequestModel request)
    {
        var parameters = new DynamicParameters();
        parameters.Add("@UserId", request.UserId);
        parameters.Add("@TransactionType", request.TransactionType);
        parameters.Add("@Amount", request.Amount);
        parameters.Add("@Status", request.Status, dbType: DbType.String, direction: ParameterDirection.InputOutput);
        parameters.Add("@CreatedAt");
        parameters.Add("@Id", dbType: DbType.Int32, direction: ParameterDirection.Output);

        await _dbConnection.ExecuteAsync("CreateDepositWithdraw", parameters, commandType: CommandType.StoredProcedure);

        var status = parameters.Get<string>("@Status");
        var id = parameters.Get<int>("@Id").ToString();

        if (status == "Failed")
        {
            return null;
        }

        return id;
    }

    public async Task<decimal> GetAmountAsync(string transactionId)
    {
        string query = @"
            SELECT Amount FROM DepositWithdrawRequests
            WHERE Id = @transactionId";

        return await _dbConnection.ExecuteScalarAsync<decimal>(query, new { transactionId } );
    }

    public async Task<string?> GetUserIdAsync(string transactionId)
    {
        string query = @"
            SELECT UserId FROM DepositWithdrawRequests
            WHERE Id = @transactionId";

        return await _dbConnection.ExecuteScalarAsync<string?>(query, new { transactionId });
    }

    // Done
    public async Task<bool> UpdateStatusAsync(string transactionId, string status)
    {
        var parameters = new DynamicParameters();
        parameters.Add("@Id", transactionId, DbType.String);
        parameters.Add("@Status", status, DbType.String, size: 30, direction: ParameterDirection.InputOutput);
        
        await _dbConnection.ExecuteAsync(
            "UpdateDepositWithdrawStatus",
            parameters,
            commandType: CommandType.StoredProcedure
            );

        var result = parameters.Get<string>("@Status");

        if (result == "Failed")
        {
            return false;
        }

        return true;
    }

    public async Task<bool> DeleteAsync(string transactionId)
    {
        string query = @"
            DELETE FROM DepositWithdrawRequests
            WHERE Id = @transactionId";

        int rowsAffected = await _dbConnection.ExecuteAsync(query, new { transactionId });

        if (rowsAffected > 0)
        {
            return true;
        }
        
        return false;
    }

    public async Task<IEnumerable<DepositWithdrawRequestModel>> GetPendingRequestsAsync(string value, string type)
    {
        string query = @"
            SELECT Id, UserId, TransactionType, Amount, Status, CreatedAt
            FROM DepositWithdrawRequests
            WHERE Status = @value AND TransactionType = @type
            ORDER BY CreatedAt DESC";

        return await _dbConnection.QueryAsync<DepositWithdrawRequestModel>(query, new { value, type });
    }

    public async Task<DepositWithdrawRequestModel?> GetByIdAsync(int id)
    {
        string query = @"
            SELECT UserId, TransactionType, Amount, Status, CreatedAt
            FROM DepositWithdrawRequests
            WHERE Id = @id"
        ;

        return await _dbConnection.QueryFirstOrDefaultAsync<DepositWithdrawRequestModel>(query, new { id });
    }
}
