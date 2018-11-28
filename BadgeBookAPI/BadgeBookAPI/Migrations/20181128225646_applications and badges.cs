using Microsoft.EntityFrameworkCore.Migrations;

namespace BadgeBookAPI.Migrations
{
    public partial class applicationsandbadges : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "UID",
                table: "Profile",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ApplicationId",
                table: "Badge",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BadgeDescription",
                table: "Badge",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProfileId",
                table: "Badge",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Applications",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    ImageUrl = table.Column<string>(nullable: true),
                    AppUrl = table.Column<string>(nullable: true),
                    Approved = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Applications", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Profile_UID",
                table: "Profile",
                column: "UID",
                unique: true,
                filter: "[UID] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_Profile_UserData_UID",
                table: "Profile",
                column: "UID",
                principalTable: "UserData",
                principalColumn: "UID",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Profile_UserData_UID",
                table: "Profile");

            migrationBuilder.DropTable(
                name: "Applications");

            migrationBuilder.DropIndex(
                name: "IX_Profile_UID",
                table: "Profile");

            migrationBuilder.DropColumn(
                name: "ApplicationId",
                table: "Badge");

            migrationBuilder.DropColumn(
                name: "BadgeDescription",
                table: "Badge");

            migrationBuilder.DropColumn(
                name: "ProfileId",
                table: "Badge");

            migrationBuilder.AlterColumn<string>(
                name: "UID",
                table: "Profile",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);
        }
    }
}
