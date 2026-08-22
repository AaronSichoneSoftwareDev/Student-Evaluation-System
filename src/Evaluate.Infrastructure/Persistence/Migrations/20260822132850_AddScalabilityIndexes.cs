using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Evaluate.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddScalabilityIndexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_StudentEnrollments_ClassId",
                table: "StudentEnrollments");

            migrationBuilder.DropIndex(
                name: "IX_StudentEnrollments_StudentId",
                table: "StudentEnrollments");

            migrationBuilder.CreateIndex(
                name: "IX_TeacherCourses_ClassId_AcademicYearId_IsActive",
                table: "TeacherCourses",
                columns: new[] { "ClassId", "AcademicYearId", "IsActive" });

            migrationBuilder.CreateIndex(
                name: "IX_StudentEnrollments_ClassId_AcademicYearId_Status",
                table: "StudentEnrollments",
                columns: new[] { "ClassId", "AcademicYearId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_StudentEnrollments_StudentId_AcademicYearId_Status",
                table: "StudentEnrollments",
                columns: new[] { "StudentId", "AcademicYearId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_Evaluation_CourseId_TermId_Status",
                table: "Evaluation",
                columns: new[] { "CourseId", "TermId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_Evaluation_StudentId_CourseId_TermId",
                table: "Evaluation",
                columns: new[] { "StudentId", "CourseId", "TermId" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_TeacherCourses_ClassId_AcademicYearId_IsActive",
                table: "TeacherCourses");

            migrationBuilder.DropIndex(
                name: "IX_StudentEnrollments_ClassId_AcademicYearId_Status",
                table: "StudentEnrollments");

            migrationBuilder.DropIndex(
                name: "IX_StudentEnrollments_StudentId_AcademicYearId_Status",
                table: "StudentEnrollments");

            migrationBuilder.DropIndex(
                name: "IX_Evaluation_CourseId_TermId_Status",
                table: "Evaluation");

            migrationBuilder.DropIndex(
                name: "IX_Evaluation_StudentId_CourseId_TermId",
                table: "Evaluation");

            migrationBuilder.CreateIndex(
                name: "IX_StudentEnrollments_ClassId",
                table: "StudentEnrollments",
                column: "ClassId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentEnrollments_StudentId",
                table: "StudentEnrollments",
                column: "StudentId");
        }
    }
}
