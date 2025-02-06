using MvcProject.DTO;

namespace MvcProject.Data.Repositories;

public interface ITokenRepository
{
    public Task<TokenResponse> CreatePublicTokenAsync(string userId);
}
