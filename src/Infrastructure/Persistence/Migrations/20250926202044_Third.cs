using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Third : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_QueueTickets_MedicalServices_ServiceId",
                table: "QueueTickets");

            migrationBuilder.DropTable(
                name: "DoctorMedicalService");

            migrationBuilder.DropTable(
                name: "MedicalServices");

            migrationBuilder.RenameColumn(
                name: "ServiceId",
                table: "QueueTickets",
                newName: "ProcedureId");

            migrationBuilder.RenameIndex(
                name: "IX_QueueTickets_ServiceId",
                table: "QueueTickets",
                newName: "IX_QueueTickets_ProcedureId");

            migrationBuilder.CreateTable(
                name: "Procedures",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    DepartmentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DoctorId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_QueueTickets_Procedures_ProcedureId",
                table: "QueueTickets");

            migrationBuilder.DropTable(
                name: "DoctorProcedure");

            migrationBuilder.DropTable(
                name: "Procedures");

            migrationBuilder.RenameColumn(
                name: "ProcedureId",
                table: "QueueTickets",
                newName: "ServiceId");

            migrationBuilder.RenameIndex(
                name: "IX_QueueTickets_ProcedureId",
                table: "QueueTickets",
                newName: "IX_QueueTickets_ServiceId");

            migrationBuilder.CreateTable(
                name: "MedicalServices",
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
                    table.PrimaryKey("PK_MedicalServices", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MedicalServices_Departments_DepartmentId",
                        column: x => x.DepartmentId,
                        principalTable: "Departments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DoctorMedicalService",
                columns: table => new
                {
                    DoctorsId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MedicalServicesId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DoctorMedicalService", x => new { x.DoctorsId, x.MedicalServicesId });
                    table.ForeignKey(
                        name: "FK_DoctorMedicalService_Doctors_DoctorsId",
                        column: x => x.DoctorsId,
                        principalTable: "Doctors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DoctorMedicalService_MedicalServices_MedicalServicesId",
                        column: x => x.MedicalServicesId,
                        principalTable: "MedicalServices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DoctorMedicalService_MedicalServicesId",
                table: "DoctorMedicalService",
                column: "MedicalServicesId");

            migrationBuilder.CreateIndex(
                name: "IX_MedicalServices_DepartmentId",
                table: "MedicalServices",
                column: "DepartmentId");

            migrationBuilder.AddForeignKey(
                name: "FK_QueueTickets_MedicalServices_ServiceId",
                table: "QueueTickets",
                column: "ServiceId",
                principalTable: "MedicalServices",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
