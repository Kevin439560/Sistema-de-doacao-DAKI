using Microsoft.AspNetCore.Http;
using Daki.Dominio.Enums;
using System.ComponentModel.DataAnnotations;

namespace Daki.Web.Models
{
    public class AnuncioCriarViewModel
    {
        // --- Dados do Anúncio ---
        [Required(ErrorMessage = "O título do anúncio é obrigatório.")]
        [MaxLength(100, ErrorMessage = "O título pode ter no máximo 100 caracteres.")]
        public string Titulo { get; set; } = string.Empty;

        [Required(ErrorMessage = "A descrição do item é obrigatória.")]
        public string Descricao { get; set; } = string.Empty;

        [Required(ErrorMessage = "Selecione uma categoria.")]
        public Categoria? Categoria { get; set; }

        // --- Dados do Endereço de Retirada ---
        [Required(ErrorMessage = "O CEP é obrigatório.")]
        public string CEP { get; set; } = string.Empty;

        [Required(ErrorMessage = "A rua é obrigatória.")]
        public string Rua { get; set; } = string.Empty;

        [Required(ErrorMessage = "O número é obrigatório.")]
        public string Numero { get; set; } = string.Empty;

        [Required(ErrorMessage = "O bairro é obrigatório.")]
        public string Bairro { get; set; } = string.Empty;

        [Required(ErrorMessage = "A cidade é obrigatória.")]
        public string Cidade { get; set; } = string.Empty;

        [Required(ErrorMessage = "O estado (UF) é obrigatório.")]
        [StringLength(2, MinimumLength = 2, ErrorMessage = "Use a sigla do estado (ex: DF, SP).")]
        public string UF { get; set; } = string.Empty;

        [Required(ErrorMessage = "É obrigatório anexar pelo menos uma foto do item.")]
        public List<IFormFile> Imagens { get; set; } = new List<IFormFile>();
    }
}
