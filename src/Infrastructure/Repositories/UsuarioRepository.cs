using FCG.Users.Application.Interfaces.Repository;
using FCG.Users.Domain.Entities;
using FCG.Users.Infrastructure.Persistence;
using FCG.Users.Infrastructure.Repositories.EF;

namespace FCG.Users.Infrastructure.Repositories
{
    public class UsuarioRepository : EFRepository<Usuario>, IUsuarioRepository
    {
        public UsuarioRepository(ApplicationDbContext context) : base(context)
        {}
    }
}
