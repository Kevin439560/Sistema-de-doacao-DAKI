using Daki.Dominio.Entidades;
using System;
using System.Collections.Generic;
using System.Text;

namespace Daki.Dominio.Interfaces
{
    public interface IInteresseRepository
    {
        Task<Interesse?> ObterPorIdAsync(Guid id);
        Task<IEnumerable<Interesse>> ObterPorAnuncioAsync(Guid anuncioId);
        Task<IEnumerable<Interesse>> ObterPorUsuarioAsync(Guid usuarioId);
        Task AdicionarAsync(Interesse interesse);
        Task AtualizarAsync(Interesse interesse);
    }
}
