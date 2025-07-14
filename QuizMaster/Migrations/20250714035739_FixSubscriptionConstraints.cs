using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QuizMaster.Migrations
{
    /// <inheritdoc />
    public partial class FixSubscriptionConstraints : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Subscriptions_Users_OrganizerId",
                table: "Subscriptions");

            migrationBuilder.DropForeignKey(
                name: "FK_Subscriptions_Users_SubscriberId",
                table: "Subscriptions");

            migrationBuilder.AddForeignKey(
                name: "FK_Subscriptions_Users_OrganizerId",
                table: "Subscriptions",
                column: "OrganizerId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Subscriptions_Users_SubscriberId",
                table: "Subscriptions",
                column: "SubscriberId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Subscriptions_Users_OrganizerId",
                table: "Subscriptions");

            migrationBuilder.DropForeignKey(
                name: "FK_Subscriptions_Users_SubscriberId",
                table: "Subscriptions");

            migrationBuilder.AddForeignKey(
                name: "FK_Subscriptions_Users_OrganizerId",
                table: "Subscriptions",
                column: "OrganizerId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Subscriptions_Users_SubscriberId",
                table: "Subscriptions",
                column: "SubscriberId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
