using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace BlazorApp1.Data
{
    public partial class Item
    {
        public int Id { get; set; }

        public int Id_Empresa { get; set; }
        [ForeignKey("Id_Empresa")]
        public int Id_Produto { get; set; }
        [ForeignKey("Id_Produto")]
        public virtual Produto? Produto { get; set; }
        public int Id_Venda { get; set; }
        [ForeignKey("Id_Venda")]

        [JsonIgnore]
        public virtual Venda? Venda { get; set; }
        public decimal? Quantidade { get; set; }
        public decimal? Valor { get; set; }
        public decimal? Total { get; set; }

        [NotMapped]
        public decimal? TotalItem
        {
            get
            {
                return Quantidade * Valor;
            }
        }

    }
}