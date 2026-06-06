using Daki.Dominio.Entidades;
using Daki.Dominio.Interfaces;
using Daki.Infra.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Daki.Infra.Repositorios
{
    public class UsuarioRepository : IUsuarioRepository
    {
        private readonly DakiContext _context;

        public UsuarioRepository(DakiContext context)
        {
            _context = context;
        }

        public async Task<Usuario?> ObterPorIdAsync(Guid id)
        {
            return await _context.Usuarios
                .Include(u => u.Enderecos)
                .FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<Usuario?> ObterPorEmailAsync(string email)
        {
            return await _context.Usuarios
                .FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task AdicionarAsync(Usuario usuario)
        {
            await _context.Usuarios.AddAsync(usuario);
            await _context.SaveChangesAsync();
        }

        public async Task AtualizarAsync(Usuario usuario)
        {
            _context.Usuarios.Update(usuario);
            await _context.SaveChangesAsync();
        }
    }
}
