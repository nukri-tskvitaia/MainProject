using BankingApi.DTO;

namespace BankingApi.Services;

public class CallbackService : ICallbackService
{
    private readonly IHttpClientFactory _httpClientFactory;

    public CallbackService(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public async Task<CallbackServiceModel?> NotifyMvcAsync(string url, MvcCallbackResponse data)
    {
        var httpClient = _httpClientFactory.CreateClient("BankingApiClient");
        var response = await httpClient.PostAsJsonAsync(url, data);

        var result = await response.Content.ReadFromJsonAsync<CallbackServiceModel?>();
        
        return result;
    }
}
