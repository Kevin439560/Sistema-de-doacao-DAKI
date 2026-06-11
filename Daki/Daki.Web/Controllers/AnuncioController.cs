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

        [AllowAnonymous] // Permite que visitantes não logados vejam os detalhes do item
        [HttpGet]
        public async Task<IActionResult> Detalhes(Guid id)
        {
            var anuncio = await _anuncioRepository.ObterPorIdAsync(id);
            if (anuncio == null) return NotFound();

            // 1. Pega o ID do usuário logado (se houver)
            var usuarioLogadoId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var isDono = usuarioLogadoId == anuncio.UsuarioId.ToString();

            // 2. Regra de Negócio: Só registra visualização se NÃO for o dono do anúncio
            if (!isDono)
            {
                Guid? visitanteId = string.IsNullOrEmpty(usuarioLogadoId) ? null : Guid.Parse(usuarioLogadoId);
                var ipVisitante = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "IP Desconhecido";

                //  Verifica se a pessoa já viu o anúncio antes
                bool jaVisualizou;

                if (visitanteId.HasValue)
                {
                    // Se está logado, procura pelo ID do usuário
                    jaVisualizou = anuncio.Visualizacoes.Any(v => v.UsuarioId == visitanteId.Value);
                }
                else
                {
                    // Se for anônimo, procura pelo IP
                    jaVisualizou = anuncio.Visualizacoes.Any(v => v.IpAddress == ipVisitante);
                }

                // Só salva se for uma visualização inédita daquela pessoa/IP
                if (!jaVisualizou)
                {
                    var novaVisualizacao = new VisualizacaoAnuncio(anuncio.Id, visitanteId, ipVisitante);

                    await _anuncioRepository.AdicionarVisualizacaoAsync(novaVisualizacao);
                }
            }

            return View(anuncio);
        }

        // GET: /Anuncio/MeusAnuncios
        [HttpGet]
        public async Task<IActionResult> MeusAnuncios()
        {
            // 1. Descobre quem é o usuário logado
            var usuarioIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(usuarioIdClaim)) return Unauthorized();

            var usuarioId = Guid.Parse(usuarioIdClaim);

            // 2. Busca apenas os anúncios deste usuário
            var meusAnuncios = await _anuncioRepository.ObterPorUsuarioAsync(usuarioId);

            // 3. Devolve a lista para a tela
            return View(meusAnuncios);
        }

        // GET: /Anuncio/Gerenciar/{id}
        [HttpGet]
        public async Task<IActionResult> Gerenciar(Guid id)
        {
            var usuarioIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(usuarioIdClaim)) return Unauthorized();

            var usuarioId = Guid.Parse(usuarioIdClaim);

            // Busca o anúncio completo
            var anuncio = await _anuncioRepository.ObterPorIdAsync(id);

            // Trava de Segurança: Só o dono do anúncio pode entrar nesta tela!
            if (anuncio == null || anuncio.UsuarioId != usuarioId)
            {
                TempData["Erro"] = "Você não tem permissão para gerenciar este anúncio.";
                return RedirectToAction("MeusAnuncios");
            }

            return View(anuncio);
        }

        // GET: /Anuncio/Editar/{id}
        [HttpGet]
        public async Task<IActionResult> Editar(Guid id)
        {
            var usuarioIdLogado = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var anuncio = await _anuncioRepository.ObterPorIdAsync(id);

            // Trava de segurança rigorosa
            if (anuncio == null || anuncio.UsuarioId != usuarioIdLogado)
            {
                TempData["Erro"] = "Você não tem permissão para editar este anúncio.";
                return RedirectToAction("MeusAnuncios");
            }

            return View(anuncio);
        }

        // GET: /Anuncio/ConcluirDoacao/{id}
        [HttpGet]
        public async Task<IActionResult> ConcluirDoacao(Guid id)
        {
            var usuarioIdLogado = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var anuncio = await _anuncioRepository.ObterPorIdAsync(id);

            // Trava de segurança: só o dono pode concluir
            if (anuncio == null || anuncio.UsuarioId != usuarioIdLogado) return Unauthorized();

            // Usa o método que já existia na sua classe!
            anuncio.MarcarDoado();
            await _anuncioRepository.AtualizarAsync(anuncio);

            TempData["MensagemSucesso"] = "Oba! Mais uma doação realizada!.";
            return RedirectToAction("MeusAnuncios");
        }

        // GET: /Anuncio/CancelarReserva/{id}
        [HttpGet]
        public async Task<IActionResult> CancelarReserva(Guid id)
        {
            var usuarioIdLogado = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var anuncio = await _anuncioRepository.ObterPorIdAsync(id);

            if (anuncio == null || anuncio.UsuarioId != usuarioIdLogado) return Unauthorized();

            // Volta o status para Ativo usando o método novo
            anuncio.Reativar();
            anuncio.Interesses.Where(i => i.Status == Dominio.Enums.StatusInteresse.Aceita)
                .ToList()
                .ForEach(i => i.Recuir());
            
            await _anuncioRepository.AtualizarAsync(anuncio);

            TempData["Erro"] = "Reserva desfeita. O anúncio voltou a ficar ativo na vitrine.";
            return RedirectToAction("Gerenciar", new { id = anuncio.Id });
        }

        // GET: /Anuncio/Fechar/{id}
        [HttpGet]
        public async Task<IActionResult> Fechar(Guid id)
        {
            var usuarioIdLogado = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var anuncio = await _anuncioRepository.ObterPorIdAsync(id);

            if (anuncio == null || anuncio.UsuarioId != usuarioIdLogado) return Unauthorized();

            anuncio.Fechar();
            await _anuncioRepository.AtualizarAsync(anuncio);

            TempData["MensagemSucesso"] = "Anúncio pausado! Ele não aparecerá mais na vitrine.";
            return RedirectToAction("Gerenciar", new { id = anuncio.Id });
        }

        // GET: /Anuncio/Reativar/{id}
        [HttpGet]
        public async Task<IActionResult> Reativar(Guid id)
        {
            var usuarioIdLogado = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var anuncio = await _anuncioRepository.ObterPorIdAsync(id);

            if (anuncio == null || anuncio.UsuarioId != usuarioIdLogado) return Unauthorized();

            anuncio.Reativar();
            await _anuncioRepository.AtualizarAsync(anuncio);

            TempData["MensagemSucesso"] = "Anúncio reativado! Ele voltou para a vitrine principal.";
            return RedirectToAction("Gerenciar", new { id = anuncio.Id });
        }

        // POST: /Anuncio/Editar
        [HttpPost]
        public async Task<IActionResult> Editar(Guid id, string titulo, string descricao, Daki.Dominio.Enums.Categoria categoria)
        {
            // 1. Pega quem está logado
            var usuarioIdLogado = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            // 2. Busca o anúncio no banco
            var anuncio = await _anuncioRepository.ObterPorIdAsync(id);

            // 3. Trava de Segurança dupla: Existe? O cara logado é o dono?
            if (anuncio == null) return NotFound();
            if (anuncio.UsuarioId != usuarioIdLogado)
            {
                TempData["Erro"] = "Você não tem permissão para editar este anúncio.";
                return RedirectToAction("MeusAnuncios");
            }

            // 4. Aplica os novos dados usando o método de domínio
            anuncio.AtualizarDados(titulo, descricao, categoria);

            // 5. Salva no banco de dados (o AtualizarAsync padrão do seu repositório vai funcionar perfeito aqui)
            await _anuncioRepository.AtualizarAsync(anuncio);

            // 6. Devolve para a tela de Gerenciamento com sucesso
            TempData["MensagemSucesso"] = "As informações do anúncio foram atualizadas!";
            return RedirectToAction("Gerenciar", new { id = anuncio.Id });
        }

        // POST: /Anuncio/Criar
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Criar(AnuncioCriarViewModel model)
        {
            if (model.Imagens == null || !model.Imagens.Any() || model.Imagens.All(i => i.Length == 0))
            {
                // Injeta o erro direto no campo "Imagens" da tela
                ModelState.AddModelError("Imagens", "É obrigatório anexar pelo menos uma foto do item.");
            }

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
                var novoAnuncio = new Anuncio(usuarioId, endereco, model.Titulo, model.Descricao, model.Categoria.Value);

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

        // POST: /Anuncio/ExcluirAdmin/{id}
        [HttpPost]
        [Authorize(Roles = "Admin")] // Só quem tem o claim de Admin entra.
        public async Task<IActionResult> ExcluirAdmin(Guid id)
        {
            var anuncio = await _anuncioRepository.ObterPorIdAsync(id);
            if (anuncio == null) return NotFound();

            anuncio.Encerrar();
            await _anuncioRepository.AtualizarAsync(anuncio);

            return RedirectToAction("Index", "Home");
        }
    }


}
