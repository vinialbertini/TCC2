using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BlazorApp1.Data
{
    public partial class Produto
    {
        public int Id { get; set; }

        [ForeignKey("Empresa")]
        public int Id_Empresa { get; set; }

        [Required(ErrorMessage = "A Descrição do produto é obrigatória")]
        public string? Descricao { get; set; }
        public string? UN { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal? Valor { get; set; }

    }
}