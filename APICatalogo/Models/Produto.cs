using APICatalogo.Validations;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace APICatalogo.Models
{
    [Table("Produtos")]
    public class Produto
    {
        [Key]
        public int ProdutoId { get; set; }

        [Required(ErrorMessage = "O {0} é obrigatório")]
        [StringLength(20, ErrorMessage = "O {0} deve ter entre {2} e {1} caracteres", MinimumLength = 5)]
        [PrimeiraLetraMaiuscula]
        public string Nome { get; set; }

        [Required]
        [MaxLength(300)]
        public string Descricao { get; set; }

        [Required(ErrorMessage = "O {0} é obrigatório")]
        [DataType(DataType.Currency)]
        [Column(TypeName = "decimal(8,2)")]
        [Range(1, 10000, ErrorMessage = "O {0} deve estar entre {1} e {2}")]
        public decimal Preco { get; set; }

        [Required]
        [MaxLength(500)]
        public string ImagemUrl { get; set; }

        public float Estoque { get; set; }
        public DateTime DataCadastro { get; set; }

        public int CategoriaId { get; set; }

        [System.Text.Json.Serialization.JsonIgnore]
        public Categoria Categoria { get; set; }
    }
}