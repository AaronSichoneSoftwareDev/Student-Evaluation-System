using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Evaluate.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddEnrollmentYearStatusIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_StudentEnrollments_AcademicYearId",
                table: "StudentEnrollments");

            migrationBuilder.CreateIndex(
                name: "IX_StudentEnrollments_AcademicYearId_Status",
                table: "StudentEnrollments",
                columns: new[] { "AcademicYearId", "Status" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_StudentEnrollments_AcademicYearId_Status",
                table: "StudentEnrollments");

            migrationBuilder.CreateIndex(
                name: "IX_StudentEnrollments_AcademicYearId",
                table: "StudentEnrollments",
                column: "AcademicYearId");
        }
    }
}
