using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace E_Diary.WEB.Data.Migrations
{
    /// <inheritdoc />
    public partial class changedYearGrade : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_YearGrades_CertificationPeriods_CertificationPeriodId",
                table: "YearGrades");

            migrationBuilder.RenameColumn(
                name: "CertificationPeriodId",
                table: "YearGrades",
                newName: "StudyYearId");

            migrationBuilder.RenameIndex(
                name: "IX_YearGrades_CertificationPeriodId",
                table: "YearGrades",
                newName: "IX_YearGrades_StudyYearId");

            migrationBuilder.AddForeignKey(
                name: "FK_YearGrades_StudyYears_StudyYearId",
                table: "YearGrades",
                column: "StudyYearId",
                principalTable: "StudyYears",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_YearGrades_StudyYears_StudyYearId",
                table: "YearGrades");

            migrationBuilder.RenameColumn(
                name: "StudyYearId",
                table: "YearGrades",
                newName: "CertificationPeriodId");

            migrationBuilder.RenameIndex(
                name: "IX_YearGrades_StudyYearId",
                table: "YearGrades",
                newName: "IX_YearGrades_CertificationPeriodId");

            migrationBuilder.AddForeignKey(
                name: "FK_YearGrades_CertificationPeriods_CertificationPeriodId",
                table: "YearGrades",
                column: "CertificationPeriodId",
                principalTable: "CertificationPeriods",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
