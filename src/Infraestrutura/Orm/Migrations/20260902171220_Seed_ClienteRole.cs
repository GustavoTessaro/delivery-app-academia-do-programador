using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DeliveryApp.Infraestrutura.Orm.Migrations
{
    /// <inheritdoc />
    public partial class Seed_ClienteRole : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { new Guid("01a058f4-a048-79a3-b1a6-0f01d629a126"), "01a058f7-9492-73bc-8e4b-934c53594ed6", "Cliente", "CLIENTE" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("01a058f4-a048-79a3-b1a6-0f01d629a126"));
        }
    }
}
