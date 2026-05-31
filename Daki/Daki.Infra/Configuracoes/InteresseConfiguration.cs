using Daki.Dominio.Entidades;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Daki.Infra.Configuracoes
{
    public class InteresseConfiguration : IEntityTypeConfiguration<Interesse>
    {
        public void Configure(EntityTypeBuilder<Interesse> builder)
        {
            builder.ToTable("Interesses");

            builder.HasKey(i => i.Id);

            builder.Property(i => i.Justificativa)
                .IsRequired()
                .HasColumnType("text");

            builder.Property(i => i.Status)
                .IsRequired()
                .HasConversion<string>()
                .HasMaxLength(20);
        }
    }
}
