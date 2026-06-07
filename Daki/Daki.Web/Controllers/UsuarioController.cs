using Daki.Dominio.Entidades;
using Daki.Dominio.Interfaces;
using Daki.Web.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

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

        // GET: /Usuario/MeuPerfil
        [HttpGet]
        public async Task<IActionResult> MeuPerfil()
        {
            // Pega o ID do usuário que está logado
            var usuarioIdLogado = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            // Busca os dados dele no banco
            var usuario = await _usuarioRepository.ObterPorIdAsync(usuarioIdLogado);

            if (usuario == null) return NotFound();

            //  Mapear os dados do banco para o ViewModel da tela
            var viewModel = new MeuPerfilViewModel
            {
                Nome = usuario.Nome,
                Telefone = usuario.Fone, // (Nota: Se a sua propriedade na Entidade se chamar Fone, ajuste aqui)
                Email = usuario.Email
            };

            // Manda o ViewModel para a tela preencher o formulário
            return View(viewModel);
        }

        // POST: /Usuario/MeuPerfil
        [HttpPost]
        public async Task<IActionResult> MeuPerfil(MeuPerfilViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model); // Se o formulário for inválido, devolve a tela com os erros
            }

            var usuarioIdLogado = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var usuario = await _usuarioRepository.ObterPorIdAsync(usuarioIdLogado);

            if (usuario == null) return NotFound();

            // 1. Atualiza os dados de contato usando o método do domínio
            usuario.AtualizarDados(model.Nome, model.Telefone);

            // 2. Se ele digitou algo no campo de senha, nós atualizamos
            if (!string.IsNullOrWhiteSpace(model.NovaSenha))
            {
                if (!BCrypt.Net.BCrypt.Verify(model.SenhaAtual, usuario.PasswordHash))
                {
      
                    ModelState.AddModelError("SenhaAtual", "A senha atual está incorreta.");

                    return View(model);
                }
                usuario.AlterarSenha(BCrypt.Net.BCrypt.HashPassword(model.NovaSenha));
            }

            // 3. Salva tudo no banco de dados
            await _usuarioRepository.AtualizarAsync(usuario);

            //  4. Atualiza o Cookie (Crachá) com o novo nome
            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
                new(ClaimTypes.Name, usuario.Nome), // Aqui entra o nome atualizado!
                new(ClaimTypes.Email, usuario.Email)
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            // Re-efetua o login "por baixo dos panos" para trocar o cookie antigo pelo novo
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            TempData["MensagemSucesso"] = "Os seus dados foram atualizados com sucesso!";
            return RedirectToAction("MeuPerfil");
        }
    }
}
