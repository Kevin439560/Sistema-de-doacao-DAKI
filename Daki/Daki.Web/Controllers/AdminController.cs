using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
// Ajuste o namespace abaixo para o do seu projeto (onde está o seu DbContext)
using Daki.Infra.Data;

namespace Daki.Web.Controllers
{
    // A mágica da segurança acontece aqui. Só quem tem o crachá de Admin entra!
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly DakiContext _context; // Troque pelo nome do seu DbContext se for diferente

        public AdminController(DakiContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Dashboard()
        {
            // Pega a data de 7 dias atrás
            var dataLimite = DateTime.UtcNow.Date.AddDays(-7);

            // PASSO 1: Falar com o Banco de Dados (PostgreSQL)
            // Agrupa as datas puras e conta, sem formatar texto ainda
            var dadosBrutos = await _context.Anuncios
                .Where(a => a.DataCriacao >= dataLimite)
                .GroupBy(a => a.DataCriacao.Date)
                .Select(g => new
                {
                    DataOriginal = g.Key,
                    Quantidade = g.Count()
                })
                .OrderBy(x => x.DataOriginal)
                .ToListAsync(); // <-- O SQL é executado AQUI e os dados vão para a RAM

            // PASSO 2: Falar com o C# (Memória)
            // Agora que temos os dados, formatamos a data para o Chart.js
            var anunciosPorDia = dadosBrutos
                .Select(x => new
                {
                    Data = x.DataOriginal.ToString("dd/MM"),
                    Quantidade = x.Quantidade
                })
                .ToList();

            // Separa os dados em duas listas para o gráfico de Javascript entender
            var labels = anunciosPorDia.Select(x => x.Data).ToArray();
            var valores = anunciosPorDia.Select(x => x.Quantidade).ToArray();

            // Transforma o C# em texto JSON para mandar para a tela
            ViewBag.Labels = JsonSerializer.Serialize(labels);
            ViewBag.Valores = JsonSerializer.Serialize(valores);

            return View();
        }   
    }
}