using MvcProject.DTO;
using MvcProject.Helper;

namespace MvcProject.Services;

public class BankingApiService : IBankingApiService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;

    public BankingApiService(IHttpClientFactory httpClientFactory, IConfiguration configuration)
    {

        _httpClientFactory = httpClientFactory;
        _configuration = configuration;
    }

    // Done
    public async Task<DepositBankingApiResponse> DepositBankingApiAsync(string depositWithdrawId, decimal amount)
    {
        var hash = HashGenerator.GenerateHash(
            amount, _configuration["Secrets:MerchantId"]!,
            depositWithdrawId, _configuration["Secrets:Key"]!);

        var httpClient = _httpClientFactory.CreateClient();
        var response = await httpClient.PostAsJsonAsync(
            $"{_configuration["Secrets:BankingApiBaseUrl"]}/Deposit/StartDeposit",
            new DepositRequest
            {
                Id = depositWithdrawId,
                Amount = amount,
                MerchantId = _configuration["Secrets:MerchantId"]!,
                Hash = hash
            });

        var responseContent = await response.Content.ReadFromJsonAsync<DepositBankingApiResponse>();
        return responseContent!;
    }

    public async Task<WithdrawBankingApiResponse> WithdrawBankingApiAsync(string depositWithdrawId, decimal amount)
    {
        var hash = HashGenerator.GenerateHash(
            amount, _configuration["Secrets:MerchantId"]!,
            depositWithdrawId, _configuration["Secrets:Key"]!);

        var httpClient = _httpClientFactory.CreateClient();
        var response = await httpClient.PostAsJsonAsync(
            $"{_configuration["Secrets:BankingApiBaseUrl"]}/Withdraw/StartWithdraw",
            new WithdrawRequest
            {
                Id = depositWithdrawId,
                Amount = amount,
                MerchantId = _configuration["Secrets:MerchantId"]!,
                Hash = hash
            });

        var responseContent = await response.Content.ReadFromJsonAsync<WithdrawBankingApiResponse>();
        return responseContent!;
    }
}
