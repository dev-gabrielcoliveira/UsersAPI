using FCG.Users.Application.DTOs;
using FCG.Users.Application.Services;
using FCG.Users.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace FCG.Users.Controllers
{
    /// <summary>
    /// Gerenciamento dos usuários da plataforma.
    /// </summary>
    /// <remarks>
    /// Permite listar, buscar, cadastrar, atualizar e remover usuários.
    /// </remarks>
    [ApiController]
    [Route("api/[controller]")]
    public class UsuarioController : ControllerBase
    {
        private readonly UsuarioService _usuarioService;
        private readonly ILogger<UsuarioController> _logger;

        public UsuarioController(UsuarioService usuarioService, ILogger<UsuarioController> logger)
        {
            _usuarioService = usuarioService;
            _logger = logger;
        }

        [HttpGet]
        [Authorize(Policy = "Administrador")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<IEnumerable<Usuario>> ObterTodos()
        {
            try
            {
                var usuarios = _usuarioService.ObterTodos();
                return Ok(usuarios);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter todos os usuários.");
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Erro interno no servidor." });
            }
        }

        [HttpGet("{id:int}")]
        [Authorize(Policy = "AdministradorOuUsuario")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult ObterPorId([FromRoute, Range(1, int.MaxValue)] int id)
        {
            try
            {
                var usuario = _usuarioService.ObterPorId(id);
                if (usuario == null)
                    return NotFound(new { mensagem = "Usuário não encontrado." });

                return Ok(usuario);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter usuário com Id: {Id}", id);
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Erro interno no servidor." });
            }
        }

        [HttpPost]
        [Authorize(Policy = "Administrador")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Criar([FromBody] UsuarioCriarInput usuarioInput)
        {
            try
            {
                var usuario = await _usuarioService.Criar(usuarioInput);
                _logger.LogInformation("Usuário {Email} foi criado com sucesso.", usuarioInput.Email);

                return CreatedAtAction(nameof(ObterPorId), new { id = usuario.Id }, usuario);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { mensagem = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao incluir usuário: {Nome}", usuarioInput?.Nome);
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Erro interno no servidor." });
            }
        }

        [HttpPut]
        [Authorize(Policy = "Administrador")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult Atualizar(int id, [FromBody] UsuarioAtualizarInput usuarioInput)
        {
            if (id != usuarioInput.IdUsuario) // Impede que o usuário errado seja alterado
                return BadRequest(new { mensagem = "O ID da URL não corresponde ao ID do corpo da requisição." });

            try
            {
                var usuario = _usuarioService.ObterPorId(usuarioInput.IdUsuario);
                if (usuario == null)
                    return NotFound(new { mensagem = "Usuário não encontrado." });

                // Mapeia os dados do input para a entidade
                usuario.Nome = usuarioInput.Nome;
                usuario.Email = usuarioInput.Email;
                usuario.Senha = usuarioInput.Senha;

                _usuarioService.Alterar(usuario);
                _logger.LogInformation("Usuário {Id} foi alterado com sucesso.", usuarioInput.IdUsuario);

                return NoContent();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { mensagem = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao alterar usuário com Id: {Id}", usuarioInput.IdUsuario);
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Erro interno no servidor." });
            }
        }

        [HttpDelete("{id:int}")]
        [Authorize(Policy = "Administrador")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult Remover([FromRoute, Range(1, int.MaxValue)] int id)
        {
            try
            {
                var usuario = _usuarioService.ObterPorId(id);
                if (usuario == null)
                    return NotFound(new { mensagem = "Usuário não encontrado." });

                _usuarioService.Excluir(id);
                _logger.LogInformation("Usuário {Id} foi removido.", id);

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao excluir usuário com Id: {Id}", id);
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Erro interno no servidor." });
            }
        }
    }
}