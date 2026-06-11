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
        public DateTime DataCriacao { get; private set; }

        // Coleções (Navegação)
        public Usuario Usuario { get; private set; }
        public Endereco Endereco { get; private set; }
        public ICollection<ImagemAnuncio> Imagens { get; private set; } = new List<ImagemAnuncio>();
        public ICollection<Interesse> Interesses { get; private set; } = new List<Interesse>();
        public ICollection<VisualizacaoAnuncio> Visualizacoes { get; private set; } = new List<VisualizacaoAnuncio>();

        // Construtor vazio exigido pelo Entity Framework
        protected Anuncio() { }

        public Anuncio(Guid usuarioId, Endereco endereco, string titulo, string descricao, Categoria categoria)
        {
            Id = Guid.NewGuid();
            UsuarioId = usuarioId;
            EnderecoId = endereco.Id;
            Endereco = endereco;
            Titulo = titulo;
            Descricao = descricao;
            Categoria = categoria;
            Status = Status.Ativo; // Todo anúncio nasce ativo
            DataCriacao = DateTime.UtcNow;
        }

        // Métodos de Regra de Negócio
        public void Reservar() => Status = Status.Reservado;
        public void MarcarDoado() => Status = Status.Concluido;
        public void Fechar() => Status = Status.Fechado;
        public void Reativar() => Status = Status.Ativo;
        public void Encerrar() => Status = Status.Encerrado;

        public void AddImagem(string urlImagem, bool principal)
        {
            Imagens.Add(new ImagemAnuncio(Id, urlImagem, principal));
        }

        public void AtualizarDados(string titulo, string descricao, Daki.Dominio.Enums.Categoria categoria)
        {
            Titulo = titulo;
            Descricao = descricao;
            Categoria = categoria;
        }


    }

}
