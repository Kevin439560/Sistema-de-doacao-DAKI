using Daki.Dominio.Entidades;
using Daki.Dominio.Enums;
using Daki.Dominio.Interfaces;
using Daki.Infra.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Daki.Infra.Repositorios
{
    public class AnuncioRepository : IAnuncioRepository
    {
        private readonly DakiContext _context;

        public AnuncioRepository(DakiContext context)
        {
            _context = context;
        }

        public async Task<Anuncio?> ObterPorIdAsync(Guid id)
        {
            return await _context.Anuncios
                .Include(a => a.Imagens)
                .Include(a => a.Endereco)
                .Include(a => a.Usuario)
                .Include(a => a.Interesses)
                    .ThenInclude(i => i.Usuario)
                .Include(a => a.Visualizacoes)
                .FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task<IEnumerable<Anuncio>> ObterVitrineAsync(Categoria? categoria = null)
        {
            var query = _context.Anuncios
                .Include(a => a.Imagens)
                .Include(a => a.Endereco)
                .Where(a => a.Status == Status.Ativo)
                .AsQueryable();

            if (categoria.HasValue)
            {
                query = query.Where(a => a.Categoria == categoria.Value);
            }

            return await query.OrderByDescending(a => a.DataCriacao).ToListAsync();
        }

        public async Task<IEnumerable<Anuncio>> ObterPorCategoriaAsync(Categoria categoria)
        {
            return await _context.Anuncios
                .Include(a => a.Imagens)
                .Include(a => a.Endereco)
                .Where(a => a.Status == Status.Ativo && a.Categoria == categoria)
                .ToListAsync();
        }

        public async Task<IEnumerable<Anuncio>> ObterPorUsuarioAsync(Guid usuarioId)
        {
            return await _context.Anuncios
                .Include(a => a.Imagens)
                .Include(a => a.Interesses)
                .Include(a => a.Visualizacoes)
                .Where(a => a.UsuarioId == usuarioId)
                .ToListAsync();
        }

        public async Task AdicionarAsync(Anuncio anuncio)
        {
            await _context.Anuncios.AddAsync(anuncio);
            await _context.SaveChangesAsync();
        }

        public async Task AtualizarAsync(Anuncio anuncio)
        {
            _context.Anuncios.Update(anuncio);
            await _context.SaveChangesAsync();
        }

        public async Task AdicionarVisualizacaoAsync(VisualizacaoAnuncio visualizacao)
        {
            _context.Set<VisualizacaoAnuncio>().Add(visualizacao);
            await _context.SaveChangesAsync();
        }

    }
}
