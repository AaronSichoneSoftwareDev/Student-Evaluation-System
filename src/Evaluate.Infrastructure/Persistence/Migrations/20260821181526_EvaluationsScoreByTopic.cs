using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Evaluate.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class EvaluationsScoreByTopic : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EvaluationResults_EvaluationCriteria_CriteriaId",
                table: "EvaluationResults");

            migrationBuilder.DropColumn(
                name: "MaxScore",
                table: "EvaluationResults");

            migrationBuilder.DropColumn(
                name: "TopicId",
                table: "Evaluation");

            migrationBuilder.RenameColumn(
                name: "CriteriaId",
                table: "EvaluationResults",
                newName: "TopicId");

            migrationBuilder.RenameIndex(
                name: "IX_EvaluationResults_CriteriaId",
                table: "EvaluationResults",
                newName: "IX_EvaluationResults_TopicId");

            migrationBuilder.AddColumn<string>(
                name: "Comment",
                table: "EvaluationResults",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_EvaluationResults_Topics_TopicId",
                table: "EvaluationResults",
                column: "TopicId",
                principalTable: "Topics",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EvaluationResults_Topics_TopicId",
                table: "EvaluationResults");

            migrationBuilder.DropColumn(
                name: "Comment",
                table: "EvaluationResults");

            migrationBuilder.RenameColumn(
                name: "TopicId",
                table: "EvaluationResults",
                newName: "CriteriaId");

            migrationBuilder.RenameIndex(
                name: "IX_EvaluationResults_TopicId",
                table: "EvaluationResults",
                newName: "IX_EvaluationResults_CriteriaId");

            migrationBuilder.AddColumn<decimal>(
                name: "MaxScore",
                table: "EvaluationResults",
                type: "TEXT",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "TopicId",
                table: "Evaluation",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddForeignKey(
                name: "FK_EvaluationResults_EvaluationCriteria_CriteriaId",
                table: "EvaluationResults",
                column: "CriteriaId",
                principalTable: "EvaluationCriteria",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
