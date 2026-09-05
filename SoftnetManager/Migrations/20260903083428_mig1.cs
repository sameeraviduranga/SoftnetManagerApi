using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SoftnetManager.Migrations
{
    /// <inheritdoc />
    public partial class mig1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Address_Users_UserID",
                table: "Address");

            migrationBuilder.DropForeignKey(
                name: "FK_Address_Zones_ZoneID",
                table: "Address");

            migrationBuilder.DropIndex(
                name: "IX_Address_UserID",
                table: "Address");

            migrationBuilder.DropColumn(
                name: "UserID",
                table: "Address");

            migrationBuilder.AlterColumn<int>(
                name: "ZoneID",
                table: "Address",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "Line3",
                table: "Address",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.UpdateData(
                table: "Address",
                keyColumn: "ID",
                keyValue: 1,
                column: "Line3",
                value: null);

            migrationBuilder.AddForeignKey(
                name: "FK_Address_Zones_ZoneID",
                table: "Address",
                column: "ZoneID",
                principalTable: "Zones",
                principalColumn: "ID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Address_Zones_ZoneID",
                table: "Address");

            migrationBuilder.AlterColumn<int>(
                name: "ZoneID",
                table: "Address",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Line3",
                table: "Address",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UserID",
                table: "Address",
                type: "int",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Address",
                keyColumn: "ID",
                keyValue: 1,
                columns: new[] { "Line3", "UserID" },
                values: new object[] { "", null });

            migrationBuilder.CreateIndex(
                name: "IX_Address_UserID",
                table: "Address",
                column: "UserID");

            migrationBuilder.AddForeignKey(
                name: "FK_Address_Users_UserID",
                table: "Address",
                column: "UserID",
                principalTable: "Users",
                principalColumn: "ID");

            migrationBuilder.AddForeignKey(
                name: "FK_Address_Zones_ZoneID",
                table: "Address",
                column: "ZoneID",
                principalTable: "Zones",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
