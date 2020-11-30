using Microsoft.EntityFrameworkCore.Migrations;

namespace OutlookClone.Migrations
{
    public partial class groupAdd : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Groups",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GroupOwnerID = table.Column<int>(type: "int", nullable: false),
                    GroupName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Groups", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ContactModelGroupModel",
                columns: table => new
                {
                    GroupMembersId = table.Column<int>(type: "int", nullable: false),
                    GroupsId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContactModelGroupModel", x => new { x.GroupMembersId, x.GroupsId });
                    table.ForeignKey(
                        name: "FK_ContactModelGroupModel_Contacts_GroupMembersId",
                        column: x => x.GroupMembersId,
                        principalTable: "Contacts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ContactModelGroupModel_Groups_GroupsId",
                        column: x => x.GroupsId,
                        principalTable: "Groups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ContactModelGroupModel_GroupsId",
                table: "ContactModelGroupModel",
                column: "GroupsId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ContactModelGroupModel");

            migrationBuilder.DropTable(
                name: "Groups");
        }
    }
}
