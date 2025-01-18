using Dapper;
using System.Data;

namespace MvcProject.Data.Repositories;

public class UserRepository : IUserRepository
{
    private readonly IDbConnection _dbConnection;

    public UserRepository(IDbConnection dbConnection)
    {
        _dbConnection = dbConnection;
    }

    public async Task<string> GetUsernameAsync(string userId)
    {
        string query = "SELECT UserName FROM Users WHERE Id = @userId";
        return await _dbConnection.QuerySingleOrDefaultAsync<string>(query, new { userId });
    }
}
