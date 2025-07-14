using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QuizMaster.Migrations
{
    /// <inheritdoc />
    public partial class ChangeQuizCategoryConstraint : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Quizzes_Categories_CategoryId",
                table: "Quizzes");

            migrationBuilder.AddForeignKey(
                name: "FK_Quizzes_Categories_CategoryId",
                table: "Quizzes",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Quizzes_Categories_CategoryId",
                table: "Quizzes");

            migrationBuilder.AddForeignKey(
                name: "FK_Quizzes_Categories_CategoryId",
                table: "Quizzes",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
