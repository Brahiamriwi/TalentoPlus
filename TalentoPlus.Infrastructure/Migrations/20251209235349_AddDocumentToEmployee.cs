using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TalentoPlus.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddDocumentToEmployee : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Document",
                table: "Employees",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Document",
                table: "Employees");
        }
    }
}
