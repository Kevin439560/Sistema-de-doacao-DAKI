using Daki.Dominio.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Daki.Dominio.Entidades
{
    public class Interesse
    {
        public Guid Id { get; protected set; }
        public Guid UsuarioId { get; private set; }
        public Guid AnuncioId { get; private set; }
        public string Justificativa { get; private set; } = string.Empty;
        public StatusInteresse Status { get; private set; }

        // Navegação
        public Anuncio? Anuncio { get; private set; }
        public Usuario? Usuario { get; private set; }

        // Construtor vazio exigido pelo Entity Framework
        protected Interesse() { }

        public Interesse(Guid usuarioId, Guid anuncioId, string justificativa)
        {
            Id = Guid.NewGuid();
            UsuarioId = usuarioId;
            AnuncioId = anuncioId;
            Justificativa = justificativa;
            Status = StatusInteresse.Pendente;// Todo interesse nasce pendente
        }

        // Métodos de Regra de Negócio
        public void Aceitar()
        {
            Status = StatusInteresse.Aceita;
        }

        public void Recusar()
        {
            Status = StatusInteresse.Recusada;
        }
    }
}
