﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kunigi.Data.Migrations
{
    /// <inheritdoc />
    public partial class remaining : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ParentType",
                table: "MediaFiles");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ParentType",
                table: "MediaFiles",
                type: "varchar(50)",
                nullable: true);
        }
    }
}
