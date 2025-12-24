using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class yo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Incidencias_Atracciones_AtraccionId1",
                table: "Incidencias");

            migrationBuilder.DropIndex(
                name: "IX_Incidencias_AtraccionId1",
                table: "Incidencias");

            migrationBuilder.DropColumn(
                name: "AtraccionId1",
                table: "Incidencias");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "AtraccionId1",
                table: "Incidencias",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Incidencias_AtraccionId1",
                table: "Incidencias",
                column: "AtraccionId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Incidencias_Atracciones_AtraccionId1",
                table: "Incidencias",
                column: "AtraccionId1",
                principalTable: "Atracciones",
                principalColumn: "Id");
        }
    }
}
