using System;
using System.Collections.Generic;
using System.Text;

namespace Daki.Dominio.Entidades
{
    public class ImagemAnuncio
    {
        public Guid Id { get; private set; }
        public Guid AnuncioId { get; private set; }
        public string UrlImagem { get; private set; } = string.Empty;
        public bool Principal { get; private set; }

        public Anuncio? Anuncio { get; private set; }

        protected ImagemAnuncio() { }

        public ImagemAnuncio(Guid anuncioId, string urlImagem, bool principal)
        {
            Id = Guid.NewGuid();
            AnuncioId = anuncioId;
            UrlImagem = urlImagem;
            Principal = principal;
        }
    }
}
