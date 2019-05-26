using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace UWPHelper.Migrations
{
    public partial class SourceCodeUpdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "name",
                table: "SourceCodes",
                newName: "Name");

            migrationBuilder.AddColumn<DateTime>(
                name: "LastEditDate",
                table: "SourceCodes",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "SearchTime",
                table: "SourceCodes",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastEditDate",
                table: "SourceCodes");

            migrationBuilder.DropColumn(
                name: "SearchTime",
                table: "SourceCodes");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "SourceCodes",
                newName: "name");
        }
    }
}
