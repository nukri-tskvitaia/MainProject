using BankingApi.Configuration;

namespace BankingApi.Helper;

public static class RequestValidator
{
    public static bool ValidateDeposit(List<MerchantSettings> merchants, decimal amount, string merchantId)
    {
        bool isMerchantValid = merchants.Any(x => x.MerchantId == merchantId);
        bool isAmountValid = amount > 0.0m;

        return isMerchantValid && isAmountValid;
    }
}
