using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Procedure : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_QueueTickets_Procedures_ProcedureId",
                table: "QueueTickets");

            migrationBuilder.DropTable(
                name: "DoctorProcedure");

            migrationBuilder.DropTable(
                name: "Procedures");

            migrationBuilder.AddColumn<Guid>(
                name: "ProcedureId",
                table: "Doctors",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ProcedureId1",
                table: "Doctors",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Departments",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Departments",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<Guid>(
                name: "DoctorId",
                table: "Departments",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "ParentCategoryId",
                table: "Departments",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Doctors_ProcedureId1",
                table: "Doctors",
                column: "ProcedureId1");

            migrationBuilder.CreateIndex(
                name: "IX_Departments_ParentCategoryId",
                table: "Departments",
                column: "ParentCategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_Departments_Departments_ParentCategoryId",
                table: "Departments",
                column: "ParentCategoryId",
                principalTable: "Departments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Doctors_Departments_ProcedureId1",
                table: "Doctors",
                column: "ProcedureId1",
                principalTable: "Departments",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_QueueTickets_Departments_ProcedureId",
                table: "QueueTickets",
                column: "ProcedureId",
                principalTable: "Departments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Departments_Departments_ParentCategoryId",
                table: "Departments");

            migrationBuilder.DropForeignKey(
                name: "FK_Doctors_Departments_ProcedureId1",
                table: "Doctors");

            migrationBuilder.DropForeignKey(
                name: "FK_QueueTickets_Departments_ProcedureId",
                table: "QueueTickets");

            migrationBuilder.DropIndex(
                name: "IX_Doctors_ProcedureId1",
                table: "Doctors");

            migrationBuilder.DropIndex(
                name: "IX_Departments_ParentCategoryId",
                table: "Departments");

            migrationBuilder.DropColumn(
                name: "ProcedureId",
                table: "Doctors");

            migrationBuilder.DropColumn(
                name: "ProcedureId1",
                table: "Doctors");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Departments");

            migrationBuilder.DropColumn(
                name: "DoctorId",
                table: "Departments");

            migrationBuilder.DropColumn(
                name: "ParentCategoryId",
                table: "Departments");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Departments",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.CreateTable(
                name: "Procedures",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DepartmentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    DoctorId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Procedures", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Procedures_Departments_DepartmentId",
                        column: x => x.DepartmentId,
                        principalTable: "Departments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DoctorProcedure",
                columns: table => new
                {
                    DoctorsId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MedicalServicesId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DoctorProcedure", x => new { x.DoctorsId, x.MedicalServicesId });
                    table.ForeignKey(
                        name: "FK_DoctorProcedure_Doctors_DoctorsId",
                        column: x => x.DoctorsId,
                        principalTable: "Doctors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DoctorProcedure_Procedures_MedicalServicesId",
                        column: x => x.MedicalServicesId,
                        principalTable: "Procedures",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DoctorProcedure_MedicalServicesId",
                table: "DoctorProcedure",
                column: "MedicalServicesId");

            migrationBuilder.CreateIndex(
                name: "IX_Procedures_DepartmentId",
                table: "Procedures",
                column: "DepartmentId");

            migrationBuilder.AddForeignKey(
                name: "FK_QueueTickets_Procedures_ProcedureId",
                table: "QueueTickets",
                column: "ProcedureId",
                principalTable: "Procedures",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
