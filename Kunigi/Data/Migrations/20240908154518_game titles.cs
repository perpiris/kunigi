using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kunigi.Data.Migrations
{
    /// <inheritdoc />
    public partial class gametitles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Title",
                table: "ParentGames",
                newName: "SubTitle");

            migrationBuilder.AddColumn<string>(
                name: "MainTitle",
                table: "ParentGames",
                type: "varchar(255)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MainTitle",
                table: "ParentGames");

            migrationBuilder.RenameColumn(
                name: "SubTitle",
                table: "ParentGames",
                newName: "Title");
        }
    }
}
