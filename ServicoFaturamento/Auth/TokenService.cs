using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace ServicoFaturamento.Auth;

public class TokenService
{
    private readonly JwtSettings _settings;
    public TokenService(JwtSettings settings)
    {
        _settings = settings;
    }

public string GerarToken(string usuario, string perfil)
    {
        var chave = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.SecretKey));

        var credenciais = new SigningCredentials(
            chave, SecurityAlgorithms.HmacSha256);
        
        var claims = new[]
        {
            new Claim(ClaimTypes.Name, usuario),
            new Claim(ClaimTypes.Role, perfil),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var token = new JwtSecurityToken(
            issuer: _settings.Issuer,
            audience: _settings.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddHours(_settings.ExpirationHours),
            signingCredentials: credenciais
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
};