using Microsoft.EntityFrameworkCore.Migrations;

namespace BadgeBookAPI.Migrations
{
    public partial class msg : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Messages",
                columns: table => new
                {
                    MessageID = table.Column<string>(nullable: false),
                    Msg = table.Column<string>(nullable: true),
                    SenderUID = table.Column<string>(nullable: true),
                    ReceiverUID = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Messages", x => x.MessageID);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Messages");
        }
    }
}
