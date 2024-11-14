using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BlazorApp1.Data
{
    public partial class Cliente
    {
        public int Id { get; set; }

        [ForeignKey("Empresa")]
        public int Id_Empresa { get; set; }
       
        public string? Nome { get; set; }

        [DataType(DataType.Date)]
        public DateTime? Data_Nascimento { get; set; }

        [Required(ErrorMessage = "O CPF/CNPJ do cliente é obrigatório")]
        [StringLength(14)]
        public string? CPF_CNPJ { get; set; }
        public string? RG { get; set; }
        public string? Telefone { get; set; }
        public string? CEP { get; set; }
        public string? Endereco { get; set; }
        public int? Numero { get; set; }
        public string? Complemento { get; set; }
        public string? Bairro { get; set; }
        public string? Cidade { get; set; }
        public string? Estado { get; set; }
    }
}