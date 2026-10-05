using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GelatinCapsule.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Add_OwnerDepartmentId_To_All_Tables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "OwnerDepartmentId",
                table: "Users",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "OwnerDepartmentId",
                table: "Roles",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "OwnerDepartmentId",
                table: "Permissions",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "OwnerDepartmentId",
                table: "Modules",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "OwnerDepartmentId",
                table: "Forms",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "OwnerDepartmentId",
                table: "Departments",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OwnerDepartmentId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "OwnerDepartmentId",
                table: "Roles");

            migrationBuilder.DropColumn(
                name: "OwnerDepartmentId",
                table: "Permissions");

            migrationBuilder.DropColumn(
                name: "OwnerDepartmentId",
                table: "Modules");

            migrationBuilder.DropColumn(
                name: "OwnerDepartmentId",
                table: "Forms");

            migrationBuilder.DropColumn(
                name: "OwnerDepartmentId",
                table: "Departments");
        }
    }
}
