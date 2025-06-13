using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Questly.Domain.Migrations
{
    /// <inheritdoc />
    public partial class NewAuthorization : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Authorizations");

            migrationBuilder.AddColumn<string>(
                name: "c_salt",
                table: "Users",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "RefreshSessions",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    f_user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    c_refresh_token_hash = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: false),
                    c_user_agent = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    c_ip_address = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    d_created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    d_expires_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    d_revoked_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RefreshSessions", x => x.id);
                    table.ForeignKey(
                        name: "FK_RefreshSessions_Users_f_user_id",
                        column: x => x.f_user_id,
                        principalTable: "Users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RefreshSessions_c_refresh_token_hash",
                table: "RefreshSessions",
                column: "c_refresh_token_hash",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RefreshSessions_f_user_id",
                table: "RefreshSessions",
                column: "f_user_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RefreshSessions");

            migrationBuilder.DropColumn(
                name: "c_salt",
                table: "Users");

            migrationBuilder.CreateTable(
                name: "Authorizations",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    f_user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    c_auth_token = table.Column<string>(type: "character varying(640)", maxLength: 640, nullable: false),
                    c_auth_token_hash = table.Column<string>(type: "character varying(640)", maxLength: 640, nullable: false),
                    c_google_token = table.Column<string>(type: "character varying(1560)", maxLength: 1560, nullable: true)
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

            migrationBuilder.CreateIndex(
                name: "IX_Authorizations_f_user_id",
                table: "Authorizations",
                column: "f_user_id");
        }
    }
}
