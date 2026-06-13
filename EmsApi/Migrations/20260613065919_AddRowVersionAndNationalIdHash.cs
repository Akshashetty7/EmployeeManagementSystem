using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EmsApi.Migrations
{
    /// <inheritdoc />
    public partial class AddRowVersionAndNationalIdHash : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Employees_NationalId",
                table: "Employees");

            migrationBuilder.AlterColumn<string>(
                name: "NationalId",
                table: "Employees",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NationalIdHash",
                table: "Employees",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<byte[]>(
                name: "RowVersion",
                table: "Employees",
                type: "rowversion",
                rowVersion: true,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Employees_NationalIdHash",
                table: "Employees",
                column: "NationalIdHash",
                unique: true,
                filter: "[NationalIdHash] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Employees_NationalIdHash",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "NationalIdHash",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "RowVersion",
                table: "Employees");

            migrationBuilder.AlterColumn<string>(
                name: "NationalId",
                table: "Employees",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Employees_NationalId",
                table: "Employees",
                column: "NationalId",
                unique: true,
                filter: "[NationalId] IS NOT NULL");
        }
    }
}
