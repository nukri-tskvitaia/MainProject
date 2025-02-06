namespace CasinoApi.DTO;

public class UserInfoModel
{
    public string UserId { get; set; } = default!;
    public string UserName { get; set;} = default!;
    public string FirstName { get; set;} = "FirstName";
    public string LastName { get; set; } = "LastName";
    public string Email { get; set; } = default!;
    public string CountryCode {  get; set; } = "CC";
    public string CountryName { get; set; } = "Country";
    public int Gender { get; set; } = 0;
    public string? Currency { get; set; } = default!;
    public decimal CurrentBalance { get; set; }
}
