using Dapper;
using MvcProject.DTO;
using MvcProject.Models;
using System.Data;

namespace MvcProject.Data.Repositories;

public class ProcessRepository : IProcessRepository
{
    private readonly IDbConnection _dbConnection;

    public ProcessRepository(IDbConnection dbConnection)
    {
        _dbConnection = dbConnection;
    }

    public async Task<ResultResponse> ProcessDepositAsync(int depositWithdrawId, Transaction transaction)
    {
        var parameters = new DynamicParameters();
        // DepositWithdraw Table
        parameters.Add("@RequestId", depositWithdrawId, DbType.String);
        parameters.Add("@NewStatus", transaction.Status, DbType.String, size: 50, direction: ParameterDirection.Input);

        // Transaction Table
        parameters.Add("@TransactionId", transaction.Id, DbType.String, size: 450);
        parameters.Add("@UserId", transaction.UserId, DbType.String, size: 450);
        parameters.Add("@Amount", transaction.Amount, DbType.Decimal);
        parameters.Add("@TransactionType", transaction.TransactionType, dbType: DbType.String, size: 50);

        // Wallet Table

        // Output Responses
        parameters.Add("@Status", dbType: DbType.String, size: 50, direction: ParameterDirection.Output);
        parameters.Add("@ErrorMessage", dbType: DbType.String, size: 4000, direction: ParameterDirection.Output);

        await _dbConnection.ExecuteAsync(
            "ProcessDeposit",
            parameters,
            commandType: CommandType.StoredProcedure);

        var resultResponse = new ResultResponse
        {
            Status = parameters.Get<string>("@Status"),
            ErrorMessage = parameters.Get<string>("@ErrorMessage")
        };

        return resultResponse;
    }

    public async Task<ResultResponse> ProcessWithdrawAsync(int depositWithdrawId, Transaction transaction)
    {
        var parameters = new DynamicParameters();
        // DepositWithdraw Table
        parameters.Add("@RequestId", depositWithdrawId, DbType.String);
        parameters.Add("@NewStatus", transaction.Status, DbType.String, size: 50, direction: ParameterDirection.Input);

        // Transaction Table
        parameters.Add("@TransactionId", transaction.Id, DbType.String, size: 450);
        parameters.Add("@UserId", transaction.UserId, DbType.String, size: 450);
        parameters.Add("@Amount", transaction.Amount, DbType.Decimal);
        parameters.Add("@TransactionType", transaction.TransactionType, dbType: DbType.String, size: 50);

        // Wallet Table

        // Output Responses
        parameters.Add("@Status", dbType: DbType.String, size: 50, direction: ParameterDirection.Output);
        parameters.Add("@ErrorMessage", dbType: DbType.String, size: 4000, direction: ParameterDirection.Output);

        await _dbConnection.ExecuteAsync(
            "ProcessWithdraw",
            parameters,
            commandType: CommandType.StoredProcedure);

        var resultResponse = new ResultResponse
        {
            Status = parameters.Get<string>("@Status"),
            ErrorMessage = parameters.Get<string>("@ErrorMessage")
        };

        return resultResponse;
    }
}
