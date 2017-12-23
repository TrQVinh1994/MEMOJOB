using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace MEMO_JOB.Data.MigrationSeekers
{
    public partial class MEMO_JOB_SEEKERS : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "APP_JOB_SEEKERS_ROLES",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    ConcurrencyStamp = table.Column<string>(nullable: true),
                    Discriminator = table.Column<string>(nullable: false),
                    Name = table.Column<string>(maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(maxLength: 256, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_APP_JOB_SEEKERS_ROLES", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "APP_JOB_SEEKERS_USERS",
                columns: table => new
                {
                    Idd = table.Column<long>(nullable: false),
                    Id = table.Column<string>(nullable: false),
                    FullName = table.Column<string>(maxLength: 256, nullable: false),
                    Avatar = table.Column<string>(nullable: true),
                    Address = table.Column<string>(nullable: true),
                    AccessFailedCount = table.Column<int>(nullable: false),
                    Birthday = table.Column<DateTime>(nullable: true),
                    City = table.Column<string>(maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: false),
                    DateUpdated = table.Column<DateTime>(nullable: true),
                    Discriminator = table.Column<string>(nullable: false),
                    Email = table.Column<string>(maxLength: 256, nullable: false),
                    EmailConfirmed = table.Column<bool>(nullable: false),
                    LastLoginDate = table.Column<DateTime>(nullable: true),
                    LockoutEnabled = table.Column<bool>(nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(nullable: true),
                    MaritalStatus = table.Column<bool>(nullable: false, defaultValue: 0),
                    Nation = table.Column<string>(maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(maxLength: 256, nullable: true),
                    PasswordHash = table.Column<string>(nullable: true),
                    PasswordChangeDate = table.Column<DateTime>(nullable: true),
                    PhoneNumber = table.Column<string>(nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(nullable: false),
                    Gender = table.Column<string>(maxLength: 5, nullable: true),
                    Subscribe = table.Column<bool>(nullable: false, defaultValue: 0),
                    SecurityStamp = table.Column<string>(nullable: true),
                    TwoFactorEnabled = table.Column<bool>(nullable: false),
                    UserName = table.Column<string>(maxLength: 256, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_APP_JOB_SEEKERS_USERS", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "APP_JOB_SEEKERS_ROLE_CLAIMS",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ClaimType = table.Column<string>(nullable: true),
                    ClaimValue = table.Column<string>(nullable: true),
                    RoleId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_APP_JOB_SEEKERS_ROLE_CLAIMS", x => x.Id);
                    table.ForeignKey(
                        name: "FK_APP_JOB_SEEKERS_ROLE_CLAIMS_APP_JOB_SEEKERS_ROLES_RoleId",
                        column: x => x.RoleId,
                        principalTable: "APP_JOB_SEEKERS_ROLES",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "APP_JOB_SEEKERS_USER_CLAIMS",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ClaimType = table.Column<string>(nullable: true),
                    ClaimValue = table.Column<string>(nullable: true),
                    UserId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_APP_JOB_SEEKERS_USER_CLAIMS", x => x.Id);
                    table.ForeignKey(
                        name: "FK_APP_JOB_SEEKERS_USER_CLAIMS_APP_JOB_SEEKERS_USERS_UserId",
                        column: x => x.UserId,
                        principalTable: "APP_JOB_SEEKERS_USERS",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "APP_JOB_SEEKERS_USER_EXTERNAL_LOGIN",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(nullable: false),
                    ProviderKey = table.Column<string>(nullable: false),
                    ProviderDisplayName = table.Column<string>(nullable: true),
                    UserId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_APP_JOB_SEEKERS_USER_EXTERNAL_LOGIN", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_APP_JOB_SEEKERS_USER_EXTERNAL_LOGIN_APP_JOB_SEEKERS_USERS_UserId",
                        column: x => x.UserId,
                        principalTable: "APP_JOB_SEEKERS_USERS",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "APP_JOB_SEEKERS_USER_ROLES",
                columns: table => new
                {
                    UserId = table.Column<string>(nullable: false),
                    RoleId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_APP_JOB_SEEKERS_USER_ROLES", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_APP_JOB_SEEKERS_USER_ROLES_APP_JOB_SEEKERS_ROLES_RoleId",
                        column: x => x.RoleId,
                        principalTable: "APP_JOB_SEEKERS_ROLES",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_APP_JOB_SEEKERS_USER_ROLES_APP_JOB_SEEKERS_USERS_UserId",
                        column: x => x.UserId,
                        principalTable: "APP_JOB_SEEKERS_USERS",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "APP_JOB_SEEKERS_USER_TOKENS",
                columns: table => new
                {
                    UserId = table.Column<string>(nullable: false),
                    LoginProvider = table.Column<string>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    Value = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_APP_JOB_SEEKERS_USER_TOKENS", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_APP_JOB_SEEKERS_USER_TOKENS_APP_JOB_SEEKERS_USERS_UserId",
                        column: x => x.UserId,
                        principalTable: "APP_JOB_SEEKERS_USERS",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_APP_JOB_SEEKERS_ROLE_CLAIMS_RoleId",
                table: "APP_JOB_SEEKERS_ROLE_CLAIMS",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "APP_JOB_SEEKERS_ROLES",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_APP_JOB_SEEKERS_USER_CLAIMS_UserId",
                table: "APP_JOB_SEEKERS_USER_CLAIMS",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_APP_JOB_SEEKERS_USER_EXTERNAL_LOGIN_UserId",
                table: "APP_JOB_SEEKERS_USER_EXTERNAL_LOGIN",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_APP_JOB_SEEKERS_USER_ROLES_RoleId",
                table: "APP_JOB_SEEKERS_USER_ROLES",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "APP_JOB_SEEKERS_USERS",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "APP_JOB_SEEKERS_USERS",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "APP_JOB_SEEKERS_ROLE_CLAIMS");

            migrationBuilder.DropTable(
                name: "APP_JOB_SEEKERS_USER_CLAIMS");

            migrationBuilder.DropTable(
                name: "APP_JOB_SEEKERS_USER_EXTERNAL_LOGIN");

            migrationBuilder.DropTable(
                name: "APP_JOB_SEEKERS_USER_ROLES");

            migrationBuilder.DropTable(
                name: "APP_JOB_SEEKERS_USER_TOKENS");

            migrationBuilder.DropTable(
                name: "APP_JOB_SEEKERS_ROLES");

            migrationBuilder.DropTable(
                name: "APP_JOB_SEEKERS_USERS");
        }
    }
}
