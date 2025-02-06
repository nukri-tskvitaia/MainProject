using CasinoApi.DTO;
using Dapper;
using System.Data;

namespace CasinoApi.Repositories;

public class TokenRepository : ITokenRepository
{
    private readonly IDbConnection _dbConnection;

    public TokenRepository(IDbConnection dbConnection)
    {
        _dbConnection = dbConnection;
    }

    public async Task<TokenResponse> AuthorizeAsync(TokenRequest request)
    {
        var privateToken = Guid.NewGuid().ToString();

        var parameters = new DynamicParameters();
        parameters.Add("@PublicToken", request.PublicToken, DbType.String, size: 36);
        parameters.Add("@PrivateToken", privateToken, DbType.String, size: 36);

        parameters.Add("@StatusCode", dbType: DbType.Int32, direction: ParameterDirection.Output);
        parameters.Add("@ErrorMessage", dbType: DbType.String, size: 4000, direction: ParameterDirection.Output);

        await _dbConnection.ExecuteAsync(
            "CreatePrivateToken",
            parameters,
            commandType: CommandType.StoredProcedure);

        var resultResponse = new TokenResponse
        {
            Data = new PrivateTokenResponse { PrivateToken = privateToken },
            StatusCode = parameters.Get<int>("@StatusCode"),
            ErrorMessage = parameters.Get<string?>("@ErrorMessage")
        };

        return resultResponse;
    }
}
