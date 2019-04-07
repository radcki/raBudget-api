using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace WebApi.Migrations
{
    public partial class migr3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TransactionScheduleId",
                table: "Transactions",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "TransactionSchedules",
                columns: table => new
                {
                    TransactionScheduleId = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    BudgetCategoryId = table.Column<int>(nullable: false),
                    Description = table.Column<string>(nullable: false),
                    Amount = table.Column<double>(nullable: false),
                    Frequency = table.Column<int>(nullable: false),
                    PeriodStep = table.Column<int>(nullable: false),
                    StartDate = table.Column<DateTime>(nullable: false),
                    EndDate = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TransactionSchedules", x => x.TransactionScheduleId);
                    table.ForeignKey(
                        name: "FK_TransactionSchedules_BudgetCategories_BudgetCategoryId",
                        column: x => x.BudgetCategoryId,
                        principalTable: "BudgetCategories",
                        principalColumn: "BudgetCategoryId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_TransactionScheduleId",
                table: "Transactions",
                column: "TransactionScheduleId");

            migrationBuilder.CreateIndex(
                name: "IX_TransactionSchedules_BudgetCategoryId",
                table: "TransactionSchedules",
                column: "BudgetCategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_Transactions_TransactionSchedules_TransactionScheduleId",
                table: "Transactions",
                column: "TransactionScheduleId",
                principalTable: "TransactionSchedules",
                principalColumn: "TransactionScheduleId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Transactions_TransactionSchedules_TransactionScheduleId",
                table: "Transactions");

            migrationBuilder.DropTable(
                name: "TransactionSchedules");

            migrationBuilder.DropIndex(
                name: "IX_Transactions_TransactionScheduleId",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "TransactionScheduleId",
                table: "Transactions");
        }
    }
}
