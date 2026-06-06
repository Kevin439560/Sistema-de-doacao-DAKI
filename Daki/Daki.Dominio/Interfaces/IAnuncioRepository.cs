using Daki.Dominio.Entidades;
using Daki.Dominio.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Daki.Dominio.Interfaces
{
    public interface IAnuncioRepository
    {
        Task<Anuncio?> ObterPorIdAsync(Guid id);
        Task<IEnumerable<Anuncio>> ObterVitrineAsync(Daki.Dominio.Enums.Categoria? categoria = null);
        Task<IEnumerable<Anuncio>> ObterPorCategoriaAsync(Categoria categoria);
        Task<IEnumerable<Anuncio>> ObterPorUsuarioAsync(Guid usuarioId);
        Task AdicionarAsync(Anuncio anuncio);
        Task AtualizarAsync(Anuncio anuncio);
        Task AdicionarVisualizacaoAsync(VisualizacaoAnuncio visualizacao);
    }
}