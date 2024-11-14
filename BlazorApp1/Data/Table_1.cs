using System.ComponentModel.DataAnnotations;

namespace BlazorApp1.Data
{
    public partial class Tables_1
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "O nome do aluno é obrigatório")]
        public string? Descricao { get; set; }
    }
}