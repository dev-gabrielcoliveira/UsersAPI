namespace FCG.Users.Application.Interfaces.Services
{
    public interface IAuthService
    {
        string GenerateToken(IConfiguration _configuration, string username, string role);
    }
}
