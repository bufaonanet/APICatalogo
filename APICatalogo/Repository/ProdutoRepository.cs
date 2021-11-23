using APICatalogo.Context;
using APICatalogo.Models;
using APICatalogo.Pagination;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace APICatalogo.Repository
{
    public class ProdutoRepository : Repository<Produto>, IProdutoRepository
    {
        public ProdutoRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<PagedList<Produto>> GetProdutosAsync(ProdutosParameters produtosParameters)
        {
            //return Get()
            //    .OrderBy(p => p.Nome)
            //    .Skip((produtosParameters.PageNumber - 1) * produtosParameters.PageSize)
            //    .Take(produtosParameters.PageSize)
            //    .ToList();

            return await PagedList<Produto>.ToPagedListAsync(
                Get().OrderBy(p => p.ProdutoId),
                produtosParameters.PageNumber,
                produtosParameters.PageSize);
        }

        public async Task<IEnumerable<Produto>> GetProdutosPorPrecoAsync()
        {
            return await Get().OrderBy(p => ((double)p.Preco)).ToListAsync();
        }
    }
}