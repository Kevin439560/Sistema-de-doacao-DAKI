using Daki.Dominio.Entidades;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Daki.Infra.Configuracoes
{
    public class EnderecoConfiguration : IEntityTypeConfiguration<Endereco>
    {
        public void Configure(EntityTypeBuilder<Endereco> builder)
        {
            // 1. Nome da Tabela
            builder.ToTable("Enderecos");

            // 2. Chave Primária (PK)
            builder.HasKey(e => e.Id);

            // 3. Propriedades
            builder.Property(e => e.CEP)
                .IsRequired()
                .HasMaxLength(15);

            builder.Property(e => e.Rua)
                .IsRequired()
                .HasMaxLength(150);

            builder.Property(e => e.Numero)
                .IsRequired()
                .HasMaxLength(10);

            builder.Property(e => e.Bairro)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(e => e.Cidade)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(e => e.UF)
                .IsRequired()
                .HasMaxLength(2);

            // 4. Relacionamentos

            // Relacionamento com Anuncio (1:N)
            builder.HasMany(e => e.Anuncios)
                .WithOne(a => a.Endereco)
                .HasForeignKey(a => a.EnderecoId)
                .OnDelete(DeleteBehavior.Restrict);// Não deixa apagar um endereço se houver um anúncio usando ele
        }
    }
}