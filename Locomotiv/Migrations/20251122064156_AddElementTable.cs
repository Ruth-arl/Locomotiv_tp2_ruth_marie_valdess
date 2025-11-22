using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Locomotiv.Migrations
{
    /// <inheritdoc />
    public partial class AddElementTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Blocks_Stations_StationId",
                table: "Blocks");

            migrationBuilder.DropForeignKey(
                name: "FK_PointsInteret_Blocks_BlockId",
                table: "PointsInteret");

            migrationBuilder.DropIndex(
                name: "IX_PointsInteret_BlockId",
                table: "PointsInteret");

            migrationBuilder.DropColumn(
                name: "HasConflict",
                table: "Stations");

            migrationBuilder.DropColumn(
                name: "Etat",
                table: "Signals");

            migrationBuilder.DropColumn(
                name: "BlockId",
                table: "PointsInteret");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Voies",
                newName: "IdVoie");

            migrationBuilder.RenameColumn(
                name: "StationId",
                table: "Blocks",
                newName: "ItineraireIdItineraire");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Blocks",
                newName: "IdBlock");

            migrationBuilder.RenameIndex(
                name: "IX_Blocks_StationId",
                table: "Blocks",
                newName: "IX_Blocks_ItineraireIdItineraire");

            migrationBuilder.AlterColumn<string>(
                name: "Nom",
                table: "Voies",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AddColumn<bool>(
                name: "EstOccupee",
                table: "Voies",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "IdTrainActuel",
                table: "Voies",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Numero",
                table: "Voies",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "TrainActuelIdTrain",
                table: "Voies",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "BlockAssocieIdBlock",
                table: "Signals",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EtatSignal",
                table: "Signals",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "IdBlockAssocie",
                table: "Signals",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "EtatSignal",
                table: "Blocks",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AddColumn<double>(
                name: "LatitudeDebut",
                table: "Blocks",
                type: "REAL",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "LatitudeFin",
                table: "Blocks",
                type: "REAL",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "LongitudeDebut",
                table: "Blocks",
                type: "REAL",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "LongitudeFin",
                table: "Blocks",
                type: "REAL",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.CreateTable(
                name: "Itineraires",
                columns: table => new
                {
                    IdItineraire = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    IdTrain = table.Column<int>(type: "INTEGER", nullable: false),
                    HeureDepart = table.Column<DateTime>(type: "TEXT", nullable: false),
                    HeureArriveeEstimee = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Itineraires", x => x.IdItineraire);
                    table.ForeignKey(
                        name: "FK_Itineraires_Trains_IdTrain",
                        column: x => x.IdTrain,
                        principalTable: "Trains",
                        principalColumn: "IdTrain",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ArretsItineraire",
                columns: table => new
                {
                    IdArret = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    IdItineraire = table.Column<int>(type: "INTEGER", nullable: false),
                    Ordre = table.Column<int>(type: "INTEGER", nullable: false),
                    IdStation = table.Column<int>(type: "INTEGER", nullable: true),
                    StationIdStation = table.Column<int>(type: "INTEGER", nullable: true),
                    IdPointInteret = table.Column<int>(type: "INTEGER", nullable: true),
                    PointInteretId = table.Column<int>(type: "INTEGER", nullable: true),
                    HeureArrivee = table.Column<DateTime>(type: "TEXT", nullable: false),
                    HeureDepart = table.Column<DateTime>(type: "TEXT", nullable: false),
                    DureeArret = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ArretsItineraire", x => x.IdArret);
                    table.ForeignKey(
                        name: "FK_ArretsItineraire_Itineraires_IdItineraire",
                        column: x => x.IdItineraire,
                        principalTable: "Itineraires",
                        principalColumn: "IdItineraire",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ArretsItineraire_PointsInteret_PointInteretId",
                        column: x => x.PointInteretId,
                        principalTable: "PointsInteret",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ArretsItineraire_Stations_StationIdStation",
                        column: x => x.StationIdStation,
                        principalTable: "Stations",
                        principalColumn: "IdStation");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Voies_TrainActuelIdTrain",
                table: "Voies",
                column: "TrainActuelIdTrain");

            migrationBuilder.CreateIndex(
                name: "IX_Signals_BlockAssocieIdBlock",
                table: "Signals",
                column: "BlockAssocieIdBlock");

            migrationBuilder.CreateIndex(
                name: "IX_ArretsItineraire_IdItineraire",
                table: "ArretsItineraire",
                column: "IdItineraire");

            migrationBuilder.CreateIndex(
                name: "IX_ArretsItineraire_PointInteretId",
                table: "ArretsItineraire",
                column: "PointInteretId");

            migrationBuilder.CreateIndex(
                name: "IX_ArretsItineraire_StationIdStation",
                table: "ArretsItineraire",
                column: "StationIdStation");

            migrationBuilder.CreateIndex(
                name: "IX_Itineraires_IdTrain",
                table: "Itineraires",
                column: "IdTrain");

            migrationBuilder.AddForeignKey(
                name: "FK_Blocks_Itineraires_ItineraireIdItineraire",
                table: "Blocks",
                column: "ItineraireIdItineraire",
                principalTable: "Itineraires",
                principalColumn: "IdItineraire");

            migrationBuilder.AddForeignKey(
                name: "FK_Signals_Blocks_BlockAssocieIdBlock",
                table: "Signals",
                column: "BlockAssocieIdBlock",
                principalTable: "Blocks",
                principalColumn: "IdBlock");

            migrationBuilder.AddForeignKey(
                name: "FK_Voies_Trains_TrainActuelIdTrain",
                table: "Voies",
                column: "TrainActuelIdTrain",
                principalTable: "Trains",
                principalColumn: "IdTrain");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Blocks_Itineraires_ItineraireIdItineraire",
                table: "Blocks");

            migrationBuilder.DropForeignKey(
                name: "FK_Signals_Blocks_BlockAssocieIdBlock",
                table: "Signals");

            migrationBuilder.DropForeignKey(
                name: "FK_Voies_Trains_TrainActuelIdTrain",
                table: "Voies");

            migrationBuilder.DropTable(
                name: "ArretsItineraire");

            migrationBuilder.DropTable(
                name: "Itineraires");

            migrationBuilder.DropIndex(
                name: "IX_Voies_TrainActuelIdTrain",
                table: "Voies");

            migrationBuilder.DropIndex(
                name: "IX_Signals_BlockAssocieIdBlock",
                table: "Signals");

            migrationBuilder.DropColumn(
                name: "EstOccupee",
                table: "Voies");

            migrationBuilder.DropColumn(
                name: "IdTrainActuel",
                table: "Voies");

            migrationBuilder.DropColumn(
                name: "Numero",
                table: "Voies");

            migrationBuilder.DropColumn(
                name: "TrainActuelIdTrain",
                table: "Voies");

            migrationBuilder.DropColumn(
                name: "BlockAssocieIdBlock",
                table: "Signals");

            migrationBuilder.DropColumn(
                name: "EtatSignal",
                table: "Signals");

            migrationBuilder.DropColumn(
                name: "IdBlockAssocie",
                table: "Signals");

            migrationBuilder.DropColumn(
                name: "LatitudeDebut",
                table: "Blocks");

            migrationBuilder.DropColumn(
                name: "LatitudeFin",
                table: "Blocks");

            migrationBuilder.DropColumn(
                name: "LongitudeDebut",
                table: "Blocks");

            migrationBuilder.DropColumn(
                name: "LongitudeFin",
                table: "Blocks");

            migrationBuilder.RenameColumn(
                name: "IdVoie",
                table: "Voies",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "ItineraireIdItineraire",
                table: "Blocks",
                newName: "StationId");

            migrationBuilder.RenameColumn(
                name: "IdBlock",
                table: "Blocks",
                newName: "Id");

            migrationBuilder.RenameIndex(
                name: "IX_Blocks_ItineraireIdItineraire",
                table: "Blocks",
                newName: "IX_Blocks_StationId");

            migrationBuilder.AlterColumn<string>(
                name: "Nom",
                table: "Voies",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "HasConflict",
                table: "Stations",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "Etat",
                table: "Signals",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "BlockId",
                table: "PointsInteret",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "EtatSignal",
                table: "Blocks",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.CreateIndex(
                name: "IX_PointsInteret_BlockId",
                table: "PointsInteret",
                column: "BlockId");

            migrationBuilder.AddForeignKey(
                name: "FK_Blocks_Stations_StationId",
                table: "Blocks",
                column: "StationId",
                principalTable: "Stations",
                principalColumn: "IdStation",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_PointsInteret_Blocks_BlockId",
                table: "PointsInteret",
                column: "BlockId",
                principalTable: "Blocks",
                principalColumn: "Id");
        }
    }
}
