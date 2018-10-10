using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace WebApi.Migrations
{
    public partial class migr6 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                                         "RefreshTokens",
                                         table => new
                                                  {
                                                      RefreshTokenId = table.Column<int>(nullable: false)
                                                                            .Annotation("SqlServer:ValueGenerationStrategy",
                                                                                        SqlServerValueGenerationStrategy
                                                                                           .IdentityColumn),
                                                      Token = table.Column<string>(nullable: false),
                                                      ClientId = table.Column<string>(nullable: false),
                                                      ValidTo = table.Column<DateTime>(nullable: false),
                                                      WasUsed = table.Column<bool>(nullable: false),
                                                      UserId = table.Column<int>(nullable: false)
                                                  },
                                         constraints: table =>
                                                      {
                                                          table.PrimaryKey("PK_RefreshTokens", x => x.RefreshTokenId);
                                                          table.ForeignKey(
                                                                           "FK_RefreshTokens_Users_UserId",
                                                                           x => x.UserId,
                                                                           "Users",
                                                                           "UserId",
                                                                           onDelete: ReferentialAction.Cascade);
                                                      });

            migrationBuilder.CreateIndex(
                                         "IX_RefreshTokens_ClientId",
                                         "RefreshTokens",
                                         "ClientId",
                                         unique: true);

            migrationBuilder.CreateIndex(
                                         "IX_RefreshTokens_UserId",
                                         "RefreshTokens",
                                         "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                                       "RefreshTokens");
        }
    }
}