using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ServicoFaturamento.Migrations
{
    /// <inheritdoc />
    public partial class AddIdempotencia : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RequisicoesProcessadas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdempotencyKey = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ProcessadoEm = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Resultado = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RequisicoesProcessadas", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RequisicoesProcessadas_IdempotencyKey",
                table: "RequisicoesProcessadas",
                column: "IdempotencyKey",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RequisicoesProcessadas");
        }
    }
}
