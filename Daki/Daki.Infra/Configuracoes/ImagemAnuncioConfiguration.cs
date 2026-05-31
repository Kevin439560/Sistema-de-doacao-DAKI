using Daki.Dominio.Entidades;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Daki.Infra.Configuracoes
{
    public class ImagemAnuncioConfiguration : IEntityTypeConfiguration<ImagemAnuncio>
    {
        public void Configure(EntityTypeBuilder<ImagemAnuncio> builder)
        {
            builder.ToTable("ImagensAnuncio");

            builder.HasKey(i => i.Id);

            builder.Property(i => i.UrlImagem)
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(i => i.Principal)
                .IsRequired();
        }
    }
}
