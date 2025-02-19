﻿using System.Security.Cryptography;
using System.Text;

namespace BankingApi.Helper;

public static class HashGenerator
{
    public static string GenerateHash(decimal amount, string merchantId, int depositWithdrawId, string key)
    {
        string data = $"{amount}{merchantId}{depositWithdrawId}{key}";
        using var sha256 = SHA256.Create();

        return Convert.ToHexString(sha256.ComputeHash(Encoding.UTF8.GetBytes(data)));
    }

    /*
    public static string GenerateWithdrawHash(decimal amount, string merchantId, string transactionId, string accountNumber, string fullName, string key)
    {
        string data = $"{amount}{merchantId}{transactionId}{accountNumber}{fullName}{key}";
        using var sha256 = SHA256.Create();

        return Convert.ToHexString(sha256.ComputeHash(Encoding.UTF8.GetBytes(data)));
    } */
}
