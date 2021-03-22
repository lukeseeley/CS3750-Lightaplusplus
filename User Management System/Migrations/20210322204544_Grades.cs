using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Lightaplusplus.Migrations
{
    public partial class Grades : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Grades",
                columns: table => new
                {
                    AssignmentId = table.Column<int>(nullable: false),
                    StudentId = table.Column<int>(nullable: false),
                    GradeDateTime = table.Column<DateTime>(nullable: false),
                    GradeValue = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Grades", x => new { x.AssignmentId, x.StudentId });
                    table.ForeignKey(
                        name: "FK_Grades_Assignments_AssignmentId",
                        column: x => x.AssignmentId,
                        principalTable: "Assignments",
                        principalColumn: "AssignmentId",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_Grades_Users_StudentId",
                        column: x => x.StudentId,
                        principalTable: "Users",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Grades_StudentId",
                table: "Grades",
                column: "StudentId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Grades");
        }
    }
}
