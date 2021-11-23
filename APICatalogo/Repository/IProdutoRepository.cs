using APICatalogo.Models;
using APICatalogo.Pagination;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace APICatalogo.Repository
{
    public interface IProdutoRepository : IRepository<Produto>
    {
        Task<IEnumerable<Produto>> GetProdutosPorPrecoAsync();
        Task<PagedList<Produto>> GetProdutosAsync(ProdutosParameters produtosParameters);
    }
}