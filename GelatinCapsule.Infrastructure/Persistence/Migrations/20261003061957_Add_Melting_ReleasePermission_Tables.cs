using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GelatinCapsule.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Add_Melting_ReleasePermission_Tables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Materials",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    DisplayName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<int>(type: "int", nullable: true),
                    OwnerDepartmentId = table.Column<int>(type: "int", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<int>(type: "int", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Materials", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ReleasePermissions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FeedRecNo = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    BatchNo = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Serial = table.Column<int>(type: "int", nullable: false),
                    Farsidate = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Shift = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    FarsiYear = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    Part = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    PType = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    CCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Machine = table.Column<int>(type: "int", nullable: false),
                    Permission = table.Column<int>(type: "int", nullable: false),
                    TankNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    TankMelt1 = table.Column<int>(type: "int", nullable: true),
                    TankMelt2 = table.Column<int>(type: "int", nullable: true),
                    Vol0 = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    Vis0 = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    Vis1 = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    LagTime = table.Column<int>(type: "int", nullable: false),
                    C0 = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    C1 = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    Vol1 = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    Wei0 = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    EqcCapsule = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    WtrNeed = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    Company = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ProductName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ColorCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<int>(type: "int", nullable: true),
                    OwnerDepartmentId = table.Column<int>(type: "int", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<int>(type: "int", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReleasePermissions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProductFormulas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    MaterialId = table.Column<int>(type: "int", nullable: false),
                    Percentage = table.Column<decimal>(type: "decimal(18,8)", precision: 18, scale: 8, nullable: false),
                    SortOrder = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<int>(type: "int", nullable: true),
                    OwnerDepartmentId = table.Column<int>(type: "int", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<int>(type: "int", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductFormulas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductFormulas_Materials_MaterialId",
                        column: x => x.MaterialId,
                        principalTable: "Materials",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ReleasePermissionMaterials",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ReleasePermissionId = table.Column<int>(type: "int", nullable: false),
                    MaterialCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    RequiredGr = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    SortOrder = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReleasePermissionMaterials", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ReleasePermissionMaterials_ReleasePermissions_ReleasePermissionId",
                        column: x => x.ReleasePermissionId,
                        principalTable: "ReleasePermissions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Materials_Code",
                table: "Materials",
                column: "Code",
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_ProductFormulas_CCode_MaterialId",
                table: "ProductFormulas",
                columns: new[] { "CCode", "MaterialId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProductFormulas_MaterialId",
                table: "ProductFormulas",
                column: "MaterialId");

            migrationBuilder.CreateIndex(
                name: "IX_ReleasePermissionMaterials_ReleasePermissionId",
                table: "ReleasePermissionMaterials",
                column: "ReleasePermissionId");

            migrationBuilder.CreateIndex(
                name: "IX_ReleasePermissions_CCode",
                table: "ReleasePermissions",
                column: "CCode");

            migrationBuilder.CreateIndex(
                name: "IX_ReleasePermissions_Farsidate",
                table: "ReleasePermissions",
                column: "Farsidate");

            migrationBuilder.CreateIndex(
                name: "IX_ReleasePermissions_FarsiYear_Serial",
                table: "ReleasePermissions",
                columns: new[] { "FarsiYear", "Serial" });

            migrationBuilder.CreateIndex(
                name: "IX_ReleasePermissions_FeedRecNo",
                table: "ReleasePermissions",
                column: "FeedRecNo",
                unique: true,
                filter: "[IsDeleted] = 0");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProductFormulas");

            migrationBuilder.DropTable(
                name: "ReleasePermissionMaterials");

            migrationBuilder.DropTable(
                name: "Materials");

            migrationBuilder.DropTable(
                name: "ReleasePermissions");
        }
    }
}
