using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace WebApi.Migrations
{
    public partial class migr5 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                                        "Role",
                                        "Users");

            migrationBuilder.AlterColumn<string>(
                                                 "Name",
                                                 "Budgets",
                                                 nullable: false,
                                                 oldClrType: typeof(string),
                                                 oldNullable: true);

            migrationBuilder.CreateTable(
                                         "UserRoles",
                                         table => new
                                                  {
                                                      UserRoleId = table.Column<int>(nullable: false)
                                                                        .Annotation("SqlServer:ValueGenerationStrategy",
                                                                                    SqlServerValueGenerationStrategy
                                                                                       .IdentityColumn),
                                                      Role = table.Column<int>(nullable: false),
                                                      UserId = table.Column<int>(nullable: false)
                                                  },
                                         constraints: table =>
                                                      {
                                                          table.PrimaryKey("PK_UserRoles", x => x.UserRoleId);
                                                          table.ForeignKey(
                                                                           "FK_UserRoles_Users_UserId",
                                                                           x => x.UserId,
                                                                           "Users",
                                                                           "UserId",
                                                                           onDelete: ReferentialAction.Cascade);
                                                      });

            migrationBuilder.CreateIndex(
                                         "IX_Users_Email",
                                         "Users",
                                         "Email",
                                         unique: true);

            migrationBuilder.CreateIndex(
                                         "IX_Users_Username",
                                         "Users",
                                         "Username",
                                         unique: true);

            migrationBuilder.CreateIndex(
                                         "IX_UserRoles_UserId",
                                         "UserRoles",
                                         "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                                       "UserRoles");

            migrationBuilder.DropIndex(
                                       "IX_Users_Email",
                                       "Users");

            migrationBuilder.DropIndex(
                                       "IX_Users_Username",
                                       "Users");

            migrationBuilder.AddColumn<int>(
                                            "Role",
                                            "Users",
                                            nullable: false,
                                            defaultValue: 0);

            migrationBuilder.AlterColumn<string>(
                                                 "Name",
                                                 "Budgets",
                                                 nullable: true,
                                                 oldClrType: typeof(string));
        }
    }
}