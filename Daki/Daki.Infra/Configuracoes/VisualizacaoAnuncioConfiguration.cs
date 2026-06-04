using Daki.Dominio.Entidades;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Daki.Infra.Configuracoes
{
    public class VisualizacaoAnuncioConfiguration : IEntityTypeConfiguration<VisualizacaoAnuncio>
    {
        public void Configure(EntityTypeBuilder<VisualizacaoAnuncio> builder)
        {
            builder.ToTable("VisualizacoesAnuncio");

            builder.HasKey(v => v.Id);

            builder.Property(v => v.IpAddress)
                .HasMaxLength(45); // Tamanho suficiente para IPv4 e IPv6

            builder.Property(v => v.DataVisualizacao)
                .IsRequired();

            // Relacionamentos

            // Visualizacao N : 1 Anuncio
            builder.HasOne(v => v.Anuncio)
                .WithMany(a => a.Visualizacoes)
                .HasForeignKey(v => v.AnuncioId)
                .OnDelete(DeleteBehavior.Cascade); // Se apagar o anúncio, limpa o histórico de views dele

            // Visualizacao N : 1 Usuario (Opcional)
            builder.HasOne(v => v.Usuario)
                .WithMany() // O usuário não precisa de uma lista das coisas que ele viu no domínio
                .HasForeignKey(v => v.UsuarioId)
                .OnDelete(DeleteBehavior.SetNull); // Se o usuário for deletado, mantemos a visualização, mas com ID nulo
        }
    }
}
