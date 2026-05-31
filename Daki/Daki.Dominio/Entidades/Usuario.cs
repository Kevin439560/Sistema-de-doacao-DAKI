using System;
using System.Collections.Generic;
using System.Text;

namespace Daki.Dominio.Entidades
{
    public class Usuario
    {
        public Guid Id { get; private set; }
        public string Nome { get; private set; } = string.Empty;
        public string Email { get; private set; } = string.Empty;
        public string PasswordHash { get; private set; } = string.Empty;
        public string Fone { get; private set; } = string.Empty;
        public DateTime DataCriacao { get; private set; }

        public Endereco? Endereco { get; private set; }
        public ICollection<Anuncio> Anuncios { get; private set; } = new List<Anuncio>();
        public ICollection<Interesse> Interesses { get; private set; } = new List<Interesse>();

        protected Usuario() { }

        public Usuario(string nome, string email, string passwordHash, string fone)
        {
            Id = Guid.NewGuid();
            Nome = nome;
            Email = email;
            PasswordHash = passwordHash;
            Fone = fone;
            DataCriacao = DateTime.UtcNow;
        }

        public void DefinirEndereco(Endereco endereco)
        {
            Endereco = endereco;
        }
    }
}
