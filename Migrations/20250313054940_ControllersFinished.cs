using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UetdsProgramiNet.Migrations
{
    /// <inheritdoc />
    public partial class ControllersFinished : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Bloglar",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    InfoUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ImgUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedUsername = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedUsername = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bloglar", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Bloglar");

            migrationBuilder.DropColumn(
                name: "ImgUrl",
                table: "Sliders");

            migrationBuilder.DropColumn(
                name: "InfoUrl",
                table: "Sliders");

            migrationBuilder.RenameColumn(
                name: "SubDescription",
                table: "Sliders",
                newName: "Info");

            migrationBuilder.RenameColumn(
                name: "AracPaketi",
                table: "Fiyatlar",
                newName: "AracSayiAciklamasi");
        }
    }
}
