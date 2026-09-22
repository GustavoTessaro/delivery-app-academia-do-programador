using DeliveryApp.Dominio.Modulos.Clientes;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DeliveryApp.Infraestrutura.Orm.Config;

public sealed class ClienteConfiguration : IEntityTypeConfiguration<Cliente>
{
    public void Configure(EntityTypeBuilder<Cliente> builder)
    {
        builder.ToTable("TBClientes");

        builder.HasKey(c => c.Id);
        builder.Property(c => c.Id).ValueGeneratedNever();

        builder.Property(c => c.Nome)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(c => c.Cpf)
            .HasMaxLength(11)
            .IsRequired();

        builder.HasIndex(c => c.Cpf).IsUnique();

        builder.HasOne<IdentityUser<Guid>>()
            .WithOne()
            .HasForeignKey<Cliente>(c => c.Id)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
