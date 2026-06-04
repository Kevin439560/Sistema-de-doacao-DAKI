using Daki.Dominio.Entidades;
using Daki.Dominio.Interfaces;
using Daki.Web.Models;
using Microsoft.AspNetCore.Mvc;

namespace Daki.Web.Controllers
{
    public class UsuarioController : Controller
    {
        private readonly IUsuarioRepository _usuarioRepository;

        //Injeção de dependência do repositório de usuários
        public UsuarioController(IUsuarioRepository usuarioRepository)
        {
            _usuarioRepository = usuarioRepository;
        }

        [HttpGet]
        public IActionResult Cadastrar()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cadastrar(UsuarioCadastroViewModel model)
        {
            try
            {
                // 1. Validação simples de backend
                if (!ModelState.IsValid)
                {
                    return View(model);
                }

                // 2. Não permitir e-mails duplicados
                var usuarioExistente = await _usuarioRepository.ObterPorEmailAsync(model.Email);
                if (usuarioExistente != null)
                {
                    // Adiciona um erro manual ao ModelState para aparecer na tela
                    ModelState.AddModelError("Email", "Este e-mail já está cadastrado em nossa plataforma.");
                    return View(model);
                }

                // 3. Gerando o Hash da senha com BCrypt (Work Factor padrão 11)
                string senhaCriptografada = BCrypt.Net.BCrypt.HashPassword(model.Senha);

                // 4. Criação da Entidade usando o Hash no lugar da senha limpa
                var novoUsuario = new Usuario(model.Nome, model.Email, senhaCriptografada, model.Fone ?? "");

                // 5. Salva no banco de dados
                await _usuarioRepository.AdicionarAsync(novoUsuario);

                TempData["MensagemSucesso"] = "Conta criada com sucesso! Faça seu login.";
                return RedirectToAction("Login", "Conta");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "Ocorreu um erro interno ao processar o seu cadastro. Tente novamente.");
                return View(model);
            }
        }
    }
}
