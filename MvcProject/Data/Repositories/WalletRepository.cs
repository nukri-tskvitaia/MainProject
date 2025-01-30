using Dapper;
using MvcProject.DTO;
using MvcProject.Helper;
using System.Data;

namespace MvcProject.Data.Repositories;

public class WalletRepository : IWalletRepository
{
    private readonly IDbConnection _dbConnection;

    public WalletRepository(IDbConnection dbConnection)
    {
        _dbConnection = dbConnection;
    }

    public async Task<CreateWalletResponse> CreateWalletAsync(WalletModel wallet)
    {
        var parameters = new DynamicParameters();
        parameters.Add("@Id", wallet.Id);
        parameters.Add("@UserId", wallet.UserId);
        parameters.Add("@Amount", wallet.Amount);
        parameters.Add("@Currency", wallet.Currency);
        parameters.Add("@BlockedAmount", wallet.BlockedAmount);
        parameters.Add("@Status", dbType: DbType.String, size: 30, direction: ParameterDirection.Output);
        parameters.Add("@ErrorMessage", dbType: DbType.String, size: 4000, direction: ParameterDirection.Output);

        await _dbConnection.ExecuteAsync(
            "CreateWallet",
            parameters,
            commandType: CommandType.StoredProcedure
            );
        var response = new CreateWalletResponse
        {
            Status = parameters.Get<string>("@Status"),
            ErrorMessage = parameters.Get<string>("@ErrorMessage")
        };

        return response;
    }

    public async Task<decimal> GetUserBalanceAsync(string userId)
    {
        string query = "SELECT CurrentBalance FROM Wallet WHERE UserId = @userId";
        return await _dbConnection
            .QuerySingleOrDefaultAsync<decimal>(query, new { userId });
    }

    public async Task<char> GetCurrencyAsync(string userId)
    {
        string query = "SELECT Currency FROM Wallet WHERE UserId = @userId";
        var value = await _dbConnection.QuerySingleOrDefaultAsync<int>(query, new { userId });
        
        var symbol = CurrencyHelper.GetCurrencyAsSymbol(value);
        return symbol;
    }

    public async Task<WalletBallanceRequest> CheckAvailableBalanceAsync(string userId)
    {
        string query = "SELECT CurrentBalance, BlockedAmount FROM Wallet WHERE UserId = @userId";
        var result = await _dbConnection
            .QuerySingleOrDefaultAsync<WalletBallanceRequest >(query, new { userId });

        return result ?? throw new InvalidOperationException("Such row does not exist.");
    }

    // Done
    public async Task<bool> DepositBalanceAsync(string userId, decimal balance)
    {
        var parameters = new DynamicParameters();
        parameters.Add("@UserId", userId, DbType.String);
        parameters.Add("@CurrentBalance", balance, DbType.Decimal);
        parameters.Add("@Status", dbType: DbType.String, size: 100, direction: ParameterDirection.Output);

        await _dbConnection.ExecuteAsync(
            "DepositWalletBalance",
            parameters,
            commandType: CommandType.StoredProcedure
            );

        var status = parameters.Get<string>("@Status");

        if (status == "Failed")
        {
            return false;
        }

        return true;
    }

    public async Task<bool> WithdrawBalanceAsync(string userId, decimal balance, decimal blockedAmount)
    {
        var parameters = new DynamicParameters();
        parameters.Add("@UserId", userId, DbType.String);
        parameters.Add("@CurrentBalance", balance, DbType.Decimal);
        parameters.Add("@BlockedAmount", blockedAmount, DbType.Decimal);
        parameters.Add("@Status", dbType: DbType.String, size: 100, direction: ParameterDirection.Output);

        await _dbConnection.ExecuteAsync(
            "WithdrawWalletBalance",
            parameters,
            commandType: CommandType.StoredProcedure
            );

        var status = parameters.Get<string>("@Status");

        if (status == "Failed")
        {
            return false;
        }

        return true;
    }
}
