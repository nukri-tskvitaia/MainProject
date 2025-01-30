using Microsoft.AspNetCore.Identity.UI.Services;
using MvcProject.Configuration;
using MvcProject.Data.Repositories;

namespace MvcProject.Services;

public static class DependencyRegister
{
    public static void Register(IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<MailSettings>(configuration.GetSection("Mail"));
        services.Configure<List<ClientSettings>>(configuration.GetSection("Secrets:ClientSettings"));

        services.AddTransient<IEmailSender, EmailSenderService>();
        services.AddScoped<IDepositWithdrawService, DepositWithdrawService>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IWalletRepository, WalletRepository>();
        services.AddScoped<IDepositWithdrawRequestRepository, DepositWithdrawRequestRepository>();
        services.AddScoped<IBankingApiService, BankingApiService>();
        services.AddScoped<ITransactionRepository, TransactionRepository>();
        services.AddScoped<IProcessRepository, ProcessRepository>();
    }
}
