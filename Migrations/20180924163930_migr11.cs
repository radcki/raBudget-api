using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace WebApi.Migrations
{
    public partial class migr11 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                                        "ChangeDateTime",
                                        "BudgetCategories");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                                                 "ChangeDateTime",
                                                 "BudgetCategories",
                                                 nullable: false,
                                                 defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0,
                                                                            DateTimeKind.Unspecified));
        }
    }
}