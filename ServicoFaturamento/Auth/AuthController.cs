using Microsoft.AspNetCore.Mvc;

namespace ServicoFaturamento.Auth;

[ApiController]
[Route("auth")]
public class AuthController : ControllerBase
{
    private readonly TokenService _tokenService;

    //Usuários hardcoded por ora - em produção viria do banco
    private static readonly Dictionary<string, string> _perfis = new()
    {
        { "admin", "admin123" },
        { "operador", "op123" }
    };

    private static readonly Dictionary<string, string> _perfil = new()
    {
        { "admin", "Administrador" },
        { "operador", "Operador" }
    };

    public AuthController(TokenService tokenService)
    {
        _tokenService = tokenService;
    }

    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginRequest request)
    {
        if(!_perfis.TryGetValue(request.Usuario, out var senha)
        || senha != request.Senha)
        {
            return Unauthorized(new {mensagem = "Usuário ou senha inválidos."});
        }

        var token = _tokenService.GerarToken(
            request.Usuario,
            _perfil[request.Usuario]);
        
        return Ok(new
        {
            token,
            expiraEm = DateTime.UtcNow.AddHours(8),
            perfil = _perfis[request.Usuario]
        });
    }
}

public record LoginRequest(string Usuario, string Senha);