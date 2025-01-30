using BankingApi.Configuration;

namespace BankingApi.Helper;

public static class RequestValidator
{
    public static bool ValidateMerchant(List<MerchantSettings> merchants, string merchantId)
    {
        bool isMerchantValid = merchants.Any(x => x.MerchantId == merchantId);

        return isMerchantValid;
    }
}
