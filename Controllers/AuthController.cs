using FCG.Users.Application.Interfaces.Services;
using FCG.Users.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace FCG.Users.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthController : Controller
    {

        private readonly IConfiguration _configuration;
        private readonly IAuthService _authService;
        private readonly UsuarioService _usuarioService;
        private readonly ILogger<UsuarioController> _logger;

        public AuthController(
            IConfiguration configuration, 
            IAuthService authService, 
            UsuarioService usuarioService, 
            ILogger<UsuarioController> logger
        )
        {
            _configuration = configuration;
            _authService = authService;
            _usuarioService = usuarioService;
            _logger = logger;
        }

        /// <summary>
        /// Realiza a autenticação do usuário.
        /// </summary>
        /// <returns>Token de autenticação.</returns>
        /// <response code="200">Autenticação realizada com sucesso</response>
        /// <response code="401">Credenciais inválidas</response>
        [HttpPost]
        public IActionResult Login([FromQuery] string email, string senha, string papel)
        {

            _logger.LogInformation("Tentativa de login para o email {Email}", email);
            // Para autenticar precisa de um usuário ativo que esteja cadastrado no banco de dados.
            var usuario = _usuarioService.ObterPorEmail(email);

            if (usuario == null || usuario.Senha != senha)
            {
                _logger.LogWarning("Login falhou para o email {Email}", email);
                return Unauthorized();
            }
            else
            {
                // Considerando o email como login do usuário.

                _logger.LogInformation("Login realizado com sucesso para {Email}", email);
                var token = _authService.GenerateToken(_configuration, email, papel);

                return Ok(token);
            }

        }

    }
}
