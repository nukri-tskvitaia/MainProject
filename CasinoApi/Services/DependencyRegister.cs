using CasinoApi.Repositories;

namespace CasinoApi.Services;

public static class DependencyRegister
{
    public static void Register(IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<ICasinoRepository, CasinoRepository>();
        services.AddScoped<ITokenRepository, TokenRepository>();
    }
}
