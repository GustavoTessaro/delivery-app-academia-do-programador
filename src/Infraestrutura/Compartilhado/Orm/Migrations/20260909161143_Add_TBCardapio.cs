using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DeliveryApp.Infraestrutura.Compartilhado.Orm.Migrations
{
    /// <inheritdoc />
    public partial class Add_TBCardapio : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TBCategorias",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    EstabelecimentoId = table.Column<Guid>(type: "uuid", nullable: false),
                    Nome = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TBCategorias", x => x.Id);
                    table.UniqueConstraint("AK_TBCategorias_Id_EstabelecimentoId", x => new { x.Id, x.EstabelecimentoId });
                    table.ForeignKey(
                        name: "FK_TBCategorias_TBEstabelecimentos_EstabelecimentoId",
                        column: x => x.EstabelecimentoId,
                        principalTable: "TBEstabelecimentos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TBProdutos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    EstabelecimentoId = table.Column<Guid>(type: "uuid", nullable: false),
                    CategoriaId = table.Column<Guid>(type: "uuid", nullable: false),
                    Nome = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Descricao = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    Preco = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: false),
                    Ativo = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TBProdutos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TBProdutos_TBCategorias_CategoriaId_EstabelecimentoId",
                        columns: x => new { x.CategoriaId, x.EstabelecimentoId },
                        principalTable: "TBCategorias",
                        principalColumns: new[] { "Id", "EstabelecimentoId" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TBProdutos_TBEstabelecimentos_EstabelecimentoId",
                        column: x => x.EstabelecimentoId,
                        principalTable: "TBEstabelecimentos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TBComplementos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ProdutoId = table.Column<Guid>(type: "uuid", nullable: false),
                    Nome = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    PrecoAdicional = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TBComplementos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TBComplementos_TBProdutos_ProdutoId",
                        column: x => x.ProdutoId,
                        principalTable: "TBProdutos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TBCategorias_EstabelecimentoId_Nome",
                table: "TBCategorias",
                columns: new[] { "EstabelecimentoId", "Nome" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TBComplementos_ProdutoId",
                table: "TBComplementos",
                column: "ProdutoId");

            migrationBuilder.CreateIndex(
                name: "IX_TBProdutos_CategoriaId_EstabelecimentoId",
                table: "TBProdutos",
                columns: new[] { "CategoriaId", "EstabelecimentoId" });

            migrationBuilder.CreateIndex(
                name: "IX_TBProdutos_EstabelecimentoId",
                table: "TBProdutos",
                column: "EstabelecimentoId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TBComplementos");

            migrationBuilder.DropTable(
                name: "TBProdutos");

            migrationBuilder.DropTable(
                name: "TBCategorias");
        }
    }
}
