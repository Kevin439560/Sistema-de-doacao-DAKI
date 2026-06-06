using System.ComponentModel.DataAnnotations;

namespace Daki.Web.Models
{
    public class InteresseCriarViewModel
    {
        [Required]
        public Guid AnuncioId { get; set; }

        [Required(ErrorMessage = "A justificativa é obrigatória.")]
        [MinLength(10, ErrorMessage = "Escreva uma mensagem um pouco mais explicativa.")]
        public string Mensagem { get; set; } = string.Empty;
    }
}
