using DeliveryApp.Dominio.Modulos.Cardapio;
using DeliveryApp.Dominio.Modulos.Estabelecimentos;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DeliveryApp.Infraestrutura.Compartilhado.Orm.Config;

public sealed class CategoriaConfiguration : IEntityTypeConfiguration<Categoria>
{
    public void Configure(EntityTypeBuilder<Categoria> builder)
    {
        builder.ToTable("TBCategorias");

        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).ValueGeneratedNever();

        builder.Property(e => e.EstabelecimentoId).IsRequired();

        builder.Property(e => e.Nome)
            .HasMaxLength(100)
            .IsRequired();

        builder.HasIndex(e => new { e.EstabelecimentoId, e.Nome })
            .IsUnique();

        builder.HasOne<Estabelecimento>()
            .WithMany()
            .HasForeignKey(e => e.EstabelecimentoId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasAlternateKey(e => new { e.Id, e.EstabelecimentoId });
    }
}
