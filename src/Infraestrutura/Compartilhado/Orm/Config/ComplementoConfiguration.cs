using DeliveryApp.Dominio.Modulos.Cardapio;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DeliveryApp.Infraestrutura.Compartilhado.Orm.Config;

public sealed class ComplementoConfiguration : IEntityTypeConfiguration<Complemento>
{
    public void Configure(EntityTypeBuilder<Complemento> builder)
    {
        builder.ToTable("TBComplementos");

        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).ValueGeneratedNever();

        builder.Property(e => e.ProdutoId).IsRequired();

        builder.Property(e => e.Nome)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(e => e.PrecoAdicional)
            .HasPrecision(10, 2)
            .IsRequired();
    }
}
