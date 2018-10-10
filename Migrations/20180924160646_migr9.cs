using Microsoft.EntityFrameworkCore.Migrations;

namespace WebApi.Migrations
{
    public partial class migr9 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                                          "ActiveBudgetId",
                                          "Users",
                                          "DefaultBudgetId");

            migrationBuilder.AddColumn<string>(
                                               "Currency",
                                               "Budgets",
                                               nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                                        "Currency",
                                        "Budgets");

            migrationBuilder.RenameColumn(
                                          "DefaultBudgetId",
                                          "Users",
                                          "ActiveBudgetId");
        }
    }
}