using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kunigi.Data.Migrations
{
    /// <inheritdoc />
    public partial class gameorder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ImageUrl",
                table: "GameYears",
                newName: "TeamFolderUrl");

            migrationBuilder.AlterColumn<string>(
                name: "TeamFolderUrl",
                table: "Teams",
                type: "varchar(255)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AddColumn<short>(
                name: "Order",
                table: "GameYears",
                type: "INTEGER",
                nullable: false,
                defaultValue: (short)0);

            migrationBuilder.AddColumn<string>(
                name: "ProfileImageUrl",
                table: "GameYears",
                type: "varchar(255)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Order",
                table: "GameYears");

            migrationBuilder.DropColumn(
                name: "ProfileImageUrl",
                table: "GameYears");

            migrationBuilder.RenameColumn(
                name: "TeamFolderUrl",
                table: "GameYears",
                newName: "ImageUrl");

            migrationBuilder.AlterColumn<string>(
                name: "TeamFolderUrl",
                table: "Teams",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(255)",
                oldNullable: true);
        }
    }
}
