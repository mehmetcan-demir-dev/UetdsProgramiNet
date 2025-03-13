using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UetdsProgramiNet.Migrations
{
    /// <inheritdoc />
    public partial class BlogsFinished : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SubDescription",
                table: "Bloglar",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SubDescription",
                table: "Bloglar");
        }
    }
}
