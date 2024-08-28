using Tech.Challenge.I.Application.Services.Token;

namespace Tech.Challenge.I.Tests.Mock;
public class MockTokenController(
    double tokenLifetimeInMinutes,
    string securityKey) : TokenController(
        tokenLifetimeInMinutes,
        securityKey)
{
    public string GenerateToken(string email)
    {
        return "mocked-token";
    }
}
