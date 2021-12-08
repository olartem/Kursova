using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Kursova.Migrations
{
    public partial class PurchasesInProducts : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "Purchases",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(2021, 12, 8, 14, 41, 8, 507, DateTimeKind.Local).AddTicks(5834),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValue: new DateTime(2021, 12, 7, 15, 16, 55, 340, DateTimeKind.Local).AddTicks(6138));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "Purchases",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(2021, 12, 7, 15, 16, 55, 340, DateTimeKind.Local).AddTicks(6138),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValue: new DateTime(2021, 12, 8, 14, 41, 8, 507, DateTimeKind.Local).AddTicks(5834));
        }
    }
}
