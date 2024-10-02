using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MultiLanguage.Migrations
{
    public partial class addColumnToProduct : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DescriptionFR",
                table: "Products",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "DescriptionTR",
                table: "Products",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DescriptionFR",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "DescriptionTR",
                table: "Products");
        }
    }
}
