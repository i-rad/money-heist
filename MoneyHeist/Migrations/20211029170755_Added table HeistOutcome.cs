using Microsoft.EntityFrameworkCore.Migrations;

namespace MoneyHeist.Migrations
{
    public partial class AddedtableHeistOutcome : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "HeistOutcome",
                columns: table => new
                {
                    HeistOutcomeId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    HeistId = table.Column<int>(nullable: false),
                    Outcome = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HeistOutcome", x => x.HeistOutcomeId);
                });

            migrationBuilder.InsertData(
                table: "HeistStatuses",
                columns: new[] { "StatusId", "Name" },
                values: new object[,]
                {
                    { 1, "PLANNING" },
                    { 2, "READY" },
                    { 3, "IN_PROGRESS" },
                    { 4, "FINISHED" }
                });

            migrationBuilder.InsertData(
                table: "MemberStatuses",
                columns: new[] { "StatusId", "Name" },
                values: new object[,]
                {
                    { 1, "AVAILABLE" },
                    { 2, "EXPIRED" },
                    { 3, "INCARCERATED" },
                    { 4, "RETIRED" }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HeistOutcome");

            migrationBuilder.DeleteData(
                table: "HeistStatuses",
                keyColumn: "StatusId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "HeistStatuses",
                keyColumn: "StatusId",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "HeistStatuses",
                keyColumn: "StatusId",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "HeistStatuses",
                keyColumn: "StatusId",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "MemberStatuses",
                keyColumn: "StatusId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "MemberStatuses",
                keyColumn: "StatusId",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "MemberStatuses",
                keyColumn: "StatusId",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "MemberStatuses",
                keyColumn: "StatusId",
                keyValue: 4);
        }
    }
}
