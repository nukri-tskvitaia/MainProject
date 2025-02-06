using Microsoft.AspNetCore.Identity;

namespace MvcProject.Models;

public class User : IdentityUser
{
    public ICollection<DepositWithdrawRequest>? DepositWithdraws { get; set; }
    public Wallet? Wallet { get; set; }
    public ICollection<Transaction>? Transactions { get; set; }
    public Tokens? Tokens { get; set; }
}