using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace E_Diary.WEB.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddedIndexToGroups : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Students_Groups_GroupYear_GroupLiteral",
                table: "Students");

            migrationBuilder.DropIndex(
                name: "IX_Students_GroupYear_GroupLiteral",
                table: "Students");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Groups",
                table: "Groups");

            migrationBuilder.DropColumn(
                name: "GroupLiteral",
                table: "Students");
            migrationBuilder.DropColumn(
    name: "Id",
    table: "Groups");
            migrationBuilder.RenameColumn(
                name: "GroupYear",
                table: "Students",
                newName: "GroupId");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "Groups",
                type: "int",
                nullable: false)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Groups",
                table: "Groups",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_Students_GroupId",
                table: "Students",
                column: "GroupId");

            migrationBuilder.CreateIndex(
                name: "IX_Groups_Year_Literal",
                table: "Groups",
                columns: new[] { "Year", "Literal" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Students_Groups_GroupId",
                table: "Students",
                column: "GroupId",
                principalTable: "Groups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Students_Groups_GroupId",
                table: "Students");

            migrationBuilder.DropIndex(
                name: "IX_Students_GroupId",
                table: "Students");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Groups",
                table: "Groups");

            migrationBuilder.DropIndex(
                name: "IX_Groups_Year_Literal",
                table: "Groups");

            migrationBuilder.RenameColumn(
                name: "GroupId",
                table: "Students",
                newName: "GroupYear");

            migrationBuilder.AddColumn<string>(
                name: "GroupLiteral",
                table: "Students",
                type: "nvarchar(1)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "Groups",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .OldAnnotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Groups",
                table: "Groups",
                columns: new[] { "Year", "Literal" });

            migrationBuilder.CreateIndex(
                name: "IX_Students_GroupYear_GroupLiteral",
                table: "Students",
                columns: new[] { "GroupYear", "GroupLiteral" });

            migrationBuilder.AddForeignKey(
                name: "FK_Students_Groups_GroupYear_GroupLiteral",
                table: "Students",
                columns: new[] { "GroupYear", "GroupLiteral" },
                principalTable: "Groups",
                principalColumns: new[] { "Year", "Literal" },
                onDelete: ReferentialAction.Cascade);
        }
    }
}
