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

    public async Task<CreateDepositWithdrawResponse> CreateDepositAsync(DepositWithdrawRequestModel request)
    {
        var parameters = new DynamicParameters();
        parameters.Add("@Id", dbType: DbType.Int32, direction: ParameterDirection.Output);
        parameters.Add("@UserId", request.UserId, size: 450);
        parameters.Add("@TransactionType", request.TransactionType, size: 50);
        parameters.Add("@Amount", request.Amount);
        parameters.Add("@NewStatus", request.Status, dbType: DbType.String, size: 50);
        parameters.Add("@Status", dbType: DbType.String, size: 50, direction: ParameterDirection.Output);
        parameters.Add("@ErrorMessage", dbType: DbType.String, size: 4000, direction: ParameterDirection.Output);

        await _dbConnection.ExecuteAsync("CreateDeposit", parameters, commandType: CommandType.StoredProcedure);
        var response = new CreateDepositWithdrawResponse
        {
            DepositWithdrawId = parameters.Get<int?>("@Id"),
            Status = parameters.Get<string>("@Status"),
            ErrorMessage = parameters.Get<string>("@ErrorMessage")
        };

        return response;
    }

    public async Task<CreateDepositWithdrawResponse> CreateWithdrawAsync(DepositWithdrawRequestModel request)
    {
        var parameters = new DynamicParameters();
        parameters.Add("@Id", dbType: DbType.Int32, direction: ParameterDirection.Output);
        parameters.Add("@UserId", request.UserId, size: 450);
        parameters.Add("@TransactionType", request.TransactionType, size: 50);
        parameters.Add("@Amount", request.Amount);
        parameters.Add("@NewStatus", request.Status, dbType: DbType.String, size: 50);
        parameters.Add("@Status", dbType: DbType.String, size: 50, direction: ParameterDirection.Output);
        parameters.Add("@ErrorMessage", dbType: DbType.String, size: 4000, direction: ParameterDirection.Output);

        await _dbConnection.ExecuteAsync("CreateWithdraw", parameters, commandType: CommandType.StoredProcedure);
        var response = new CreateDepositWithdrawResponse
        {
            DepositWithdrawId = parameters.Get<int?>("@Id"),
            Status = parameters.Get<string>("@Status"),
            ErrorMessage = parameters.Get<string>("@ErrorMessage")
        };

        return response;
    }

    public async Task<decimal> GetAmountAsync(int transactionId)
    {
        string query = @"
            SELECT Amount FROM DepositWithdrawRequests
            WHERE Id = @transactionId";

        return await _dbConnection.ExecuteScalarAsync<decimal>(query, new { transactionId } );
    }

    public async Task<string?> GetUserIdAsync(int transactionId)
    {
        string query = @"
            SELECT UserId FROM DepositWithdrawRequests
            WHERE Id = @transactionId";

        return await _dbConnection.ExecuteScalarAsync<string?>(query, new { transactionId });
    }

    // Done
    public async Task<UpdateDepositWithdrawResponse> UpdateStatusAsync(int transactionId, string status)
    {
        var parameters = new DynamicParameters();
        parameters.Add("@Id", transactionId, DbType.String);
        parameters.Add("@NewStatus", status, DbType.String, size: 30, direction: ParameterDirection.Input);
        parameters.Add("@Status", dbType: DbType.String, size: 30, direction: ParameterDirection.Output);
        parameters.Add("@ErrorMessage", dbType: DbType.String, size: 4000, direction: ParameterDirection.Output);

        await _dbConnection.ExecuteAsync(
            "UpdateDepositWithdraw",
            parameters,
            commandType: CommandType.StoredProcedure
            );

        var result = new UpdateDepositWithdrawResponse
        {
            Status = parameters.Get<string>("@Status"),
            ErrorMessage = parameters.Get<string>("@ErrorMessage")
        };

        return result;
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
