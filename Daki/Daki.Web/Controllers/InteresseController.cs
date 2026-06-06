using Daki.Dominio.Entidades;
using Daki.Dominio.Interfaces;
using Daki.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Daki.Web.Controllers
{
    [Authorize] // Apenas logados podem pedir
    public class InteresseController : Controller
    {
        private readonly IInteresseRepository _interesseRepository;
        private readonly IAnuncioRepository _anuncioRepository;

        public InteresseController(IInteresseRepository interesseRepository, IAnuncioRepository anuncioRepository)
        {
            _interesseRepository = interesseRepository;
            _anuncioRepository = anuncioRepository;
        }

        // GET: /Interesse/MeusInteresses
        [HttpGet]
        public async Task<IActionResult> MeusInteresses()
        {
            var usuarioIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(usuarioIdClaim)) return Unauthorized();

            var usuarioId = Guid.Parse(usuarioIdClaim);

            // Busca os interesses deste usuário no banco
            var meusInteresses = await _interesseRepository.ObterPorUsuarioAsync(usuarioId);

            return View(meusInteresses);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Solicitar(InteresseCriarViewModel model)
        {
            // 1. Pega o ID do usuário logado que está pedindo a doação
            var usuarioInteressadoIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(usuarioInteressadoIdClaim)) return Unauthorized();

            var usuarioInteressadoId = Guid.Parse(usuarioInteressadoIdClaim);

            // 2. Busca o anúncio para verificar segurança
            var anuncio = await _anuncioRepository.ObterPorIdAsync(model.AnuncioId);
            if (anuncio == null) return NotFound();

            // 3. O usuario so pode enviar um pedido de interesse para um anúncio uma única vez. Se ele já enviou, mostra mensagem de erro.
            if (anuncio.Interesses != null && anuncio.Interesses.Any(i => i.UsuarioId == usuarioInteressadoId))
            {
                TempData["Erro"] = "Você já enviou um pedido para esta doação. Aguarde a resposta do doador.";
                return RedirectToAction("Detalhes", "Anuncio", new { id = model.AnuncioId });
            }

            // 4. Trava de segurança: O dono não pode pedir o próprio item (se ele burlar o HTML)
            if (anuncio.UsuarioId == usuarioInteressadoId)
            {
                TempData["Erro"] = "Você não pode solicitar o próprio anúncio.";
                return RedirectToAction("Detalhes", "Anuncio", new { id = model.AnuncioId });
            }

            // 5. Cria a entidade de Domínio
            var novoInteresse = new Interesse(usuarioInteressadoId, model.AnuncioId, model.Mensagem);

            // 6. Salva no banco de dados
            await _interesseRepository.AdicionarAsync(novoInteresse);

            // 7. Redireciona de volta para a tela de detalhes com a mensagem de sucesso
            TempData["MensagemSucesso"] = "Solicitação enviada com sucesso! O doador avaliará a sua justificativa.";
            return RedirectToAction("Detalhes", "Anuncio", new { id = model.AnuncioId });
        }

        // GET: /Interesse/Aceitar/{id}
        [HttpGet]
        public async Task<IActionResult> Aceitar(Guid id)
        {
            var usuarioLogadoId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            // 1. Busca o interesse no banco (precisamos dar um Include no Anúncio para verificar o dono)
            var interesse = await _interesseRepository.ObterPorIdAsync(id);
            if (interesse == null) return NotFound();

            var anuncio = await _anuncioRepository.ObterPorIdAsync(interesse.AnuncioId);

            // 2. Trava de segurança: só o dono do anúncio pode aceitar o pedido
            if (anuncio == null || anuncio.UsuarioId != usuarioLogadoId) return Unauthorized();

            // 3. Regra de Negócio: O anúncio não pode ter sido doado para outra pessoa
            if (anuncio.Status != Daki.Dominio.Enums.Status.Ativo)
            {
                TempData["Erro"] = "Este anúncio não está mais ativo para doação.";
                return RedirectToAction("Gerenciar", "Anuncio", new { id = anuncio.Id });
            }

            // 4. Aceita o interesse
            interesse.Aceitar();
            await _interesseRepository.AtualizarAsync(interesse);

            // 5. Reserva o anúncio (usando o método que você já tem na classe Anuncio!)
            anuncio.Reservar();
            await _anuncioRepository.AtualizarAsync(anuncio);

            TempData["MensagemSucesso"] = "Pedido aceito! O anúncio foi reservado e agora vocês podem combinar a entrega.";
            return RedirectToAction("Gerenciar", "Anuncio", new { id = anuncio.Id });
        }


        // GET: /Interesse/Recusar/{id}
        [HttpGet]
        public async Task<IActionResult> Recusar(Guid id)
        {
            var usuarioLogadoId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var interesse = await _interesseRepository.ObterPorIdAsync(id);
            if (interesse == null) return NotFound();

            var anuncio = await _anuncioRepository.ObterPorIdAsync(interesse.AnuncioId);
            if (anuncio == null || anuncio.UsuarioId != usuarioLogadoId) return Unauthorized();

            // Altera o status do interesse para Recusado
            interesse.Recusar();
            await _interesseRepository.AtualizarAsync(interesse);

            TempData["MensagemSucesso"] = "O pedido foi recusado.";
            return RedirectToAction("Gerenciar", "Anuncio", new { id = anuncio.Id });
        }
    }   
}
