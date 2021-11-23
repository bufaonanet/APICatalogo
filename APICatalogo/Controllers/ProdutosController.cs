using APICatalogo.DTOs;
using APICatalogo.Models;
using APICatalogo.Pagination;
using APICatalogo.Repository;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;

namespace APICatalogo.Controllers
{   
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    public class ProdutosController : ControllerBase
    {
        private readonly IUnitOfWork _uof;
        private readonly IMapper _mapper;

        public ProdutosController(IUnitOfWork context, IMapper mapper)
        {
            _uof = context;
            _mapper = mapper;
        }

        [HttpGet("primeiro")]
        [HttpGet("/primeiro")]
        public async Task<ActionResult<ProdutoDTO>> Primeiro()
        {
            var produto = await _uof.ProdutoRepository.Get().FirstOrDefaultAsync();
            var produtosDto = _mapper.Map<ProdutoDTO>(produto);
            return produtosDto;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProdutoDTO>>> Get([FromQuery] ProdutosParameters produtosParameters)
        {
            var produtos = await _uof.ProdutoRepository.GetProdutosAsync(produtosParameters);

            var metadata = new
            {
                produtos.TotalCount,
                produtos.PageSize,
                produtos.CurrentPage,
                produtos.TotalPages,
                produtos.HasNext,
                produtos.HasPrevious
            };

            Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(metadata));

            var produtosDto = _mapper.Map<List<ProdutoDTO>>(produtos);
            return produtosDto;
        }

        /// <summary>
        /// Obtem o produto pelo seu identificado produtoId
        /// </summary>
        /// <param name="id">Código do produto</param>
        /// <returns>Um objeto Produto</returns>
        [HttpGet("{id}", Name = "ObterProduto")]
        public async Task<ActionResult<ProdutoDTO>> Get(int id)
        {
            var produto = await _uof.ProdutoRepository.GetById(p => p.ProdutoId == id);

            if (produto is null)
            {
                return NotFound();
            }

            var produtoDto = _mapper.Map<ProdutoDTO>(produto);

            return produtoDto;
        }

        [HttpGet("menorpreco")]
        public async Task<ActionResult<IEnumerable<ProdutoDTO>>> GetProdutosPrecos()
        {
            var produtos = await _uof.ProdutoRepository.GetProdutosPorPrecoAsync();
            var produtosDto = _mapper.Map<List<ProdutoDTO>>(produtos);
            return produtosDto;
        }

        /// <summary>
        /// Incluir um novo produto
        /// </summary>
        /// <param name="produtoDto"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult<ProdutoDTO>> Post([FromBody] ProdutoDTO produtoDto)
        {
            var produto = _mapper.Map<Produto>(produtoDto);
            _uof.ProdutoRepository.Add(produto);
            await _uof.Commit();

            var produtosDto = _mapper.Map<ProdutoDTO>(produto);

            return new CreatedAtRouteResult("ObterProduto", new { id = produtosDto.ProdutoId }, produtoDto);
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult> Put(int id, [FromBody] ProdutoDTO produtoDTO)
        {
            if (id != produtoDTO.ProdutoId)
            {
                return BadRequest();
            }

            var produto = _mapper.Map<Produto>(produtoDTO);

            _uof.ProdutoRepository.Update(produto);
            await _uof.Commit();

            return Ok();
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult<ProdutoDTO>> Delete(int id)
        {
            var produto = await _uof.ProdutoRepository.GetById(p => p.ProdutoId == id);

            if (produto == null)
            {
                return NotFound();
            }

            _uof.ProdutoRepository.Delete(produto);
            await _uof.Commit();

            var produtoDTO = _mapper.Map<ProdutoDTO>(produto);

            return produtoDTO;
        }
    }
}