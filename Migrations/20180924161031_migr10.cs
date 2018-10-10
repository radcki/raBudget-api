using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace WebApi.Migrations
{
    public partial class migr10 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                                        "ChangeDateTime",
                                        "Budgets");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                                                 "ChangeDateTime",
                                                 "Budgets",
                                                 nullable: false,
                                                 defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0,
                                                                            DateTimeKind.Unspecified));
        }
    }
}