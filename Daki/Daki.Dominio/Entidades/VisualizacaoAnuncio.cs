using System;
using System.Collections.Generic;
using System.Text;

namespace Daki.Dominio.Entidades
{
    public class VisualizacaoAnuncio
    {
        public Guid Id { get; private set; }
        public Guid AnuncioId { get; private set; }
        public Guid? UsuarioId { get; private set; } // Nullable para visitantes anônimos
        public string IpAddress { get; private set; } = string.Empty;
        public DateTime DataVisualizacao { get; private set; }

        // Navegação
        public Anuncio? Anuncio { get; private set; }
        public Usuario? Usuario { get; private set; }

        protected VisualizacaoAnuncio() { }

        public VisualizacaoAnuncio(Guid anuncioId, Guid? usuarioId, string ipAddress)
        {
            Id = Guid.NewGuid();
            AnuncioId = anuncioId;
            UsuarioId = usuarioId;
            IpAddress = ipAddress;
            DataVisualizacao = DateTime.UtcNow;
        }
    }
}
