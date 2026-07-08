using FCG.Users.Application.DTOs;
using FCG.Users.Application.Interfaces.Repository;
using FCG.Users.Domain.Entities;
using FCG.Contracts;
using FCG.Contracts.Events;
using FCG.Users.Application.Validators;
using MassTransit;

namespace FCG.Users.Application.Services
{
    public class UsuarioService
    {
        private readonly IUsuarioRepository _repository;
        private readonly UsuarioValidators _validator;
        private readonly IPublishEndpoint _publichEndpoint;

        public UsuarioService(
            IUsuarioRepository repository,
            IPublishEndpoint publichEndpoint
        )
        {
            _repository = repository;
            _publichEndpoint = publichEndpoint;
            _validator = new UsuarioValidators();

        }

        public void Alterar(Usuario usuario)
        {

            if (!_validator.EmailValido(usuario.Email))
                throw new Exception("Email inválido");

            if (!_validator.SenhaValida(usuario.Senha))
                throw new Exception("Senha fraca");

            if (!_validator.TamanhoMaximo(usuario.Nome, 50))
                throw new Exception("Nome excede o tamanho máximo");

            if (!_validator.TamanhoMaximo(usuario.Email, 100))
                throw new Exception("E-mail excede o tamanho máximo");

            if (!_validator.TamanhoMaximo(usuario.Senha, 32))
                throw new Exception("Senha excede o tamanho máximo");

            _repository.Alterar(usuario);

        }

        public async Task<Usuario> Criar(UsuarioCriarInput input)
        {
            if (!_validator.EmailValido(input.Email))
                throw new Exception("Email inválido");

            if (!_validator.SenhaValida(input.Senha))
                throw new Exception("Senha fraca");

            if (!_validator.TamanhoMaximo(input.Nome, 50))
                throw new Exception("Nome excede o tamanho máximo");

            if (!_validator.TamanhoMaximo(input.Email, 100))
                throw new Exception("E-mail excede o tamanho máximo");

            if (!_validator.TamanhoMaximo(input.Senha, 32))
                throw new Exception("Senha excede o tamanho máximo");

            var usuario = new Usuario
            {
                Nome = input.Nome,
                Email = input.Email,
                Senha = input.Senha,
                Situacao = "Ativo"
            };

            _repository.Cadastrar(usuario);

            await _publichEndpoint.Publish(
                new UserCreatedEvent(
                    usuario.Id,
                    usuario.Nome,
                    usuario.Email
                ));

            return usuario;
        }

        public void Excluir(int id)
        {
            var usuario = this.ObterPorId(id);

            if (usuario == null)
                throw new Exception("Usuário não encontrado");

            usuario.Situacao = "Removido";

            _repository.Alterar(usuario);
        }

        public Usuario? ObterPorEmail(string email)
        {

            // Como existe o campo situação é necessário buscar por e-mail e pela situação Ativo.

            var usuarios = _repository.ObterTodos();

            var usuario = usuarios.Where(ent => ent.Situacao == "Ativo" && ent.Email == email)
                .FirstOrDefault();

            return usuario;
        }

        public Usuario? ObterPorId(int id)
        {

            // Como existe o campo situação é necessário buscar por id e pela situação Ativo.

            var usuario = _repository.ObterTodos()
                .Where(ent => ent.Situacao == "Ativo" && ent.Id == id)
                .FirstOrDefault();

            return usuario;
        }

        public IEnumerable<Usuario> ObterTodos()
        {
            return _repository.ObterTodos()
                .Where(ent => ent.Situacao == "Ativo");
        }

    }
}
