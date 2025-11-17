using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Locomotiv.Migrations
{
    /// <inheritdoc />
    public partial class AjoutColonneEstEnGare : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Signaux_Stations_IdStation",
                table: "Signaux");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Signaux",
                table: "Signaux");

            migrationBuilder.RenameTable(
                name: "Signaux",
                newName: "Signals");

            migrationBuilder.RenameIndex(
                name: "IX_Signaux_IdStation",
                table: "Signals",
                newName: "IX_Signals_IdStation");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Signals",
                table: "Signals",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Signals_Stations_IdStation",
                table: "Signals",
                column: "IdStation",
                principalTable: "Stations",
                principalColumn: "IdStation",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Signals_Stations_IdStation",
                table: "Signals");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Signals",
                table: "Signals");

            migrationBuilder.RenameTable(
                name: "Signals",
                newName: "Signaux");

            migrationBuilder.RenameIndex(
                name: "IX_Signals_IdStation",
                table: "Signaux",
                newName: "IX_Signaux_IdStation");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Signaux",
                table: "Signaux",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Signaux_Stations_IdStation",
                table: "Signaux",
                column: "IdStation",
                principalTable: "Stations",
                principalColumn: "IdStation",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
