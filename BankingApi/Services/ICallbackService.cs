using BankingApi.DTO;

namespace BankingApi.Services;

public interface ICallbackService
{
    public Task<bool> NotifyMvcAsync(string url, MvcCallbackResponse data);
}
