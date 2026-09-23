using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Portal.Migrations
{
    /// <inheritdoc />
    public partial class AddProjectIdentityAssignments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ProjectManagerUserId",
                table: "Projects",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ProjectEngineerAssignments",
                columns: table => new
                {
                    ProjectId = table.Column<int>(type: "int", nullable: false),
                    EngineerUserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectEngineerAssignments", x => new { x.ProjectId, x.EngineerUserId });
                    table.ForeignKey(
                        name: "FK_ProjectEngineerAssignments_AspNetUsers_EngineerUserId",
                        column: x => x.EngineerUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProjectEngineerAssignments_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Projects_ProjectManagerUserId",
                table: "Projects",
                column: "ProjectManagerUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectEngineerAssignments_EngineerUserId",
                table: "ProjectEngineerAssignments",
                column: "EngineerUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Projects_AspNetUsers_ProjectManagerUserId",
                table: "Projects",
                column: "ProjectManagerUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Projects_AspNetUsers_ProjectManagerUserId",
                table: "Projects");

            migrationBuilder.DropTable(
                name: "ProjectEngineerAssignments");

            migrationBuilder.DropIndex(
                name: "IX_Projects_ProjectManagerUserId",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "ProjectManagerUserId",
                table: "Projects");
        }
    }
}
