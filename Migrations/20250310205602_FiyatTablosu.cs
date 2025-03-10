using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UetdsProgramiNet.Migrations
{
    /// <inheritdoc />
    public partial class FiyatTablosu : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Fiyatlar",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AracSayiAciklamasi = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    KullaniciMiktari = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MobilBilgisi = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DestekBilgisi = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DestekSaatleri = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    YedeklemeTuru = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedUsername = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedUsername = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Fiyatlar", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Fiyatlar");
        }
    }
}
