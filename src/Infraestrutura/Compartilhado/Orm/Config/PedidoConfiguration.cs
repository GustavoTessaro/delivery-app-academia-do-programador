using DeliveryApp.Dominio.Modulos.Clientes;
using DeliveryApp.Dominio.Modulos.Estabelecimentos;
using DeliveryApp.Dominio.Modulos.Pedidos;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public sealed class PedidoConfiguration : IEntityTypeConfiguration<Pedido>
{
    public void Configure(EntityTypeBuilder<Pedido> builder)
    {
        builder.ToTable("TBPedidos");

        builder.HasKey(p => p.Id);
        builder.Property(p => p.Id).ValueGeneratedNever();

        builder.Property(p => p.EnderecoEntrega)
            .HasMaxLength(Pedido.TamanhoMaximoEndereco)
            .IsRequired();

        builder.Property(p => p.Status)
            .HasConversion<string>()
            .HasMaxLength(30)
            .IsRequired();

        builder.Property(p => p.Subtotal)
            .HasPrecision(10, 2)
            .IsRequired();

        builder.Property(p => p.Total)
            .HasPrecision(10, 2)
            .IsRequired();

        builder.Property(p => p.Versao)
            .IsConcurrencyToken();

        builder.HasIndex(p => new { p.ClienteId, p.CriadoEmUtc });
        builder.HasIndex(p => new { p.EstabelecimentoId, p.Status, p.CriadoEmUtc });

        builder.HasOne<Cliente>()
            .WithMany()
            .HasForeignKey(p => p.ClienteId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Estabelecimento>()
            .WithMany()
            .HasForeignKey(p => p.EstabelecimentoId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(p => p.Itens)
            .WithOne()
            .HasForeignKey(i => i.PedidoId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

public sealed class ItemPedidoConfiguration : IEntityTypeConfiguration<ItemPedido>
{
    public void Configure(EntityTypeBuilder<ItemPedido> builder)
    {
        builder.ToTable("TBItensPedido");

        builder.HasKey(i => i.Id);
        builder.Property(i => i.Id).ValueGeneratedNever();

        builder.Property(i => i.NomeProduto)
            .HasMaxLength(ItemPedido.TamanhoMaximoNomeProduto)
            .IsRequired();

        builder.Property(i => i.Observacao)
            .HasMaxLength(ItemPedido.TamanhoMaximoObservacao);

        builder.Property(i => i.PrecoUnitario)
            .HasPrecision(10, 2)
            .IsRequired();

        builder.Property(i => i.ValorTotal)
            .HasPrecision(10, 2)
            .IsRequired();

        builder.HasMany(i => i.Complementos)
            .WithOne()
            .HasForeignKey(c => c.ItemPedidoId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

public sealed class ComplementoItemPedidoConfiguration : IEntityTypeConfiguration<ComplementoItemPedido>
{
    public void Configure(EntityTypeBuilder<ComplementoItemPedido> builder)
    {
        builder.ToTable("TBComplementosItemPedido");

        builder.HasKey(c => c.Id);
        builder.Property(c => c.Id).ValueGeneratedNever();

        builder.Property(c => c.Nome)
            .HasMaxLength(ComplementoItemPedido.TamanhoMaximoNome)
            .IsRequired();

        builder.Property(c => c.PrecoAdicional)
            .HasPrecision(10, 2)
            .IsRequired();
    }
}
