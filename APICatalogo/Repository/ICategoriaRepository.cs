using APICatalogo.Models;
using APICatalogo.Pagination;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace APICatalogo.Repository
{
    public interface ICategoriaRepository : IRepository<Categoria>
    {
        Task<IEnumerable<Categoria>> GetCategoriaProdutos();
        Task<PagedList<Categoria>> GetCategorias(CategoriasParameters categoriasParameters);
    }
}