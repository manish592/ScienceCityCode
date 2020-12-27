using Microsoft.EntityFrameworkCore.Migrations;

namespace Science.City.EF.dbmodel.Migrations
{
    public partial class asasasasa : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "NewsPaperCompany",
                table: "Users",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PressIDCard",
                table: "Users",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PressReporterName",
                table: "Users",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NewsPaperCompany",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "PressIDCard",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "PressReporterName",
                table: "Users");
        }
    }
}
