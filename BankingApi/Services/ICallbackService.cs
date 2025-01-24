using BankingApi.DTO;

namespace BankingApi.Services;

public interface ICallbackService
{
    public Task<CallbackServiceModel?> NotifyMvcAsync(string url, MvcCallbackResponse data);
}
