using Dapper;
using MvcProject.DTO;
using System.Data;

namespace MvcProject.Data.Repositories;

public class TokenRepository : ITokenRepository
{
    private readonly IDbConnection _dbConnection;

    public TokenRepository(IDbConnection dbConnection)
    {
        _dbConnection = dbConnection;
    }

    public async Task<TokenResponse> CreatePublicTokenAsync(string userId)
    {
        var publicToken = Guid.NewGuid().ToString();

        var parameters = new DynamicParameters();
        parameters.Add("@UserId", userId, DbType.String, size: 450);
        parameters.Add("@PublicToken", publicToken, DbType.String, size: 36);

        parameters.Add("@StatusCode", dbType: DbType.Int32, direction: ParameterDirection.Output);
        parameters.Add("@ErrorMessage", dbType: DbType.String, size: 4000, direction: ParameterDirection.Output);

        await _dbConnection.ExecuteAsync(
            "CreatePublicToken",
            parameters,
            commandType: CommandType.StoredProcedure);

        var resultResponse = new TokenResponse
        {
            PublicToken = publicToken,
            StatusCode = parameters.Get<int>("@StatusCode"),
            ErrorMessage = parameters.Get<string?>("@ErrorMessage")
        };

        return resultResponse;
    }
}
