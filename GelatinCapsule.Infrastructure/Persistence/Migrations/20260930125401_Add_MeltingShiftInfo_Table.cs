using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GelatinCapsule.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Add_MeltingShiftInfo_Table : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MeltingShiftInfos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Farsidate = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Shift = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Group = table.Column<string>(type: "nvarchar(5)", maxLength: 5, nullable: true),
                    Supervisore = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    SheftHeader = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    GelMaker1 = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    GelMaker2 = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    IpQc = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    TankWasher = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    TankWasher2 = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    SheftHeaderRep = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    SheftHeaderRepFrom = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    GelMaker1Rep = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    GelMaker1RepFrom = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    GelMaker2Rep = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    GelMaker2RepFrom = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    TankWasherRep = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    TankWasherRepFrom = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    Mildate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Momtime = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    CurrentDay = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    HelpMelter = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    YearF = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<int>(type: "int", nullable: true),
                    OwnerDepartmentId = table.Column<int>(type: "int", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<int>(type: "int", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MeltingShiftInfos", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MeltingShiftInfos_Farsidate",
                table: "MeltingShiftInfos",
                column: "Farsidate");

            migrationBuilder.CreateIndex(
                name: "UX_MeltingShiftInfo_Date_Shift",
                table: "MeltingShiftInfos",
                columns: new[] { "Farsidate", "Shift" },
                unique: true,
                filter: "[IsDeleted] = 0");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MeltingShiftInfos");
        }
    }
}
