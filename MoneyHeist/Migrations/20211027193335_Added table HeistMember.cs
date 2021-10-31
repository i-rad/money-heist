using Microsoft.EntityFrameworkCore.Migrations;

namespace MoneyHeist.Migrations
{
    public partial class AddedtableHeistMember : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "HeistMembers",
                columns: table => new
                {
                    HeistMemberId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    HeistId = table.Column<int>(nullable: false),
                    MemberId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HeistMembers", x => x.HeistMemberId);
                    table.ForeignKey(
                        name: "FK_HeistMembers_Heists_HeistId",
                        column: x => x.HeistId,
                        principalTable: "Heists",
                        principalColumn: "HeistId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_HeistMembers_Members_MemberId",
                        column: x => x.MemberId,
                        principalTable: "Members",
                        principalColumn: "MemberId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_HeistMembers_HeistId",
                table: "HeistMembers",
                column: "HeistId");

            migrationBuilder.CreateIndex(
                name: "IX_HeistMembers_MemberId",
                table: "HeistMembers",
                column: "MemberId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HeistMembers");
        }
    }
}
