using Microsoft.EntityFrameworkCore.Migrations;

namespace WebApi.Migrations
{
    public partial class migr2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                                                 "Username",
                                                 "Users",
                                                 maxLength: 30,
                                                 nullable: false,
                                                 oldClrType: typeof(string),
                                                 oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                                                 "Password",
                                                 "Users",
                                                 maxLength: 160,
                                                 nullable: false,
                                                 oldClrType: typeof(string),
                                                 oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                                                 "Email",
                                                 "Users",
                                                 maxLength: 160,
                                                 nullable: false,
                                                 oldClrType: typeof(string),
                                                 oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                                                 "Description",
                                                 "Transactions",
                                                 nullable: false,
                                                 oldClrType: typeof(string),
                                                 oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                                                 "Token",
                                                 "PasswordResets",
                                                 nullable: false,
                                                 oldClrType: typeof(string),
                                                 oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                                                 "Name",
                                                 "BudgetCategories",
                                                 nullable: false,
                                                 oldClrType: typeof(string),
                                                 oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                                                 "Icon",
                                                 "BudgetCategories",
                                                 nullable: false,
                                                 oldClrType: typeof(string),
                                                 oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                                                 "Username",
                                                 "Users",
                                                 nullable: true,
                                                 oldClrType: typeof(string),
                                                 oldMaxLength: 30);

            migrationBuilder.AlterColumn<string>(
                                                 "Password",
                                                 "Users",
                                                 nullable: true,
                                                 oldClrType: typeof(string),
                                                 oldMaxLength: 160);

            migrationBuilder.AlterColumn<string>(
                                                 "Email",
                                                 "Users",
                                                 nullable: true,
                                                 oldClrType: typeof(string),
                                                 oldMaxLength: 160);

            migrationBuilder.AlterColumn<string>(
                                                 "Description",
                                                 "Transactions",
                                                 nullable: true,
                                                 oldClrType: typeof(string));

            migrationBuilder.AlterColumn<string>(
                                                 "Token",
                                                 "PasswordResets",
                                                 nullable: true,
                                                 oldClrType: typeof(string));

            migrationBuilder.AlterColumn<string>(
                                                 "Name",
                                                 "BudgetCategories",
                                                 nullable: true,
                                                 oldClrType: typeof(string));

            migrationBuilder.AlterColumn<string>(
                                                 "Icon",
                                                 "BudgetCategories",
                                                 nullable: true,
                                                 oldClrType: typeof(string));
        }
    }
}