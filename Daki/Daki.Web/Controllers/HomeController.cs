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

        // GET: /Home/Index
        public async Task<IActionResult> Index(Daki.Dominio.Enums.Categoria? categoria)
        {
            var anuncios = await _anuncioRepository.ObterVitrineAsync(categoria);

            ViewBag.CategoriaAtual = categoria;

            return View(anuncios);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View();
        }
    }
}
