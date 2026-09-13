

using FCG.Users.Application.Interfaces.Base;
using System.Linq.Expressions;

namespace FCG.Users.Application.Interfaces
{
    public interface IRepository<T> where T : EntityBase
    {

        void Alterar(T entidade);
        Task<T?> ObterPrimeiroAsync(Expression<Func<T, bool>> predicate);

        IList<T> ObterTodos();

        T? ObterPorId(int id);

        void Cadastrar(T entidade);

        void Deletar(int id);
    }
}
