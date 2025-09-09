using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Second : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MedicalServices_Departments_ServiceCategoryId",
                table: "MedicalServices");

            migrationBuilder.RenameColumn(
                name: "ServiceCategoryId",
                table: "MedicalServices",
                newName: "DepartmentId");

            migrationBuilder.RenameIndex(
                name: "IX_MedicalServices_ServiceCategoryId",
                table: "MedicalServices",
                newName: "IX_MedicalServices_DepartmentId");

            migrationBuilder.AddForeignKey(
                name: "FK_MedicalServices_Departments_DepartmentId",
                table: "MedicalServices",
                column: "DepartmentId",
                principalTable: "Departments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MedicalServices_Departments_DepartmentId",
                table: "MedicalServices");

            migrationBuilder.RenameColumn(
                name: "DepartmentId",
                table: "MedicalServices",
                newName: "ServiceCategoryId");

            migrationBuilder.RenameIndex(
                name: "IX_MedicalServices_DepartmentId",
                table: "MedicalServices",
                newName: "IX_MedicalServices_ServiceCategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_MedicalServices_Departments_ServiceCategoryId",
                table: "MedicalServices",
                column: "ServiceCategoryId",
                principalTable: "Departments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
