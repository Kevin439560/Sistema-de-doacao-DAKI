using System;
using System.Collections.Generic;
using System.Text;

namespace Daki.Dominio.Entidades
{
    public class Endereco
    {
        public Guid Id { get; private set; }
        public Guid idUsuario { get; private set; }
        public string CEP { get; private set; } = string.Empty;
        public string Rua { get; private set; } = string.Empty;
        public string Numero { get; private set; } = string.Empty;
        public string Bairro { get; private set; } = string.Empty;
        public string Cidade { get; private set; } = string.Empty;
        public string UF { get; private set; } = string.Empty;

        public Usuario Usuario { get; private set; }
        public ICollection<Anuncio> Anuncios { get; private set; } = new List<Anuncio>();

        protected Endereco()
        {
        }

        public Endereco(Guid idUsuario, string cep, string rua, string numero, string bairro, string cidade, string uf)
        {
            Id = Guid.NewGuid();
            this.idUsuario = idUsuario;
            CEP = cep;
            Rua = rua;
            Numero = numero;
            Bairro = bairro;
            Cidade = cidade;
            UF = uf;
        }

    }
}
