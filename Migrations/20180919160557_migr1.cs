using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace WebApi.Migrations
{
    public partial class migr1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                                                   "EmailVerifiedTime",
                                                   "Users",
                                                   nullable: true,
                                                   oldClrType: typeof(DateTime));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                                                   "EmailVerifiedTime",
                                                   "Users",
                                                   nullable: false,
                                                   oldClrType: typeof(DateTime),
                                                   oldNullable: true);
        }
    }
}