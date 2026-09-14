using Azure.Storage.Queues;
using FCG.Users.Application.DTOs;
using FCG.Users.Application.Interfaces.Repository;
using FCG.Users.Application.Validators;
using FCG.Users.Domain.Entities;
using System.Text;
using System.Text.Json;

namespace FCG.Users.Application.Services
{
    public class UsuarioService
    {
        private readonly IUsuarioRepository _repository;
        private readonly UsuarioValidators _validator;
        private readonly QueueServiceClient _queueServiceClient;

        public UsuarioService(
            IUsuarioRepository repository,
            QueueServiceClient queueServiceClient
        )
        {
            _repository = repository;
            _validator = new UsuarioValidators();
            _queueServiceClient = queueServiceClient;

        }

        public void Alterar(Usuario usuario)
        {

            if (!_validator.EmailValido(usuario.Email))
                throw new ArgumentException("Email inválido");

            if (!_validator.SenhaValida(usuario.Senha))
                throw new ArgumentException("Senha fraca");

            if (!_validator.TamanhoMaximo(usuario.Nome, 50))
                throw new ArgumentException("Nome excede o tamanho máximo");

            if (!_validator.TamanhoMaximo(usuario.Email, 100))
                throw new ArgumentException("E-mail excede o tamanho máximo");

            if (!_validator.TamanhoMaximo(usuario.Senha, 32))
                throw new ArgumentException("Senha excede o tamanho máximo");

            _repository.Alterar(usuario);

        }

        public async Task<Usuario> Criar(UsuarioCriarInput input)
        {
            if (!_validator.EmailValido(input.Email))
                throw new ArgumentException("Email inválido");

            if (!_validator.SenhaValida(input.Senha))
                throw new ArgumentException("Senha fraca");

            if (!_validator.TamanhoMaximo(input.Nome, 50))
                throw new ArgumentException("Nome excede o tamanho máximo");

            if (!_validator.TamanhoMaximo(input.Email, 100))
                throw new ArgumentException("E-mail excede o tamanho máximo");

            if (!_validator.TamanhoMaximo(input.Senha, 32))
                throw new ArgumentException("Senha excede o tamanho máximo");

            var usuario = new Usuario
            {
                Nome = input.Nome,
                Email = input.Email,
                Senha = input.Senha,
                Situacao = "Ativo"
            };

            _repository.Cadastrar(usuario);

            string mensagem = $"{input.Email} | Bem-vindo! |Olá {input.Nome}, seu usuário foi criado com sucesso.";

            var queueClient = new QueueClient(
                "UseDevelopmentStorage=true",
                "notifications-v3",
                new QueueClientOptions { MessageEncoding = QueueMessageEncoding.Base64 }
            );

            await queueClient.CreateIfNotExistsAsync();
            await queueClient.SendMessageAsync(mensagem);

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

        public async Task<Usuario?> ObterPorEmail(string email)
        {

            // Como existe o campo situação é necessário buscar por e-mail e pela situação Ativo.
            var usuario = _repository.ObterTodos().Where(u => u.Situacao == "Ativo" && u.Email == email).FirstOrDefault();

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
