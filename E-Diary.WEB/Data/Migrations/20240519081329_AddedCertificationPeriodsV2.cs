using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace E_Diary.WEB.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddedCertificationPeriodsV2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "CertificationPeriods",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Name",
                table: "CertificationPeriods");
        }
    }
}
