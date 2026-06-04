using Daki.Dominio.Entidades;
using Daki.Dominio.Interfaces;
using Daki.Infra.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Daki.Infra.Repositorios
{
    public class InteresseRepository : IInteresseRepository
    {
        private readonly DakiContext _context;

        public InteresseRepository(DakiContext context)
        {
            _context = context;
        }

        public async Task<Interesse?> ObterPorIdAsync(Guid id)
        {
            return await _context.Interesses
                .Include(i => i.Anuncio)
                .FirstOrDefaultAsync(i => i.Id == id);
        }

        public async Task<IEnumerable<Interesse>> ObterPorAnuncioAsync(Guid anuncioId)
        {
            return await _context.Interesses
                .Include(i => i.Usuario) // Para o doador saber quem está interessado
                .Where(i => i.AnuncioId == anuncioId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Interesse>> ObterPorUsuarioAsync(Guid usuarioId)
        {
            return await _context.Interesses
                .Include(i => i.Anuncio) // Para o usuário ver quais anúncios ele demonstrou interesse
                .Where(i => i.UsuarioId == usuarioId)
                .ToListAsync();
        }

        public async Task AdicionarAsync(Interesse interesse)
        {
            await _context.Interesses.AddAsync(interesse);
            await _context.SaveChangesAsync();
        }

        public async Task AtualizarAsync(Interesse interesse)
        {
            _context.Interesses.Update(interesse);
            await _context.SaveChangesAsync();
        }
    }
}
