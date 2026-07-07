using FCG.Users.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FCG.Users.Configurations
{
    public class UsuarioConfiguration : IEntityTypeConfiguration<Usuario>
    {
            public void Configure(EntityTypeBuilder<Usuario> builder)
            {
                builder.ToTable("usuarios");

                // Define chave primária
                builder.HasKey(p => p.Id);

                // Força autoincremento começando em 1 e pulando de 1 em 1
                builder.Property(p => p.Id)
                    .HasColumnType("INT")
                    .ValueGeneratedOnAdd()
                    .UseIdentityColumn(1, 1);

                builder.Property(p => p.Nome).HasColumnType("VARCHAR(50)").IsRequired();
                builder.Property(p => p.Email).HasColumnType("VARCHAR(100)").IsRequired();
                builder.Property(p => p.Senha).HasColumnType("VARCHAR(255)").IsRequired();
                builder.Property(p => p.Situacao).HasColumnType("VARCHAR(10)").IsRequired();
            }
    }
}
