using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kunigi.Data.Migrations
{
    /// <inheritdoc />
    public partial class teamimagestrings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ImageUrl",
                table: "Teams",
                newName: "TeamFolderUrl");

            migrationBuilder.AddColumn<string>(
                name: "ProfileImageUrl",
                table: "Teams",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProfileImageUrl",
                table: "Teams");

            migrationBuilder.RenameColumn(
                name: "TeamFolderUrl",
                table: "Teams",
                newName: "ImageUrl");
        }
    }
}
