using FCG.Users.Application.Interfaces;
using FCG.Users.Application.Interfaces.Base;
using FCG.Users.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace FCG.Users.Infrastructure.Repositories.EF
{
    public class EFRepository<T> : IRepository<T> where T : EntityBase
    {

        protected ApplicationDbContext _context;
        protected DbSet<T> _dbSet;

        public EFRepository(ApplicationDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<T>();
        }

        public void Alterar(T entidade)
        {
            _dbSet.Update(entidade);
            _context.SaveChanges();
        }

        public void Cadastrar(T entidade)
        {
            _dbSet.Add(entidade);
            _context.SaveChanges();
        }

        public void Deletar(int id)
        {
            var entity = ObterPorId(id);
            if (entity != null)
            {
                _dbSet.Remove(entity);
                _context.SaveChanges();
            }
        }

        public T? ObterPorId(int id)
            => _dbSet.FirstOrDefault(entity => entity.Id == id);

        public IList<T> ObterTodos()
            => _dbSet.ToList();
    }
}
