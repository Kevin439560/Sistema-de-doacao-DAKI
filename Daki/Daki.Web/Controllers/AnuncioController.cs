using Daki.Dominio.Entidades;
using Daki.Dominio.Interfaces;
using Daki.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Daki.Web.Controllers
{
    //so permite acessar as ações desse controller se o usuário estiver autenticado (logado)
    [Authorize]
    public class AnuncioController : Controller
    {
        private readonly IAnuncioRepository _anuncioRepository;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public AnuncioController(IAnuncioRepository anuncioRepository, IWebHostEnvironment webHostEnvironment)
        {
            _anuncioRepository = anuncioRepository;
            _webHostEnvironment = webHostEnvironment;
        }

        [HttpGet]
        public IActionResult Criar()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Criar(AnuncioCriarViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                // 1. le o ID do usuário diretamente do Cookie de autenticação
                var usuarioIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(usuarioIdClaim)) return Unauthorized();

                var usuarioId = Guid.Parse(usuarioIdClaim);

                // 2. Instanciamos o Endereço de retirada
                var endereco = new Endereco(usuarioId, model.CEP, model.Rua, model.Numero, model.Bairro, model.Cidade, model.UF.ToUpper());

                // 3. Instanciamos o Anúncio, passando o Endereço junto
                var novoAnuncio = new Anuncio(usuarioId, endereco.Id, model.Titulo, model.Descricao, model.Categoria.Value);

                if (model.Imagens != null && model.Imagens.Count > 0)
                {
                    // 5. Array de segurança com os formatos permitidos
                    var extensoesPermitidas = new[] { ".jpg", ".jpeg", ".png" };

                    string pastaUploads = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", "anuncios");
                    if (!Directory.Exists(pastaUploads))
                    {
                        Directory.CreateDirectory(pastaUploads);
                    }

                    // 6. Percorre todos os arquivos enviados
                    for (int i = 0; i < model.Imagens.Count; i++)
                    {
                        var arquivo = model.Imagens[i];
                        if (arquivo.Length == 0) continue;

                        // 7. Verifica a extensão real do arquivo
                        var extensao = Path.GetExtension(arquivo.FileName).ToLower();
                        if (!extensoesPermitidas.Contains(extensao))
                        {
                            ModelState.AddModelError("Imagens", $"O arquivo '{arquivo.FileName}' tem um formato inválido. Use apenas JPG ou PNG.");
                            return View(model);
                        }

                        string nomeArquivoUnico = Guid.NewGuid().ToString() + extensao;
                        string caminhoFisico = Path.Combine(pastaUploads, nomeArquivoUnico);

                        using (var fileStream = new FileStream(caminhoFisico, FileMode.Create))
                        {
                            await arquivo.CopyToAsync(fileStream);
                        }

                        string urlImagem = "/uploads/anuncios/" + nomeArquivoUnico;

                        // 8. A primeira imagem vai ser a principal (capa) do anúncio
                        bool isPrincipal = (i == 0);

                        novoAnuncio.AddImagem(urlImagem, isPrincipal);
                    }
                }

                // 9. Salva no banco.
                await _anuncioRepository.AdicionarAsync(novoAnuncio);

                TempData["MensagemSucesso"] = "Sua doação foi publicada com sucesso e já está disponível na vitrine!";
                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "Ocorreu um erro ao salvar o anúncio. Tente novamente.");
                return View(model);
            }
        }
    }
}
