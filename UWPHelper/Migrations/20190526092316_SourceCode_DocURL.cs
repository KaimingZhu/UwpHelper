using Microsoft.EntityFrameworkCore.Migrations;

namespace UWPHelper.Migrations
{
    public partial class SourceCode_DocURL : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DocURL",
                table: "SourceCodes",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DocURL",
                table: "SourceCodes");
        }
    }
}
