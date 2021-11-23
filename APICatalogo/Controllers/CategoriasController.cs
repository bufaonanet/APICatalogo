using APICatalogo.DTOs;
using APICatalogo.Models;
using APICatalogo.Pagination;
using APICatalogo.Repository;
using APICatalogo.Services;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;

namespace APICatalogo.Controllers
{
    [Authorize]
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
   
    public class CategoriasController : ControllerBase
    {
        private readonly IUnitOfWork _uof;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly ILogger _logger;

        public CategoriasController(
            IUnitOfWork context,
            IConfiguration configuration,
            ILogger<CategoriasController> logger,
            IMapper mapper)
        {
            _uof = context;
            _configuration = configuration;
            _logger = logger;
            _mapper = mapper;
        }

        [HttpGet("autor")]
        public ActionResult<string> Autor()
        {
            var autor = _configuration["autor"];
            var conexao = _configuration["ConnectionStrings:SqliteConnectionString"];
            return $"Autor: {autor} - Conexao: {conexao}";
        }

        [HttpGet("saudacao/{nome}")]
        public ActionResult<string> Saudacoes([FromServices] IMeuServico meuServico, string nome)
        {
            return meuServico.Saudacao(nome);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CategoriaDTO>>> Get([FromQuery] CategoriasParameters categoriasParameters)
        {
            try
            {
                _logger.LogInformation("================ GET api/categorias ==============");

                var categorias = await _uof.CategoriaRepository.GetCategorias(categoriasParameters);

                var metadata = new
                {
                    categorias.TotalCount,
                    categorias.PageSize,
                    categorias.CurrentPage,
                    categorias.TotalPages,
                    categorias.HasNext,
                    categorias.HasPrevious
                };

                Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(metadata));

                var categoriasDto = _mapper.Map<List<CategoriaDTO>>(categorias);

                return categoriasDto;
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Erro ao acessar o obter Categoria");
            }
        }

        /// <summary>
        /// Obtem uma Categoria pleo seu Id
        /// </summary>
        /// <param name="id">Código da Categoria</param>
        /// <returns>Objeto Categoria</returns>
        [HttpGet("{id:int}", Name = "ObterCategoria")]
        [ProducesResponseType(typeof(CategoriaDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<CategoriaDTO>> Get(int id)
        {
            try
            {
                _logger.LogInformation($"================ GET api/categorias/{id} ==============");

                var categoria = await _uof.CategoriaRepository
                    .GetById(p => p.CategoriaId == id);

                if (categoria is null)
                {
                    _logger.LogInformation($"================ GET api/categorias/{id} NOT FOUND=============");
                    return NotFound($"Nenhuma Categoria para o Id={id}");
                }

                var categoriasto = _mapper.Map<CategoriaDTO>(categoria);

                return categoriasto;
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Erro ao acessar o banco");
            }
        }

        [HttpGet("produtos")]
        public async Task<ActionResult<IEnumerable<CategoriaDTO>>> GetCategoriasProdutos()
        {
            try
            {
                _logger.LogInformation("================ GET api/categorias/produtos ==============");

                var categorias = await _uof.CategoriaRepository.GetCategoriaProdutos();
                var categoriasDto = _mapper.Map<List<CategoriaDTO>>(categorias);

                return categoriasDto;
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Erro ao acessar o obter Categoria");
            }
        }

        /// <summary>
        /// Inclui uma nova categoria
        /// </summary>
        /// <remarks>
        /// Exemplo de request:
        ///
        ///     POST api/categorias
        ///     {
        ///        "categoriaId": 1,
        ///        "nome": "categoria1",
        ///        "imagemUrl": "http://teste.net/1.jpg"
        ///     }
        /// </remarks>
        /// <param name="categoriaDTO">objeto Categoria</param>
        /// <returns>O objeto Categoria incluida</returns>
        /// <remarks>Retorna um objeto Categoria incluído</remarks>

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ProdutoDTO>> Post([FromBody] CategoriaDTO categoriaDTO)
        {
            try
            {
                var categoria = _mapper.Map<Categoria>(categoriaDTO);
                _uof.CategoriaRepository.Add(categoria);
                await _uof.Commit();

                var categoriaDto = _mapper.Map<CategoriaDTO>(categoria);

                return new CreatedAtRouteResult("ObterCategoria", new { id = categoriaDto.CategoriaId }, categoriaDto);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Erro ao criar uma nova Categoria");
            }
        }

        /// <summary>
        /// Altera uma Categoria
        /// </summary>
        /// <param name="id"></param>
        /// <param name="categoriaDTO"></param>
        /// <returns>retorna 400 ou 200</returns>
        [HttpPut("{id:int}")]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Put))]
        public async Task<ActionResult> Put(int id, [FromBody] CategoriaDTO categoriaDTO)
        {
            try
            {
                if (id != categoriaDTO.CategoriaId)
                {
                    return BadRequest($"Não foi possível atualizar categoria com id={id}");
                }

                var categoria = _mapper.Map<Categoria>(categoriaDTO);

                _uof.CategoriaRepository.Update(categoria);
                await _uof.Commit();

                return Ok($"Categoria com id={id} atualizada com sucesso!");
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    $"Erro tentar atualizar Categoria com id={id}");
            }
        }

        /// <summary>
        /// Deleta uma Categoria
        /// </summary>
        /// <param name="id">codigo da categoria (int) </param>
        /// <returns></returns>
        [HttpDelete("{id:int}")]
        public async Task<ActionResult<CategoriaDTO>> Delete(int id)
        {
            try
            {
                var categoria = await _uof.CategoriaRepository
                    .GetById(p => p.CategoriaId == id);

                if (categoria == null)
                {
                    return NotFound($"Nenhuma Categoria para o Id={id}");
                }

                _uof.CategoriaRepository.Delete(categoria);
                await _uof.Commit();

                var categoriaDto = _mapper.Map<CategoriaDTO>(categoria);

                return categoriaDto;
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    $"Erro tentar excluir Categoria com id={id}");
            }
        }
    }
}