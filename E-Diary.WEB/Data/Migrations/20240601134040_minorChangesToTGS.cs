using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace E_Diary.WEB.Data.Migrations
{
    /// <inheritdoc />
    public partial class minorChangesToTGS : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "StudyYearId",
                table: "TeacherGroupSubjects",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_TeacherGroupSubjects_StudyYearId",
                table: "TeacherGroupSubjects",
                column: "StudyYearId");

            migrationBuilder.AddForeignKey(
                name: "FK_TeacherGroupSubjects_StudyYears_StudyYearId",
                table: "TeacherGroupSubjects",
                column: "StudyYearId",
                principalTable: "StudyYears",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TeacherGroupSubjects_StudyYears_StudyYearId",
                table: "TeacherGroupSubjects");

            migrationBuilder.DropIndex(
                name: "IX_TeacherGroupSubjects_StudyYearId",
                table: "TeacherGroupSubjects");

            migrationBuilder.DropColumn(
                name: "StudyYearId",
                table: "TeacherGroupSubjects");
        }
    }
}
