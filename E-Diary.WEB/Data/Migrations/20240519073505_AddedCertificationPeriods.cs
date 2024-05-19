using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace E_Diary.WEB.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddedCertificationPeriods : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "StudyYears",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Start = table.Column<DateOnly>(type: "date", nullable: false),
                    End = table.Column<DateOnly>(type: "date", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudyYears", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CertificationPeriods",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StudyYearId = table.Column<int>(type: "int", nullable: false),
                    Start = table.Column<DateOnly>(type: "date", nullable: false),
                    End = table.Column<DateOnly>(type: "date", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CertificationPeriods", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CertificationPeriods_StudyYears_StudyYearId",
                        column: x => x.StudyYearId,
                        principalTable: "StudyYears",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PeriodGrades",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Value = table.Column<int>(type: "int", nullable: false),
                    PeriodInfoId = table.Column<int>(type: "int", nullable: false),
                    CertificationPeriodId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PeriodGrades", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PeriodGrades_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PeriodGrades_CertificationPeriods_CertificationPeriodId",
                        column: x => x.CertificationPeriodId,
                        principalTable: "CertificationPeriods",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PeriodGrades_TeacherGroupSubjects_PeriodInfoId",
                        column: x => x.PeriodInfoId,
                        principalTable: "TeacherGroupSubjects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "YearGrades",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Value = table.Column<int>(type: "int", nullable: false),
                    YearInfoId = table.Column<int>(type: "int", nullable: false),
                    CertificationPeriodId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_YearGrades", x => x.Id);
                    table.ForeignKey(
                        name: "FK_YearGrades_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_YearGrades_CertificationPeriods_CertificationPeriodId",
                        column: x => x.CertificationPeriodId,
                        principalTable: "CertificationPeriods",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_YearGrades_TeacherGroupSubjects_YearInfoId",
                        column: x => x.YearInfoId,
                        principalTable: "TeacherGroupSubjects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CertificationPeriods_StudyYearId",
                table: "CertificationPeriods",
                column: "StudyYearId");

            migrationBuilder.CreateIndex(
                name: "IX_PeriodGrades_CertificationPeriodId",
                table: "PeriodGrades",
                column: "CertificationPeriodId");

            migrationBuilder.CreateIndex(
                name: "IX_PeriodGrades_PeriodInfoId",
                table: "PeriodGrades",
                column: "PeriodInfoId");

            migrationBuilder.CreateIndex(
                name: "IX_PeriodGrades_UserId",
                table: "PeriodGrades",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_YearGrades_CertificationPeriodId",
                table: "YearGrades",
                column: "CertificationPeriodId");

            migrationBuilder.CreateIndex(
                name: "IX_YearGrades_UserId",
                table: "YearGrades",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_YearGrades_YearInfoId",
                table: "YearGrades",
                column: "YearInfoId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PeriodGrades");

            migrationBuilder.DropTable(
                name: "YearGrades");

            migrationBuilder.DropTable(
                name: "CertificationPeriods");

            migrationBuilder.DropTable(
                name: "StudyYears");
        }
    }
}
