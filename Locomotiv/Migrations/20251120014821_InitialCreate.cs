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
                    CapaciteMax = table.Column<int>(type: "INTEGER", nullable: false),
                    Latitude = table.Column<double>(type: "REAL", nullable: false),
                    Longitude = table.Column<double>(type: "REAL", nullable: false),
                    HasConflict = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Stations", x => x.IdStation);
                });

            migrationBuilder.CreateTable(
                name: "Signals",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Type = table.Column<string>(type: "TEXT", nullable: false),
                    Etat = table.Column<int>(type: "INTEGER", nullable: false),
                    IdStation = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Signals", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Signals_Stations_IdStation",
                        column: x => x.IdStation,
                        principalTable: "Stations",
                        principalColumn: "IdStation",
                        onDelete: ReferentialAction.Cascade);
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
                    Etat = table.Column<int>(type: "INTEGER", nullable: false),
                    EstEnGare = table.Column<bool>(type: "INTEGER", nullable: false),
                    Locomotive = table.Column<string>(type: "TEXT", nullable: false),
                    NombreWagons = table.Column<int>(type: "INTEGER", nullable: false)
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

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Prenom = table.Column<string>(type: "TEXT", nullable: false),
                    Nom = table.Column<string>(type: "TEXT", nullable: false),
                    Username = table.Column<string>(type: "TEXT", nullable: false),
                    Password = table.Column<string>(type: "TEXT", nullable: true),
                    StationId = table.Column<int>(type: "INTEGER", nullable: true),
                    Role = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Users_Stations_StationId",
                        column: x => x.StationId,
                        principalTable: "Stations",
                        principalColumn: "IdStation",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "Voies",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Nom = table.Column<string>(type: "TEXT", nullable: false),
                    IdStation = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Voies", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Voies_Stations_IdStation",
                        column: x => x.IdStation,
                        principalTable: "Stations",
                        principalColumn: "IdStation",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Blocks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Nom = table.Column<string>(type: "TEXT", nullable: false),
                    StationId = table.Column<int>(type: "INTEGER", nullable: true),
                    TrainOccupantId = table.Column<int>(type: "INTEGER", nullable: true),
                    EtatSignal = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Blocks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Blocks_Stations_StationId",
                        column: x => x.StationId,
                        principalTable: "Stations",
                        principalColumn: "IdStation",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Blocks_Trains_TrainOccupantId",
                        column: x => x.TrainOccupantId,
                        principalTable: "Trains",
                        principalColumn: "IdTrain");
                });

            migrationBuilder.CreateTable(
                name: "PointsInteret",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Nom = table.Column<string>(type: "TEXT", nullable: false),
                    Type = table.Column<string>(type: "TEXT", nullable: false),
                    Latitude = table.Column<double>(type: "REAL", nullable: false),
                    Longitude = table.Column<double>(type: "REAL", nullable: false),
                    BlockId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PointsInteret", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PointsInteret_Blocks_BlockId",
                        column: x => x.BlockId,
                        principalTable: "Blocks",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Blocks_StationId",
                table: "Blocks",
                column: "StationId");

            migrationBuilder.CreateIndex(
                name: "IX_Blocks_TrainOccupantId",
                table: "Blocks",
                column: "TrainOccupantId");

            migrationBuilder.CreateIndex(
                name: "IX_PointsInteret_BlockId",
                table: "PointsInteret",
                column: "BlockId");

            migrationBuilder.CreateIndex(
                name: "IX_Signals_IdStation",
                table: "Signals",
                column: "IdStation");

            migrationBuilder.CreateIndex(
                name: "IX_Trains_IdStation",
                table: "Trains",
                column: "IdStation");

            migrationBuilder.CreateIndex(
                name: "IX_Users_StationId",
                table: "Users",
                column: "StationId");

            migrationBuilder.CreateIndex(
                name: "IX_Voies_IdStation",
                table: "Voies",
                column: "IdStation");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PointsInteret");

            migrationBuilder.DropTable(
                name: "Signals");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Voies");

            migrationBuilder.DropTable(
                name: "Blocks");

            migrationBuilder.DropTable(
                name: "Trains");

            migrationBuilder.DropTable(
                name: "Stations");
        }
    }
}
