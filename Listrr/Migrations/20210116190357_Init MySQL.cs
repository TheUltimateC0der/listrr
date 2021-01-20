using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Listrr.Migrations
{
    public partial class InitMySQL : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<string>(type: "varchar(255) CHARACTER SET utf8mb4", nullable: false),
                    Name = table.Column<string>(type: "varchar(256) CHARACTER SET utf8mb4", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "varchar(256) CHARACTER SET utf8mb4", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<string>(type: "varchar(255) CHARACTER SET utf8mb4", nullable: false),
                    Level = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    UserName = table.Column<string>(type: "varchar(256) CHARACTER SET utf8mb4", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "varchar(256) CHARACTER SET utf8mb4", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "varchar(256) CHARACTER SET utf8mb4", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "varchar(256) CHARACTER SET utf8mb4", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    PasswordHash = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: true),
                    SecurityStamp = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: true),
                    PhoneNumber = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CountryCodes",
                columns: table => new
                {
                    Name = table.Column<string>(type: "varchar(255) CHARACTER SET utf8mb4", nullable: false),
                    Code = table.Column<string>(type: "varchar(255) CHARACTER SET utf8mb4", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CountryCodes", x => new { x.Name, x.Code });
                });

            migrationBuilder.CreateTable(
                name: "ImDbRatings",
                columns: table => new
                {
                    IMDbId = table.Column<string>(type: "varchar(255) CHARACTER SET utf8mb4", nullable: false),
                    Rating = table.Column<int>(type: "int", nullable: false),
                    Votes = table.Column<int>(type: "int", nullable: false),
                    Created = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    Updated = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ImDbRatings", x => x.IMDbId);
                });

            migrationBuilder.CreateTable(
                name: "LanguageCodes",
                columns: table => new
                {
                    Name = table.Column<string>(type: "varchar(255) CHARACTER SET utf8mb4", nullable: false),
                    Code = table.Column<string>(type: "varchar(255) CHARACTER SET utf8mb4", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LanguageCodes", x => new { x.Name, x.Code });
                });

            migrationBuilder.CreateTable(
                name: "TraktMovieCertifications",
                columns: table => new
                {
                    Slug = table.Column<string>(type: "varchar(255) CHARACTER SET utf8mb4", nullable: false),
                    Description = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: true),
                    Name = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TraktMovieCertifications", x => x.Slug);
                });

            migrationBuilder.CreateTable(
                name: "TraktMovieGenres",
                columns: table => new
                {
                    Slug = table.Column<string>(type: "varchar(255) CHARACTER SET utf8mb4", nullable: false),
                    Name = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TraktMovieGenres", x => x.Slug);
                });

            migrationBuilder.CreateTable(
                name: "TraktShowCertifications",
                columns: table => new
                {
                    Slug = table.Column<string>(type: "varchar(255) CHARACTER SET utf8mb4", nullable: false),
                    Description = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: true),
                    Name = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TraktShowCertifications", x => x.Slug);
                });

            migrationBuilder.CreateTable(
                name: "TraktShowGenres",
                columns: table => new
                {
                    Slug = table.Column<string>(type: "varchar(255) CHARACTER SET utf8mb4", nullable: false),
                    Name = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TraktShowGenres", x => x.Slug);
                });

            migrationBuilder.CreateTable(
                name: "TraktShowNetworks",
                columns: table => new
                {
                    Id = table.Column<string>(type: "varchar(255) CHARACTER SET utf8mb4", nullable: false),
                    Name = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TraktShowNetworks", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TraktShowStatuses",
                columns: table => new
                {
                    Name = table.Column<string>(type: "varchar(255) CHARACTER SET utf8mb4", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TraktShowStatuses", x => x.Name);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    RoleId = table.Column<string>(type: "varchar(255) CHARACTER SET utf8mb4", nullable: false),
                    ClaimType = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: true),
                    ClaimValue = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    UserId = table.Column<string>(type: "varchar(255) CHARACTER SET utf8mb4", nullable: false),
                    ClaimType = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: true),
                    ClaimValue = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "varchar(128) CHARACTER SET utf8mb4", maxLength: 128, nullable: false),
                    ProviderKey = table.Column<string>(type: "varchar(128) CHARACTER SET utf8mb4", maxLength: 128, nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: true),
                    UserId = table.Column<string>(type: "varchar(255) CHARACTER SET utf8mb4", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "varchar(255) CHARACTER SET utf8mb4", nullable: false),
                    RoleId = table.Column<string>(type: "varchar(255) CHARACTER SET utf8mb4", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "varchar(255) CHARACTER SET utf8mb4", nullable: false),
                    LoginProvider = table.Column<string>(type: "varchar(128) CHARACTER SET utf8mb4", maxLength: 128, nullable: false),
                    Name = table.Column<string>(type: "varchar(128) CHARACTER SET utf8mb4", maxLength: 128, nullable: false),
                    Value = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TraktLists",
                columns: table => new
                {
                    Id = table.Column<uint>(type: "int unsigned", nullable: false),
                    Name = table.Column<string>(type: "varchar(255) CHARACTER SET utf8mb4", nullable: true),
                    Type = table.Column<int>(type: "int", nullable: false),
                    ContentType = table.Column<int>(type: "int", nullable: false),
                    Slug = table.Column<string>(type: "varchar(255) CHARACTER SET utf8mb4", nullable: true),
                    ScanState = table.Column<int>(type: "int", nullable: false, defaultValue: 2),
                    Process = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    Items = table.Column<int>(type: "int", nullable: true),
                    Likes = table.Column<int>(type: "int", nullable: true),
                    ItemList = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: true),
                    LastProcessed = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    OwnerId = table.Column<string>(type: "varchar(255) CHARACTER SET utf8mb4", nullable: true),
                    Query = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: true),
                    SearchByAlias = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    SearchByBiography = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    SearchByDescription = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    SearchByName = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    SearchByOverview = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    SearchByPeople = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    SearchByTitle = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    SearchByTranslations = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    SearchByTagline = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    Filter_Years = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: true),
                    Filter_Runtimes = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: true),
                    Filter_Ratings_Trakt = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: true),
                    Filter_Ratings_IMDb = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: true),
                    Filter_Languages = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: true),
                    Filter_Genres = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: true),
                    Filter_Countries = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: true),
                    Filter_Translations = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: true),
                    ExclusionFilter_Languages = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: true),
                    ExclusionFilter_Genres = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: true),
                    ExclusionFilter_Countries = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: true),
                    ExclusionFilter_Translations = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: true),
                    ExclusionFilter_Keywords = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: true),
                    Filter_Certifications_Movie = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: true),
                    ExclusionFilter_Certifications_Movie = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: true),
                    Filter_Certifications_Show = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: true),
                    Filter_Networks = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: true),
                    Filter_Status = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: true),
                    ExclusionFilter_Certifications_Show = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: true),
                    ExclusionFilter_Networks = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: true),
                    ExclusionFilter_Status = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TraktLists", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TraktLists_AspNetUsers_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CountryCodes_Code",
                table: "CountryCodes",
                column: "Code");

            migrationBuilder.CreateIndex(
                name: "IX_ImDbRatings_IMDbId",
                table: "ImDbRatings",
                column: "IMDbId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LanguageCodes_Code",
                table: "LanguageCodes",
                column: "Code");

            migrationBuilder.CreateIndex(
                name: "IX_TraktLists_OwnerId",
                table: "TraktLists",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_TraktLists_Slug_Name",
                table: "TraktLists",
                columns: new[] { "Slug", "Name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TraktMovieCertifications_Slug",
                table: "TraktMovieCertifications",
                column: "Slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TraktMovieGenres_Slug",
                table: "TraktMovieGenres",
                column: "Slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TraktShowCertifications_Slug",
                table: "TraktShowCertifications",
                column: "Slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TraktShowGenres_Slug",
                table: "TraktShowGenres",
                column: "Slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TraktShowStatuses_Name",
                table: "TraktShowStatuses",
                column: "Name",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "CountryCodes");

            migrationBuilder.DropTable(
                name: "ImDbRatings");

            migrationBuilder.DropTable(
                name: "LanguageCodes");

            migrationBuilder.DropTable(
                name: "TraktLists");

            migrationBuilder.DropTable(
                name: "TraktMovieCertifications");

            migrationBuilder.DropTable(
                name: "TraktMovieGenres");

            migrationBuilder.DropTable(
                name: "TraktShowCertifications");

            migrationBuilder.DropTable(
                name: "TraktShowGenres");

            migrationBuilder.DropTable(
                name: "TraktShowNetworks");

            migrationBuilder.DropTable(
                name: "TraktShowStatuses");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "AspNetUsers");
        }
    }
}
