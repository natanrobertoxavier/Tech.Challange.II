using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Tech.Challenge.I.Application.Services.Token;
public class TokenController(
    double tokenLifetimeInMinutes,
    string securityKey)
{
    private const string EmailAlias = "eml";
    private readonly double _tokenLifetimeInMinutes = tokenLifetimeInMinutes;
    private readonly string _securitykey = securityKey;

    public string GenerateToken(string userEmail)
    {
        var claims = new List<Claim>
        {
            new Claim(EmailAlias, userEmail)
        };

        var tokenHandler = new JwtSecurityTokenHandler();

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(_tokenLifetimeInMinutes),
            SigningCredentials = new SigningCredentials(SimetricKey(), SecurityAlgorithms.HmacSha256Signature)
        };

        var securityToken = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(securityToken);

    }

    public ClaimsPrincipal ValidateToken(string token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();

        var validationParameters = new TokenValidationParameters
        {
            RequireExpirationTime = true,
            IssuerSigningKey = SimetricKey(),
            ClockSkew = new TimeSpan(0),
            ValidateIssuer = false,
            ValidateAudience = false,
        };

        var claims = tokenHandler.ValidateToken(token, validationParameters, out _);
        return claims;
    }

    public string RecoverEmail(string token)
    {
        var claims = ValidateToken(token);
        return claims.FindFirst(EmailAlias).Value;
    }

    private SymmetricSecurityKey SimetricKey()
    {
        var simmetricKey = Convert.FromBase64String(_securitykey);

        return new SymmetricSecurityKey(simmetricKey);
    }
}
