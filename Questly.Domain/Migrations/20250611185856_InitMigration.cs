using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Questly.Domain.Migrations
{
    /// <inheritdoc />
    public partial class InitMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AchievementCategories",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    c_name = table.Column<string>(type: "text", nullable: false),
                    c_description = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AchievementCategories", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "Cities",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    c_name = table.Column<string>(type: "text", nullable: false),
                    c_description = table.Column<string>(type: "text", nullable: true),
                    c_lat = table.Column<double>(type: "double precision", nullable: true),
                    c_lng = table.Column<double>(type: "double precision", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cities", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "Partners",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    c_company_name = table.Column<string>(type: "text", nullable: false),
                    c_address = table.Column<string>(type: "text", nullable: false),
                    c_owner_email = table.Column<string>(type: "text", nullable: true),
                    c_contact_phone = table.Column<long>(type: "bigint", nullable: false),
                    c_commission_rate = table.Column<float>(type: "real", nullable: false),
                    c_is_active = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Partners", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "PlaceTypes",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    c_name = table.Column<string>(type: "text", nullable: false),
                    c_description = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlaceTypes", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "Achievements",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    c_title = table.Column<string>(type: "text", nullable: false),
                    c_description = table.Column<string>(type: "text", nullable: false),
                    c_goal = table.Column<int>(type: "integer", nullable: false, defaultValue: 1),
                    c_reward_score = table.Column<int>(type: "integer", nullable: false),
                    f_city = table.Column<Guid>(type: "uuid", nullable: true),
                    c_icon_url = table.Column<string>(type: "text", nullable: false),
                    c_lat = table.Column<float>(type: "real", nullable: true),
                    c_lng = table.Column<float>(type: "real", nullable: true),
                    f_category = table.Column<Guid>(type: "uuid", nullable: true),
                    c_is_partner = table.Column<bool>(type: "boolean", nullable: false),
                    c_created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Achievements", x => x.id);
                    table.ForeignKey(
                        name: "FK_Achievements_AchievementCategories_f_category",
                        column: x => x.f_category,
                        principalTable: "AchievementCategories",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_Achievements_Cities_f_city",
                        column: x => x.f_city,
                        principalTable: "Cities",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    c_username = table.Column<string>(type: "text", nullable: false),
                    c_email = table.Column<string>(type: "text", nullable: false),
                    c_password_hash = table.Column<string>(type: "text", nullable: false),
                    f_city_id = table.Column<Guid>(type: "uuid", nullable: false),
                    c_created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    c_avatar_url = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.id);
                    table.ForeignKey(
                        name: "FK_Users_Cities_f_city_id",
                        column: x => x.f_city_id,
                        principalTable: "Cities",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Places",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    c_name = table.Column<string>(type: "text", nullable: false),
                    c_description = table.Column<string>(type: "text", nullable: true),
                    c_lat = table.Column<float>(type: "real", nullable: true),
                    c_lng = table.Column<float>(type: "real", nullable: true),
                    f_city = table.Column<Guid>(type: "uuid", nullable: true),
                    f_type_id = table.Column<Guid>(type: "uuid", nullable: true),
                    c_is_partner = table.Column<bool>(type: "boolean", nullable: false),
                    f_achievement_id = table.Column<Guid>(type: "uuid", nullable: true),
                    f_partner_id = table.Column<Guid>(type: "uuid", nullable: true),
                    c_icon_url = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Places", x => x.id);
                    table.ForeignKey(
                        name: "FK_Places_Achievements_f_achievement_id",
                        column: x => x.f_achievement_id,
                        principalTable: "Achievements",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_Places_Cities_f_city",
                        column: x => x.f_city,
                        principalTable: "Cities",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_Places_Partners_f_partner_id",
                        column: x => x.f_partner_id,
                        principalTable: "Partners",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_Places_PlaceTypes_f_type_id",
                        column: x => x.f_type_id,
                        principalTable: "PlaceTypes",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "Authorizations",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    f_user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    c_google_token = table.Column<string>(type: "text", nullable: true),
                    c_auth_token = table.Column<string>(type: "text", nullable: false),
                    c_auth_token_hash = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Authorizations", x => x.id);
                    table.ForeignKey(
                        name: "FK_Authorizations_Users_f_user_id",
                        column: x => x.f_user_id,
                        principalTable: "Users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Leaderboard",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    f_user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    f_city = table.Column<Guid>(type: "uuid", nullable: true),
                    c_score = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Leaderboard", x => x.id);
                    table.ForeignKey(
                        name: "FK_Leaderboard_Cities_f_city",
                        column: x => x.f_city,
                        principalTable: "Cities",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_Leaderboard_Users_f_user_id",
                        column: x => x.f_user_id,
                        principalTable: "Users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserAchievements",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    f_user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    f_achievement_id = table.Column<Guid>(type: "uuid", nullable: false),
                    c_progress = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    b_is_completed = table.Column<bool>(type: "boolean", nullable: false),
                    c_earned_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserAchievements", x => x.id);
                    table.ForeignKey(
                        name: "FK_UserAchievements_Achievements_f_achievement_id",
                        column: x => x.f_achievement_id,
                        principalTable: "Achievements",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserAchievements_Users_f_user_id",
                        column: x => x.f_user_id,
                        principalTable: "Users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Achievements_f_category",
                table: "Achievements",
                column: "f_category");

            migrationBuilder.CreateIndex(
                name: "IX_Achievements_f_city",
                table: "Achievements",
                column: "f_city");

            migrationBuilder.CreateIndex(
                name: "IX_Authorizations_f_user_id",
                table: "Authorizations",
                column: "f_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_Leaderboard_f_city",
                table: "Leaderboard",
                column: "f_city");

            migrationBuilder.CreateIndex(
                name: "IX_Leaderboard_f_user_id",
                table: "Leaderboard",
                column: "f_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_Places_f_achievement_id",
                table: "Places",
                column: "f_achievement_id");

            migrationBuilder.CreateIndex(
                name: "IX_Places_f_city",
                table: "Places",
                column: "f_city");

            migrationBuilder.CreateIndex(
                name: "IX_Places_f_partner_id",
                table: "Places",
                column: "f_partner_id");

            migrationBuilder.CreateIndex(
                name: "IX_Places_f_type_id",
                table: "Places",
                column: "f_type_id");

            migrationBuilder.CreateIndex(
                name: "IX_UserAchievements_f_achievement_id",
                table: "UserAchievements",
                column: "f_achievement_id");

            migrationBuilder.CreateIndex(
                name: "IX_UserAchievements_f_user_id",
                table: "UserAchievements",
                column: "f_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_Users_c_email",
                table: "Users",
                column: "c_email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_c_username",
                table: "Users",
                column: "c_username",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_f_city_id",
                table: "Users",
                column: "f_city_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Authorizations");

            migrationBuilder.DropTable(
                name: "Leaderboard");

            migrationBuilder.DropTable(
                name: "Places");

            migrationBuilder.DropTable(
                name: "UserAchievements");

            migrationBuilder.DropTable(
                name: "Partners");

            migrationBuilder.DropTable(
                name: "PlaceTypes");

            migrationBuilder.DropTable(
                name: "Achievements");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "AchievementCategories");

            migrationBuilder.DropTable(
                name: "Cities");
        }
    }
}
