using Daki.Dominio.Entidades;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Daki.Infra.Data
{
    public class DakiContext : DbContext
    {

        // O construtor recebe as opções (como a string de conexão) lá do projeto Web
        public DakiContext(DbContextOptions<DakiContext> options) : base(options)
        {
        }
        //  as tabelas no banco
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Endereco> Enderecos { get; set; }
        public DbSet<Anuncio> Anuncios { get; set; }
        public DbSet<ImagemAnuncio> ImagensAnuncio { get; set; }
        public DbSet<Interesse> Interesses { get; set; }
        public DbSet<VisualizacaoAnuncio> VisualizacoesAnuncio { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(DakiContext).Assembly);
        }
    }
}
