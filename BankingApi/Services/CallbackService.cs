using BankingApi.DTO;

namespace BankingApi.Services;

public class CallbackService : ICallbackService
{
    private readonly IHttpClientFactory _httpClientFactory;

    public CallbackService(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public async Task<bool> NotifyMvcAsync(string url, MvcCallbackResponse data)
    {
        var httpClient = _httpClientFactory.CreateClient("BankingApiClient");
        var response = await httpClient.PostAsJsonAsync(url, data);
        
        return response.IsSuccessStatusCode;
    }
}
