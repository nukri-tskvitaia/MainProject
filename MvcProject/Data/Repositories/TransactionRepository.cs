using Dapper;
using MvcProject.DTO;
using MvcProject.Models;
using System.Data;

namespace MvcProject.Data.Repositories;

public class TransactionRepository : ITransactionRepository
{
    private readonly IDbConnection _dbConnection;

    public TransactionRepository(IDbConnection dbConnection)
    {
        _dbConnection = dbConnection;
    }

    // Done
    public async Task<bool> CreateAsync(Transaction data)
    {
        var parameters = new DynamicParameters();

        parameters.Add("@Id", data.Id, DbType.String);
        parameters.Add("@UserId", data.UserId, DbType.String);
        parameters.Add("@Amount", data.Amount, DbType.Decimal);
        parameters.Add("@Status", data.Status, dbType: DbType.String, size: 30, direction: ParameterDirection.InputOutput);
        parameters.Add("@CreatedAt", dbType: DbType.DateTime);

        await _dbConnection.ExecuteAsync("CreateTransaction", parameters, commandType: CommandType.StoredProcedure);

        var status = parameters.Get<string>("@Status");
        
        if (status == "Failed")
        {
            return false;
        }

        return true;
    }

    public async Task<IEnumerable<TransactionModel>> GetAllUserAsync(string userId)
    {
        string query = @"
            SELECT * FROM Transactions
            WHERE UserId = @userId
            ORDER BY CreatedAt DESC";

        return await _dbConnection.QueryAsync<TransactionModel>(query, new { userId});
    }
}