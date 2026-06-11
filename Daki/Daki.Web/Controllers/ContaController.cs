using Daki.Dominio.Interfaces;
using Daki.Web.Models;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace Daki.Web.Controllers
{
    public class ContaController : Controller
    {
        private readonly IUsuarioRepository _usuarioRepository;

        public ContaController(IUsuarioRepository usuarioRepository)
        {
            _usuarioRepository = usuarioRepository;
        }

        [HttpGet]
        public IActionResult Login()
        {
            // Se o usuário já estiver logado ele vai para a Home.
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(UsuarioLoginViewModel model)
        {
            // 1. valida se os campos estão vazios ou fora do formato
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // 2. Busca o usuário pelo e-mail
            var usuario = await _usuarioRepository.ObterPorEmailAsync(model.Email);

            // 3. Verifica se o usuário existe e se a senha está correta
            if (usuario == null || !BCrypt.Net.BCrypt.Verify(model.Senha, usuario.PasswordHash))
            {
                // Adiciona um erro genérico se as credenciais estiverem erradas
                ModelState.AddModelError(string.Empty, "E-mail ou senha inválidos.");
                return View(model);
            }

            // 4. Cria as claims para o usuário autenticado
            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
                new(ClaimTypes.Name, usuario.Nome),
                new(ClaimTypes.Email, usuario.Email)
            };


            if (usuario.IsAdmin)
            {
                claims.Add(new Claim(ClaimTypes.Role, "Admin"));
            }

            // Criar a identidade carimbada com essas informações
            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            // Solicita o navegador do usuário salvar esse crachá (Cookie) de forma segura
            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity));

            // Se deu tudo certo, redireciona para a página inicial
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            // Remove o crachá do usuário e encerra a sessão
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");

        }
    }
}
