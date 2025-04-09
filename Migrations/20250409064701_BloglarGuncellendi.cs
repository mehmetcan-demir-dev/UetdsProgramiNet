using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UetdsProgramiNet.Migrations
{
    /// <inheritdoc />
    public partial class BloglarGuncellendi : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Bloglar",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "PublishedDate",
                table: "Bloglar",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Bloglar");

            migrationBuilder.DropColumn(
                name: "PublishedDate",
                table: "Bloglar");
        }
    }
}
