using APICatalogo.Context;
using System;
using System.Threading.Tasks;

namespace APICatalogo.Repository
{
    public class UnitOfWork : IUnitOfWork, IDisposable
    {
        private ProdutoRepository _produtoRepo;
        private CategoriaRepository _categoriaRepo;
        public AppDbContext _context;

        public UnitOfWork(AppDbContext context)
        {
            _context = context;
        }

        public IProdutoRepository ProdutoRepository
            => _produtoRepo ?? new ProdutoRepository(_context);

        public ICategoriaRepository CategoriaRepository
            => _categoriaRepo ?? new CategoriaRepository(_context);

        public async Task Commit()
        {
            await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}