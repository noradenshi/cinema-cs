using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace cinema_cs.Migrations
{
    /// <inheritdoc />
    public partial class PriceTier : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MaxDaysBefore",
                table: "PriceTiers",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "MinDaysBefore",
                table: "PriceTiers",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "PriceTiers",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "MaxDaysBefore", "MinDaysBefore" },
                values: new object[] { 0, -1 });

            migrationBuilder.UpdateData(
                table: "PriceTiers",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "MaxDaysBefore", "MinDaysBefore" },
                values: new object[] { 2, 1 });

            migrationBuilder.UpdateData(
                table: "PriceTiers",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "MaxDaysBefore", "MinDaysBefore" },
                values: new object[] { 4, 3 });

            migrationBuilder.UpdateData(
                table: "PriceTiers",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "MaxDaysBefore", "MinDaysBefore" },
                values: new object[] { 8, 5 });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MaxDaysBefore",
                table: "PriceTiers");

            migrationBuilder.DropColumn(
                name: "MinDaysBefore",
                table: "PriceTiers");
        }
    }
}
