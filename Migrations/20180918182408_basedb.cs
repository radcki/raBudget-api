using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace WebApi.Migrations
{
    public partial class basedb : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                                         "Users",
                                         table => new
                                                  {
                                                      UserId = table.Column<int>(nullable: false)
                                                                    .Annotation("SqlServer:ValueGenerationStrategy",
                                                                                SqlServerValueGenerationStrategy
                                                                                   .IdentityColumn),
                                                      Email = table.Column<string>(nullable: true),
                                                      EmailVerifiedTime = table.Column<DateTime>(nullable: false),
                                                      Username = table.Column<string>(nullable: true),
                                                      Password = table.Column<string>(nullable: true),
                                                      CreationTime = table.Column<DateTime>(nullable: false)
                                                  },
                                         constraints: table => { table.PrimaryKey("PK_Users", x => x.UserId); });

            migrationBuilder.CreateTable(
                                         "Budgets",
                                         table => new
                                                  {
                                                      BudgetId = table.Column<int>(nullable: false)
                                                                      .Annotation("SqlServer:ValueGenerationStrategy",
                                                                                  SqlServerValueGenerationStrategy
                                                                                     .IdentityColumn),
                                                      Name = table.Column<string>(nullable: true),
                                                      StartingDate = table.Column<DateTime>(nullable: false),
                                                      ChangeDateTime = table.Column<DateTime>(nullable: false),
                                                      UserId = table.Column<int>(nullable: false)
                                                  },
                                         constraints: table =>
                                                      {
                                                          table.PrimaryKey("PK_Budgets", x => x.BudgetId);
                                                          table.ForeignKey(
                                                                           "FK_Budgets_Users_UserId",
                                                                           x => x.UserId,
                                                                           "Users",
                                                                           "UserId",
                                                                           onDelete: ReferentialAction.Cascade);
                                                      });

            migrationBuilder.CreateTable(
                                         "PasswordChanges",
                                         table => new
                                                  {
                                                      PasswordChangeId = table.Column<int>(nullable: false)
                                                                              .Annotation("SqlServer:ValueGenerationStrategy",
                                                                                          SqlServerValueGenerationStrategy
                                                                                             .IdentityColumn),
                                                      ChangeDateTime = table.Column<DateTime>(nullable: false),
                                                      UserId = table.Column<int>(nullable: false)
                                                  },
                                         constraints: table =>
                                                      {
                                                          table.PrimaryKey("PK_PasswordChanges",
                                                                           x => x.PasswordChangeId);
                                                          table.ForeignKey(
                                                                           "FK_PasswordChanges_Users_UserId",
                                                                           x => x.UserId,
                                                                           "Users",
                                                                           "UserId",
                                                                           onDelete: ReferentialAction.Cascade);
                                                      });

            migrationBuilder.CreateTable(
                                         "PasswordResets",
                                         table => new
                                                  {
                                                      PasswordResetId = table.Column<int>(nullable: false)
                                                                             .Annotation("SqlServer:ValueGenerationStrategy",
                                                                                         SqlServerValueGenerationStrategy
                                                                                            .IdentityColumn),
                                                      Token = table.Column<string>(nullable: true),
                                                      GenerationDateTime = table.Column<DateTime>(nullable: false),
                                                      UserId = table.Column<int>(nullable: false)
                                                  },
                                         constraints: table =>
                                                      {
                                                          table.PrimaryKey("PK_PasswordResets", x => x.PasswordResetId);
                                                          table.ForeignKey(
                                                                           "FK_PasswordResets_Users_UserId",
                                                                           x => x.UserId,
                                                                           "Users",
                                                                           "UserId",
                                                                           onDelete: ReferentialAction.Cascade);
                                                      });

            migrationBuilder.CreateTable(
                                         "BudgetCategories",
                                         table => new
                                                  {
                                                      BudgetCategoryId = table.Column<int>(nullable: false)
                                                                              .Annotation("SqlServer:ValueGenerationStrategy",
                                                                                          SqlServerValueGenerationStrategy
                                                                                             .IdentityColumn),
                                                      Type = table.Column<int>(nullable: false),
                                                      Icon = table.Column<string>(nullable: true),
                                                      Name = table.Column<string>(nullable: true),
                                                      MonthlyAmount = table.Column<double>(nullable: false),
                                                      ChangeDateTime = table.Column<DateTime>(nullable: false),
                                                      BudgetId = table.Column<int>(nullable: false)
                                                  },
                                         constraints: table =>
                                                      {
                                                          table.PrimaryKey("PK_BudgetCategories",
                                                                           x => x.BudgetCategoryId);
                                                          table.ForeignKey(
                                                                           "FK_BudgetCategories_Budgets_BudgetId",
                                                                           x => x.BudgetId,
                                                                           "Budgets",
                                                                           "BudgetId",
                                                                           onDelete: ReferentialAction.Cascade);
                                                      });

            migrationBuilder.CreateTable(
                                         "Transactions",
                                         table => new
                                                  {
                                                      TransactionId = table.Column<int>(nullable: false)
                                                                           .Annotation("SqlServer:ValueGenerationStrategy",
                                                                                       SqlServerValueGenerationStrategy
                                                                                          .IdentityColumn),
                                                      Description = table.Column<string>(nullable: true),
                                                      Amount = table.Column<double>(nullable: false),
                                                      TransactionDateTime = table.Column<DateTime>(nullable: false),
                                                      CreationDateTime = table.Column<DateTime>(nullable: false),
                                                      CreatedByUserId = table.Column<int>(nullable: false),
                                                      BudgetCategoryId = table.Column<int>(nullable: false)
                                                  },
                                         constraints: table =>
                                                      {
                                                          table.PrimaryKey("PK_Transactions", x => x.TransactionId);
                                                          table.ForeignKey(
                                                                           "FK_Transactions_BudgetCategories_BudgetCategoryId",
                                                                           x => x.BudgetCategoryId,
                                                                           "BudgetCategories",
                                                                           "BudgetCategoryId",
                                                                           onDelete: ReferentialAction.Cascade);
                                                      });

            migrationBuilder.CreateIndex(
                                         "IX_BudgetCategories_BudgetId",
                                         "BudgetCategories",
                                         "BudgetId");

            migrationBuilder.CreateIndex(
                                         "IX_Budgets_UserId",
                                         "Budgets",
                                         "UserId");

            migrationBuilder.CreateIndex(
                                         "IX_PasswordChanges_UserId",
                                         "PasswordChanges",
                                         "UserId");

            migrationBuilder.CreateIndex(
                                         "IX_PasswordResets_UserId",
                                         "PasswordResets",
                                         "UserId");

            migrationBuilder.CreateIndex(
                                         "IX_Transactions_BudgetCategoryId",
                                         "Transactions",
                                         "BudgetCategoryId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                                       "PasswordChanges");

            migrationBuilder.DropTable(
                                       "PasswordResets");

            migrationBuilder.DropTable(
                                       "Transactions");

            migrationBuilder.DropTable(
                                       "BudgetCategories");

            migrationBuilder.DropTable(
                                       "Budgets");

            migrationBuilder.DropTable(
                                       "Users");
        }
    }
}