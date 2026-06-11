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
        public bool IsAdmin { get; private set; } = false;

        // O 'set' agora é private, protegendo a coleção
        public ICollection<Endereco> Enderecos { get; private set; } = new List<Endereco>();
        public ICollection<Anuncio> Anuncios { get; private set; } = new List<Anuncio>();
        public ICollection<Interesse> Interesses { get; private set; } = new List<Interesse>();

        // Construtor vazio exigido pelo Entity Framework
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

        // Renomeamos o método e adicionamos o objeto à coleção
        public void AdicionarEndereco(Endereco endereco)
        {
            Enderecos.Add(endereco);
        }

        public void AtualizarDados(string nome, string telefone)
        {
            // Poderíamos colocar validações aqui no futuro (ex: verificar tamanho da string)
            Nome = nome;
            Fone = telefone;
        }

        public void AlterarSenha(string novaSenha)
        {
            PasswordHash = novaSenha;
        
        }
    }
}
