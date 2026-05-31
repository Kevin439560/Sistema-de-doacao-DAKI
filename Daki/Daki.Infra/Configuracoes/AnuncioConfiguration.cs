using Daki.Dominio.Entidades;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Daki.Infra.Configuracoes
{
    public class AnuncioConfiguration : IEntityTypeConfiguration<Anuncio>
    {
        public void Configure(EntityTypeBuilder<Anuncio> builder) {

            // 1. Nome da Tabela
            builder.ToTable("Anuncios");

            // 2. Chave Primária (PK)
            builder.HasKey(a => a.Id);

            // 3. Propriedades
            builder.Property(a => a.Titulo)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(a => a.Descricao)
                .IsRequired()
                .HasColumnType("TEXT");

                // Salvando os Enums como texto no banco para facilitar a leitura
            builder.Property(a => a.Categoria)
                .IsRequired()
                .HasConversion<string>()
                .HasMaxLength(50);

            builder.Property(a => a.Status)
                .IsRequired()
                .HasConversion<string>()
                .HasMaxLength(20);

            // 4. Relacionamentos

            // Relacionamento com Imagens (um para muitos)
            builder.HasMany(a => a.Imagens)
                .WithOne(i => i.Anuncio)
                .HasForeignKey(i => i.AnuncioId)
                .OnDelete(DeleteBehavior.Cascade);

            // Relacionamento com Interesses (um para muitos)
            builder.HasMany(a => a.Interesses)
                .WithOne(i => i.Anuncio)
                .HasForeignKey(i => i.AnuncioId)
                .OnDelete(DeleteBehavior.Cascade);

        }

    }
}
