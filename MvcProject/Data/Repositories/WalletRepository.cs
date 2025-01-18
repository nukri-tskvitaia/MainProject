using Dapper;
using System.Data;

namespace MvcProject.Data.Repositories;

public class WalletRepository : IWalletRepository
{
    private readonly IDbConnection _dbConnection;

    public WalletRepository(IDbConnection dbConnection)
    {
        _dbConnection = dbConnection;
    }

    public async Task<decimal> GetUserBalanceAsync(string userId)
    {
        string query = "SELECT CurrentBalance FROM Wallet WHERE UserId = @userId";
        return await _dbConnection
            .QuerySingleOrDefaultAsync<decimal>(query, new { userId });
    }
}
