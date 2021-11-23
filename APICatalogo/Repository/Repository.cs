using APICatalogo.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace APICatalogo.Repository
{
    public abstract class Repository<T> : IRepository<T> where T : class
    {
        protected readonly AppDbContext _context;

        protected Repository(AppDbContext context)
        {
            _context = context;
        }

        public IQueryable<T> Get()
        {
            return _context.Set<T>().AsNoTracking();
        }

        public async Task<T> GetById(Expression<Func<T, bool>> predicate)
        {
            return await _context.Set<T>().SingleOrDefaultAsync(predicate);
        }

        public void Add(T entity)
        {
            _context.Set<T>().Add(entity);
        }

        public void Update(T entity)
        {
            _context.Entry(entity).State = EntityState.Modified;
            _context.Set<T>().Update(entity);
        }

        public void Delete(T entity)
        {
            _context.Set<T>().Remove(entity);
        }
    }
}