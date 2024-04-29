using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace E_Diary.WEB.Data.Migrations
{
    /// <inheritdoc />
    public partial class changedgradingsystem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Value",
                table: "Grades",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Value",
                table: "Grades");
        }
    }
}
