using Daki.Dominio.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Daki.Dominio.Entidades
{
    public class Anuncio
    {
        public Guid Id { get; private set; }
        public Guid UsuarioId { get; private set; }
        public Guid EnderecoId { get; private set; }
        public string Titulo { get; private set; } = string.Empty;
        public string Descricao { get; private set; } = string.Empty;
        public Categoria Categoria { get; private set; }
        public Status Status { get; private set; }

        public Usuario Usuario { get; private set; }
        public Endereco Endereco { get; private set; }
        public ICollection<ImagemAnuncio> Imagens { get; private set; } = new List<ImagemAnuncio>();
        public ICollection<Interesse> Interesses { get; private set; } = new List<Interesse>();

        protected Anuncio() { }

        public Anuncio(Guid usuarioId, Guid enderecoId, string titulo, string descricao, Categoria categoria)
        {
            Id = Guid.NewGuid();
            UsuarioId = usuarioId;
            EnderecoId = enderecoId;
            Titulo = titulo;
            Descricao = descricao;
            Categoria = categoria;
            Status = Status.Ativo;
        }

        public void Reservar()
        {
            Status = Status.Reservado;
        }

        public void MarcarDoado()
        {
            Status = Status.Concluido;
        }
        public void AddImagem(string urlImagem, bool principal)
        {
            Imagens.Add(new ImagemAnuncio(Id, urlImagem, principal));
        }

        public void Fechar()
        {
            Status = Status.Fechado;
        }
    }
}
