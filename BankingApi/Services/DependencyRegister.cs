using BankingApi.Configuration;

namespace BankingApi.Services;

public static class DependencyRegister
{
    public static void Register(IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<List<MerchantSettings>>(configuration.GetSection("Secrets:MerchantSettings"));
        services.AddScoped<ICallbackService, CallbackService>();
    }
}
