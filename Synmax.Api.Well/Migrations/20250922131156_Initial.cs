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
                name: "WellDetails",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Operator = table.Column<string>(type: "text", nullable: false),
                    Status = table.Column<string>(type: "text", nullable: false),
                    WellType = table.Column<string>(type: "text", nullable: false),
                    WorkType = table.Column<string>(type: "text", nullable: false),
                    DirectionalStatus = table.Column<string>(type: "text", nullable: false),
                    MultiLateral = table.Column<string>(type: "text", nullable: false),
                    MineralOwner = table.Column<string>(type: "text", nullable: false),
                    SurfaceOwner = table.Column<string>(type: "text", nullable: false),
                    SurfaceLocation = table.Column<string>(type: "text", nullable: false),
                    GLElevation = table.Column<double>(type: "double precision", nullable: false),
                    KBElevation = table.Column<double>(type: "double precision", nullable: false),
                    DFElevation = table.Column<double>(type: "double precision", nullable: false),
                    SingleMultipleCompletion = table.Column<string>(type: "text", nullable: false),
                    PotashWaiver = table.Column<string>(type: "text", nullable: false),
                    SpudDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastInspection = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    TVD = table.Column<double>(type: "double precision", nullable: false),
                    API = table.Column<string>(type: "text", nullable: false),
                    Latitude = table.Column<double>(type: "double precision", nullable: false),
                    Longitude = table.Column<double>(type: "double precision", nullable: false),
                    CRS = table.Column<string>(type: "text", nullable: false)
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
                name: "WellDetails");
        }
    }
}
