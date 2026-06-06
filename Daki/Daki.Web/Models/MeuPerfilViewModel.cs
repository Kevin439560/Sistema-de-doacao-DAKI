using System.ComponentModel.DataAnnotations;

namespace Daki.Web.Models
{
    public class MeuPerfilViewModel
    {
        [Required(ErrorMessage = "O Nome é obrigatório.")]
        public string? Nome { get; set; }

        [Required(ErrorMessage = "O Telefone é obrigatório.")]
        public string? Telefone { get; set; }

        public string? SenhaAtual { get; set; }

        [MinLength(6, ErrorMessage = "A nova senha deve ter pelo menos 6 caracteres.")]
        public string? NovaSenha { get; set; }
    }
}