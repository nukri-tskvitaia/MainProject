namespace BankingApi.Helper;

public static class RequestValidator
{
    public static bool ValidateDeposit(List<string> merchants, decimal amount, string merchantId)
    {
        bool isMerchantValid = merchants.Contains(merchantId);
        bool isAmountValid = amount > 0.0m;

        return isMerchantValid && isAmountValid;
    }
}
