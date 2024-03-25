using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace E_Diary.WEB.Data.Migrations
{
    /// <inheritdoc />
    public partial class ChangedFalseConstraintValues : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "ValidLessonNumber",
                table: "Lessons");

            migrationBuilder.AddCheckConstraint(
                name: "ValidLessonNumber",
                table: "Lessons",
                sql: "LessonOnDayNumber > 0 AND LessonOnDayNumber < 11");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "ValidLessonNumber",
                table: "Lessons");

            migrationBuilder.AddCheckConstraint(
                name: "ValidLessonNumber",
                table: "Lessons",
                sql: "LessonOnDayNumber > 1 AND LessonOnDayNumber < 10");
        }
    }
}
