using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Synmax.Api.Well.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DatabaseSeeded",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    SeededAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DatabaseSeeded", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "WellDetails",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Operator = table.Column<string>(type: "TEXT", nullable: false),
                    Status = table.Column<string>(type: "TEXT", nullable: false),
                    WellType = table.Column<string>(type: "TEXT", nullable: false),
                    WorkType = table.Column<string>(type: "TEXT", nullable: false),
                    DirectionalStatus = table.Column<string>(type: "TEXT", nullable: false),
                    MultiLateral = table.Column<string>(type: "TEXT", nullable: false),
                    MineralOwner = table.Column<string>(type: "TEXT", nullable: false),
                    SurfaceOwner = table.Column<string>(type: "TEXT", nullable: false),
                    SurfaceLocation = table.Column<string>(type: "TEXT", nullable: false),
                    GLElevation = table.Column<double>(type: "REAL", nullable: false),
                    KBElevation = table.Column<double>(type: "REAL", nullable: false),
                    DFElevation = table.Column<double>(type: "REAL", nullable: false),
                    SingleMultipleCompletion = table.Column<string>(type: "TEXT", nullable: false),
                    PotashWaiver = table.Column<string>(type: "TEXT", nullable: false),
                    SpudDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    LastInspection = table.Column<DateTime>(type: "TEXT", nullable: false),
                    TVD = table.Column<double>(type: "REAL", nullable: false),
                    API = table.Column<string>(type: "TEXT", nullable: false),
                    Latitude = table.Column<double>(type: "REAL", nullable: false),
                    Longitude = table.Column<double>(type: "REAL", nullable: false),
                    CRS = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WellDetails", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DatabaseSeeded");

            migrationBuilder.DropTable(
                name: "WellDetails");
        }
    }
}
