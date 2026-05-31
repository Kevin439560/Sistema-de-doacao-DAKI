using Daki.Dominio.Entidades;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Daki.Infra.Configuracoes
{
    public class UsuarioConfiguration : IEntityTypeConfiguration<Usuario>
    {
        public void Configure(EntityTypeBuilder<Usuario> builder)
        {
            // 1. Nome da Tabela
            builder.ToTable("Usuarios");

            // 2. Chave Primária (PK)
            builder.HasKey(u => u.Id);

            // 3. Propriedades
            builder.Property(u => u.Nome)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(u => u.Email)
                .IsRequired()
                .HasMaxLength(150);

            builder.Property(u => u.PasswordHash)
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(u => u.Fone)
                .HasMaxLength(20);

            builder.Property(u => u.DataCriacao)
                .IsRequired();

            // 4. Relacionamentos

            // Relacionamento 1:N com Anuncio
            builder.HasMany(u => u.Anuncios)
                .WithOne(a => a.Usuario)
                .HasForeignKey(a => a.UsuarioId)
                .OnDelete(DeleteBehavior.Restrict);// Evita deletar um usuário se ele tiver anúncios ativos

            // Relacionamento 1:N com Interesse
            builder.HasMany(u => u.Interesses)
                .WithOne(i => i.Usuario)
                .HasForeignKey(i => i.UsuarioId)
                .OnDelete(DeleteBehavior.Cascade);// Se deletar o usuário, deleta os interesses relacionados

            // Relacionamento 1:1 com Endereco
            builder.HasOne(u => u.Endereco)
                .WithOne(e => e.Usuario)
                .HasForeignKey<Endereco>(e => e.UsuarioId)
                .OnDelete(DeleteBehavior.Cascade);// Se deletar o usuário, deleta o endereço
        }
    }
}
