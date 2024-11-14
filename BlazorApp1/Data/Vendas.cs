using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace BlazorApp1.Data
{
    public partial class Venda
    {
        public int Id { get; set; }
       
        public int Id_Empresa { get; set; }
        [ForeignKey("Id_Empresa")]
        public int Id_Cliente { get; set; }
        [ForeignKey("Id_Cliente")]

        //[JsonIgnore]
        public virtual Empresas? Empresas { get; set; }

        [ForeignKey(nameof(Id_Cliente))]
        public virtual Cliente? Cliente { get; set; }

        [DataType(DataType.Date)]
        public DateTime? Data_Emissao { get; set; }

        public int? Nr_Nota { get; set; }
        public int? Serie { get; set; }
        public string? CPF_CNPJ { get; set; }
        public string? Chave { get; set; }
        public virtual byte[]? XML_{ get; set; }
        public decimal? Total { get; set; }
        public virtual List<Item>? Itens { get; set; }
        public Venda()
        {
            Itens = new List<Item>();
        }

    }
}