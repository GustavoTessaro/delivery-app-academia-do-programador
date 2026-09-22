using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DeliveryApp.Infraestrutura.Compartilhado.Orm.Migrations
{
    /// <inheritdoc />
    public partial class Add_TBPedidos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TBPedidos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ClienteId = table.Column<Guid>(type: "uuid", nullable: false),
                    EstabelecimentoId = table.Column<Guid>(type: "uuid", nullable: false),
                    EnderecoEntrega = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    Status = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    Subtotal = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: false),
                    TaxaEntrega = table.Column<decimal>(type: "numeric", nullable: false),
                    Total = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: false),
                    CriadoEmUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    AtualizadoEmUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    Versao = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TBPedidos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TBPedidos_TBClientes_ClienteId",
                        column: x => x.ClienteId,
                        principalTable: "TBClientes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TBPedidos_TBEstabelecimentos_EstabelecimentoId",
                        column: x => x.EstabelecimentoId,
                        principalTable: "TBEstabelecimentos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TBItensPedido",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PedidoId = table.Column<Guid>(type: "uuid", nullable: false),
                    ProdutoId = table.Column<Guid>(type: "uuid", nullable: false),
                    NomeProduto = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Quantidade = table.Column<int>(type: "integer", nullable: false),
                    PrecoUnitario = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: false),
                    ValorTotal = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: false),
                    Observacao = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TBItensPedido", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TBItensPedido_TBPedidos_PedidoId",
                        column: x => x.PedidoId,
                        principalTable: "TBPedidos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TBComplementosItemPedido",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ItemPedidoId = table.Column<Guid>(type: "uuid", nullable: false),
                    ComplementoProdutoId = table.Column<Guid>(type: "uuid", nullable: false),
                    Nome = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    PrecoAdicional = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TBComplementosItemPedido", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TBComplementosItemPedido_TBItensPedido_ItemPedidoId",
                        column: x => x.ItemPedidoId,
                        principalTable: "TBItensPedido",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TBComplementosItemPedido_ItemPedidoId",
                table: "TBComplementosItemPedido",
                column: "ItemPedidoId");

            migrationBuilder.CreateIndex(
                name: "IX_TBItensPedido_PedidoId",
                table: "TBItensPedido",
                column: "PedidoId");

            migrationBuilder.CreateIndex(
                name: "IX_TBPedidos_ClienteId_CriadoEmUtc",
                table: "TBPedidos",
                columns: new[] { "ClienteId", "CriadoEmUtc" });

            migrationBuilder.CreateIndex(
                name: "IX_TBPedidos_EstabelecimentoId_Status_CriadoEmUtc",
                table: "TBPedidos",
                columns: new[] { "EstabelecimentoId", "Status", "CriadoEmUtc" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TBComplementosItemPedido");

            migrationBuilder.DropTable(
                name: "TBItensPedido");

            migrationBuilder.DropTable(
                name: "TBPedidos");
        }
    }
}
