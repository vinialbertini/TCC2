using System.ComponentModel.DataAnnotations;

namespace BlazorApp1.Data
{
    public partial class Empresas
    {
        public int Id { get; set; }

        //[Required(ErrorMessage = "O nome do aluno é obrigatório")]
        public string? Fantasia { get; set; }
        public string? Razao_Social { get; set; }
        public string? Endereco { get; set; }
        public int? Numero { get; set; }
        public string? Bairro { get; set; }
        public string? Cidade { get; set; }
        public string? UF { get; set; }
        public string? CEP { get; set; }
        public virtual byte[]? Certificado { get; set; }
        public string? Senha_Certificado { get; set; }
        public string? Email { get; set; }
        public string? Senha_Acesso { get; set; }
        public string? CNPJ { get; set; }

        [DataType(DataType.Date)]
        public DateTime? Data_Cadastro { get; set; }
        public string? Ambiente { get; set; }
        public string? Complemento { get; set; }

    }
}