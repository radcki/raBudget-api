using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace WebApi.Migrations
{
    public partial class migr1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MonthlyAmount",
                table: "BudgetCategories");

            migrationBuilder.CreateTable(
                name: "BudgetCategoryAmountConfigs",
                columns: table => new
                {
                    BudgetCategoryAmountConfigId = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    MonthlyAmount = table.Column<double>(nullable: false),
                    ValidFrom = table.Column<DateTime>(nullable: false),
                    ValidTo = table.Column<DateTime>(nullable: true),
                    BudgetCategoryId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BudgetCategoryAmountConfigs", x => x.BudgetCategoryAmountConfigId);
                    table.ForeignKey(
                        name: "FK_BudgetCategoryAmountConfigs_BudgetCategories_BudgetCategoryId",
                        column: x => x.BudgetCategoryId,
                        principalTable: "BudgetCategories",
                        principalColumn: "BudgetCategoryId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BudgetCategoryAmountConfigs_BudgetCategoryId",
                table: "BudgetCategoryAmountConfigs",
                column: "BudgetCategoryId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BudgetCategoryAmountConfigs");

            migrationBuilder.AddColumn<double>(
                name: "MonthlyAmount",
                table: "BudgetCategories",
                nullable: false,
                defaultValue: 0.0);
        }
    }
}
