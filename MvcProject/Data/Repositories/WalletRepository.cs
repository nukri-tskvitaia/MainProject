using Dapper;
using MvcProject.Helper;
using MvcProject.Models;
using System.Data;

namespace MvcProject.Data.Repositories;

public class WalletRepository : IWalletRepository
{
    private readonly IDbConnection _dbConnection;

    public WalletRepository(IDbConnection dbConnection)
    {
        _dbConnection = dbConnection;
    }

    public async Task<Guid> CreateWalletAsync(Wallet wallet)
    {
        string query = @"
            INSERT INTO Wallet (Id, UserId, CurrentBalance, Currency)
            VALUES (@Id, @UserId, @CurrentBalance, @Currency)
            SELECT CAST(SCOPE_IDENTITY() as int)";

        return await _dbConnection.ExecuteScalarAsync<Guid>(query, new
        {
            wallet.Id,
            wallet.UserId,
            wallet.CurrentBalance,
            wallet.Currency,
        });
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
}
