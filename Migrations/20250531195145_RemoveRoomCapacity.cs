﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace cinema_cs.Migrations
{
    /// <inheritdoc />
    public partial class RemoveRoomCapacity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Capacity",
                table: "Rooms");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Capacity",
                table: "Rooms",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }
    }
}
