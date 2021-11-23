using APICatalogo.Context;
using APICatalogo.Models;
using APICatalogo.Pagination;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace APICatalogo.Repository
{
    public class CategoriaRepository : Repository<Categoria>, ICategoriaRepository
    {
        public CategoriaRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Categoria>> GetCategoriaProdutos()
        {
            return await Get().Include(p => p.Produtos).ToListAsync();
        }

        public async Task<PagedList<Categoria>> GetCategorias(CategoriasParameters categoriasParameters)
        {
            return await PagedList<Categoria>.ToPagedListAsync(
                Get().OrderBy(p => p.Nome),
                categoriasParameters.PageNumber,
                categoriasParameters.PageSize);
        }
    }
}