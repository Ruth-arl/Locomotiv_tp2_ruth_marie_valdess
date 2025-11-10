using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Locomotiv.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Stations",
                columns: table => new
                {
                    IdStation = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Nom = table.Column<string>(type: "TEXT", nullable: true),
                    Ville = table.Column<string>(type: "TEXT", nullable: true),
                    CapaciteMax = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Stations", x => x.IdStation);
                });

            migrationBuilder.CreateTable(
                name: "Trains",
                columns: table => new
                {
                    IdTrain = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    IdStation = table.Column<int>(type: "INTEGER", nullable: false),
                    HeureDepart = table.Column<DateTime>(type: "TEXT", nullable: false),
                    HeureArrivee = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Type = table.Column<int>(type: "INTEGER", nullable: false),
                    Etat = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Trains", x => x.IdTrain);
                    table.ForeignKey(
                        name: "FK_Trains_Stations_IdStation",
                        column: x => x.IdStation,
                        principalTable: "Stations",
                        principalColumn: "IdStation",
                        onDelete: ReferentialAction.Restrict);
                });

            //migrationBuilder.CreateTable(
            //    name: "Users",
            //    columns: table => new
            //    {
            //        Id = table.Column<int>(type: "INTEGER", nullable: false)
            //            .Annotation("Sqlite:Autoincrement", true),
            //        Prenom = table.Column<string>(type: "TEXT", nullable: false),
            //        Nom = table.Column<string>(type: "TEXT", nullable: false),
            //        Username = table.Column<string>(type: "TEXT", nullable: false),
            //        Password = table.Column<string>(type: "TEXT", nullable: false),
            //        StationId = table.Column<int>(type: "INTEGER", nullable: true),
            //        Role = table.Column<string>(type: "TEXT", nullable: true)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_Users", x => x.Id);
            //        table.ForeignKey(
            //            name: "FK_Users_Stations_StationId",
            //            column: x => x.StationId,
            //            principalTable: "Stations",
            //            principalColumn: "IdStation");
            //    });

            migrationBuilder.CreateIndex(
                name: "IX_Trains_IdStation",
                table: "Trains",
                column: "IdStation");

            migrationBuilder.CreateIndex(
                name: "IX_Users_StationId",
                table: "Users",
                column: "StationId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Trains");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Stations");
        }
    }
}
