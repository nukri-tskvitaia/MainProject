using CasinoApi.DTO;

namespace CasinoApi.Repositories;

public interface ITokenRepository
{
    public Task<TokenResponse> AuthorizeAsync(TokenRequest request);
}
