using BankingApi.Configuration;

namespace BankingApi.Services;

public static class DependencyRegister
{
    public static void Register(IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<MerchantSettings>(configuration.GetSection("MerchantSettings"));
        services.AddScoped<ICallbackService, CallbackService>();
    }
}
