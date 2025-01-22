using Microsoft.AspNetCore.Identity.UI.Services;
using MvcProject.Data.Repositories;

namespace MvcProject.Services;

public static class DependencyRegister
{
    public static void Register(IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<MailSettings>(configuration.GetSection("Mail"));

        services.AddTransient<IEmailSender, EmailSenderService>();
        services.AddScoped<IDepositWithdrawService, DepositWithdrawService>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IWalletRepository, WalletRepository>();
        services.AddScoped<IDepositWithdrawRequestRepository, DepositWithdrawRequestRepository>();
        services.AddScoped<IBankingApiService, BankingApiService>();
    }
}
