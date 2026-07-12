

using FCG.Users.Application.Interfaces.Base;

namespace FCG.Users.Application.Interfaces
{
    public interface IRepository<T> where T : EntityBase
    {

        void Alterar(T entidade);

        IList<T> ObterTodos();

        T? ObterPorId(int id);

        void Cadastrar(T entidade);

        void Deletar(int id);
    }
}
