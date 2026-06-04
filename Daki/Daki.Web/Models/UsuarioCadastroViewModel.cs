using System.ComponentModel.DataAnnotations;

namespace Daki.Web.Models
{
    public class UsuarioCadastroViewModel
    {
        [Required(ErrorMessage = "O nome é obrigatório.")]
        [MinLength(3, ErrorMessage = "O nome deve ter no mínimo 3 caracteres.")]
        public string Nome { get; set; } = string.Empty;

        [Required(ErrorMessage = "O e-mail é obrigatório.")]
        [EmailAddress(ErrorMessage = "Digite um formato de e-mail válido.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "A senha é obrigatória.")]
        [MinLength(6, ErrorMessage = "A senha deve ter pelo menos 6 caracteres.")]
        public string Senha { get; set; } = string.Empty;

        // O telefone não é obrigatório, mas se for preenchido, validamos o formato
        [Phone(ErrorMessage = "Digite um telefone válido.")]
        public string? Fone { get; set; }
    }
}
