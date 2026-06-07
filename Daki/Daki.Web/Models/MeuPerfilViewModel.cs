using System.ComponentModel.DataAnnotations;

namespace Daki.Web.Models
{
    public class MeuPerfilViewModel
    {
        [Required(ErrorMessage = "O Nome é obrigatório.")]
        public string? Nome { get; set; }
        public string? Email { get; set; }

        [Required(ErrorMessage = "O Telefone é obrigatório.")]
        [RegularExpression(@"^\(?([1-9]{2})\)?[-. ]?([2-9])?[-. ]?([0-9]{4})[-. ]?([0-9]{4})$",
            ErrorMessage = "Digite um telefone válido com DDD (ex: 11999998888 ou 1133334444).")]
        public string? Telefone { get; set; }

        public string? SenhaAtual { get; set; }

        [MinLength(6, ErrorMessage = "A nova senha deve ter pelo menos 6 caracteres.")]
        public string? NovaSenha { get; set; }
    }
}