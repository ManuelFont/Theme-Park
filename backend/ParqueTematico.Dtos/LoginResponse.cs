namespace Dtos;

public sealed class LoginResponse(string token, DateTime expiraEn)
{
    public string Token { get; } = token;
    public DateTime ExpiraEn { get; } = expiraEn;
}
