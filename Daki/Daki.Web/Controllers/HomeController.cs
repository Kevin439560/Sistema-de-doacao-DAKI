using Daki.Dominio.Interfaces;
using Daki.Web.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Daki.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly IAnuncioRepository _anuncioRepository;

        public HomeController(IAnuncioRepository anuncioRepository)
        {
            _anuncioRepository = anuncioRepository;
        }

        public async Task<IActionResult> Index()
        {
            // Busca todos os anúncios com status "Ativo" vindos do banco
            var anunciosAtivos = await _anuncioRepository.ObterTodosAtivosAsync();

            // Passa a lista de anúncios diretamente para a View
            return View(anunciosAtivos);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View();
        }
    }
}
