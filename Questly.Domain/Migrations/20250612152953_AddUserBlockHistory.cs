using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Questly.Domain.Migrations
{
    /// <inheritdoc />
    public partial class AddUserBlockHistory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BlockUserHistory",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    f_user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    b_block_status = table.Column<bool>(type: "boolean", nullable: false),
                    c_reason = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    d_modif_datetime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BlockUserHistory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BlockUserHistory_Users_f_user_id",
                        column: x => x.f_user_id,
                        principalTable: "Users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BlockUserHistory_f_user_id",
                table: "BlockUserHistory",
                column: "f_user_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BlockUserHistory");
        }
    }
}
