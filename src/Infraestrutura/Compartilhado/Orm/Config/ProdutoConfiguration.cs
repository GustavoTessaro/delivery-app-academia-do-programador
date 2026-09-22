using DeliveryApp.Dominio.Modulos.Cardapio;
using DeliveryApp.Dominio.Modulos.Estabelecimentos;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DeliveryApp.Infraestrutura.Compartilhado.Orm.Config;

public sealed class ProdutoConfiguration : IEntityTypeConfiguration<Produto>
{
    public void Configure(EntityTypeBuilder<Produto> builder)
    {
        builder.ToTable("TBProdutos");

        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).ValueGeneratedNever();

        builder.Property(e => e.EstabelecimentoId).IsRequired();
        builder.Property(e => e.CategoriaId).IsRequired();

        builder.Property(e => e.Nome)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(e => e.Descricao)
            .HasMaxLength(1000)
            .IsRequired();

        builder.Property(e => e.Preco)
            .HasPrecision(10, 2)
            .IsRequired();

        builder.Property(e => e.Ativo).IsRequired();

        builder.HasOne<Estabelecimento>()
            .WithMany()
            .HasForeignKey(e => e.EstabelecimentoId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.Categoria)
            .WithMany()
            .HasForeignKey(e => new { e.CategoriaId, e.EstabelecimentoId })
            .HasPrincipalKey(e => new { e.Id, e.EstabelecimentoId })
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(e => e.Complementos)
            .WithOne()
            .HasForeignKey(e => e.ProdutoId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
