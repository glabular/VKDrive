using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VKDrive.API.Migrations
{
    /// <inheritdoc />
    public partial class VkdriveDbInitialMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "VkdriveEntries",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    OriginalName = table.Column<string>(type: "TEXT", nullable: false),
                    OriginalPath = table.Column<string>(type: "TEXT", nullable: false),
                    UniqueName = table.Column<string>(type: "TEXT", nullable: false),
                    ArchivePassword = table.Column<string>(type: "TEXT", nullable: false),
                    AesKey = table.Column<byte[]>(type: "BLOB", nullable: false),
                    AesIV = table.Column<byte[]>(type: "BLOB", nullable: false),
                    CreationDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Size = table.Column<long>(type: "INTEGER", nullable: false),
                    Checksum = table.Column<string>(type: "TEXT", nullable: false),
                    IsFolder = table.Column<bool>(type: "INTEGER", nullable: false),
                    Links = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VkdriveEntries", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "VkdriveEntries");
        }
    }
}
