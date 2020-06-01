using Microsoft.EntityFrameworkCore.Migrations;

namespace Scraper.API.Migrations
{
    public partial class v1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Discipline_ScientificField_FieldId",
                table: "Discipline");

            migrationBuilder.DropForeignKey(
                name: "FK_Subjects_Discipline_DisciplineId",
                table: "Subjects");

            migrationBuilder.DropForeignKey(
                name: "FK_Version_Articles_ArticleId",
                table: "Version");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Version",
                table: "Version");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ScientificField",
                table: "ScientificField");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Discipline",
                table: "Discipline");

            migrationBuilder.RenameTable(
                name: "Version",
                newName: "Versions");

            migrationBuilder.RenameTable(
                name: "ScientificField",
                newName: "ScientificFields");

            migrationBuilder.RenameTable(
                name: "Discipline",
                newName: "Disciplines");

            migrationBuilder.RenameIndex(
                name: "IX_Version_ArticleId",
                table: "Versions",
                newName: "IX_Versions_ArticleId");

            migrationBuilder.RenameIndex(
                name: "IX_Discipline_FieldId",
                table: "Disciplines",
                newName: "IX_Disciplines_FieldId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Versions",
                table: "Versions",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ScientificFields",
                table: "ScientificFields",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Disciplines",
                table: "Disciplines",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Disciplines_ScientificFields_FieldId",
                table: "Disciplines",
                column: "FieldId",
                principalTable: "ScientificFields",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Subjects_Disciplines_DisciplineId",
                table: "Subjects",
                column: "DisciplineId",
                principalTable: "Disciplines",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Versions_Articles_ArticleId",
                table: "Versions",
                column: "ArticleId",
                principalTable: "Articles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Disciplines_ScientificFields_FieldId",
                table: "Disciplines");

            migrationBuilder.DropForeignKey(
                name: "FK_Subjects_Disciplines_DisciplineId",
                table: "Subjects");

            migrationBuilder.DropForeignKey(
                name: "FK_Versions_Articles_ArticleId",
                table: "Versions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Versions",
                table: "Versions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ScientificFields",
                table: "ScientificFields");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Disciplines",
                table: "Disciplines");

            migrationBuilder.RenameTable(
                name: "Versions",
                newName: "Version");

            migrationBuilder.RenameTable(
                name: "ScientificFields",
                newName: "ScientificField");

            migrationBuilder.RenameTable(
                name: "Disciplines",
                newName: "Discipline");

            migrationBuilder.RenameIndex(
                name: "IX_Versions_ArticleId",
                table: "Version",
                newName: "IX_Version_ArticleId");

            migrationBuilder.RenameIndex(
                name: "IX_Disciplines_FieldId",
                table: "Discipline",
                newName: "IX_Discipline_FieldId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Version",
                table: "Version",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ScientificField",
                table: "ScientificField",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Discipline",
                table: "Discipline",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Discipline_ScientificField_FieldId",
                table: "Discipline",
                column: "FieldId",
                principalTable: "ScientificField",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Subjects_Discipline_DisciplineId",
                table: "Subjects",
                column: "DisciplineId",
                principalTable: "Discipline",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Version_Articles_ArticleId",
                table: "Version",
                column: "ArticleId",
                principalTable: "Articles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
