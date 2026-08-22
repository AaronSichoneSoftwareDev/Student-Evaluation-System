using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Evaluate.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddTermIsCurrent : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsCurrent",
                table: "Terms",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsCurrent",
                table: "Terms");
        }
    }
}
