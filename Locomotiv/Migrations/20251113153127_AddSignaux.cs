using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Locomotiv.Migrations
{
    /// <inheritdoc />
    public partial class AddSignaux : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Signals_Stations_StationId",
                table: "Signals");

            migrationBuilder.DropForeignKey(
                name: "FK_Users_Stations_StationId",
                table: "Users");

            migrationBuilder.DropForeignKey(
                name: "FK_Voies_Stations_StationId",
                table: "Voies");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Signals",
                table: "Signals");

            migrationBuilder.DropColumn(
                name: "EstDisponible",
                table: "Voies");

            migrationBuilder.RenameTable(
                name: "Signals",
                newName: "Signaux");

            migrationBuilder.RenameColumn(
                name: "StationId",
                table: "Voies",
                newName: "IdStation");

            migrationBuilder.RenameColumn(
                name: "NumeroQuai",
                table: "Voies",
                newName: "Nom");

            migrationBuilder.RenameIndex(
                name: "IX_Voies_StationId",
                table: "Voies",
                newName: "IX_Voies_IdStation");

            migrationBuilder.RenameColumn(
                name: "StationId",
                table: "Signaux",
                newName: "IdStation");

            migrationBuilder.RenameIndex(
                name: "IX_Signals_StationId",
                table: "Signaux",
                newName: "IX_Signaux_IdStation");

            migrationBuilder.AddColumn<bool>(
                name: "EstEnGare",
                table: "Trains",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

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

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Stations_StationId",
                table: "Users",
                column: "StationId",
                principalTable: "Stations",
                principalColumn: "IdStation",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Voies_Stations_IdStation",
                table: "Voies",
                column: "IdStation",
                principalTable: "Stations",
                principalColumn: "IdStation",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Signaux_Stations_IdStation",
                table: "Signaux");

            migrationBuilder.DropForeignKey(
                name: "FK_Users_Stations_StationId",
                table: "Users");

            migrationBuilder.DropForeignKey(
                name: "FK_Voies_Stations_IdStation",
                table: "Voies");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Signaux",
                table: "Signaux");

            migrationBuilder.DropColumn(
                name: "EstEnGare",
                table: "Trains");

            migrationBuilder.RenameTable(
                name: "Signaux",
                newName: "Signals");

            migrationBuilder.RenameColumn(
                name: "Nom",
                table: "Voies",
                newName: "NumeroQuai");

            migrationBuilder.RenameColumn(
                name: "IdStation",
                table: "Voies",
                newName: "StationId");

            migrationBuilder.RenameIndex(
                name: "IX_Voies_IdStation",
                table: "Voies",
                newName: "IX_Voies_StationId");

            migrationBuilder.RenameColumn(
                name: "IdStation",
                table: "Signals",
                newName: "StationId");

            migrationBuilder.RenameIndex(
                name: "IX_Signaux_IdStation",
                table: "Signals",
                newName: "IX_Signals_StationId");

            migrationBuilder.AddColumn<bool>(
                name: "EstDisponible",
                table: "Voies",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Signals",
                table: "Signals",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Signals_Stations_StationId",
                table: "Signals",
                column: "StationId",
                principalTable: "Stations",
                principalColumn: "IdStation",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Stations_StationId",
                table: "Users",
                column: "StationId",
                principalTable: "Stations",
                principalColumn: "IdStation");

            migrationBuilder.AddForeignKey(
                name: "FK_Voies_Stations_StationId",
                table: "Voies",
                column: "StationId",
                principalTable: "Stations",
                principalColumn: "IdStation",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
