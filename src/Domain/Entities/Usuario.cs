using FCG.Users.Application.Interfaces.Base;

namespace FCG.Users.Domain.Entities
{
    public class Usuario : EntityBase
    {
        public required string Nome { get; set; }

        public required string Email { get; set; }

        public required string Senha { get; set; }

        public required string Situacao { get; set; }

    }
}
