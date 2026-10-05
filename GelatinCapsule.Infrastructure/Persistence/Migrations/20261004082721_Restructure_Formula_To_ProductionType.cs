using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GelatinCapsule.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Restructure_Formula_To_ProductionType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ProductFormulas_CCode_MaterialId",
                table: "ProductFormulas");

            migrationBuilder.DropColumn(
                name: "CCode",
                table: "ProductFormulas");

            migrationBuilder.AddColumn<int>(
                name: "ProductionTypeId",
                table: "ProductFormulas",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_ProductFormulas_ProductionTypeId_MaterialId",
                table: "ProductFormulas",
                columns: new[] { "ProductionTypeId", "MaterialId" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductFormulas_ProductionTypes_ProductionTypeId",
                table: "ProductFormulas",
                column: "ProductionTypeId",
                principalTable: "ProductionTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductFormulas_ProductionTypes_ProductionTypeId",
                table: "ProductFormulas");

            migrationBuilder.DropIndex(
                name: "IX_ProductFormulas_ProductionTypeId_MaterialId",
                table: "ProductFormulas");

            migrationBuilder.DropColumn(
                name: "ProductionTypeId",
                table: "ProductFormulas");

            migrationBuilder.AddColumn<string>(
                name: "CCode",
                table: "ProductFormulas",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_ProductFormulas_CCode_MaterialId",
                table: "ProductFormulas",
                columns: new[] { "CCode", "MaterialId" },
                unique: true);
        }
    }
}
