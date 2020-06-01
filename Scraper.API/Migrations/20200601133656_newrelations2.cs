using Microsoft.EntityFrameworkCore.Migrations;

namespace Scraper.API.Migrations
{
    public partial class newrelations2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Articles_Subjects_PrimarySubjectId",
                table: "Articles");

            migrationBuilder.DropForeignKey(
                name: "FK_Subjects_Articles_ArticleId",
                table: "Subjects");

            migrationBuilder.DropIndex(
                name: "IX_Subjects_ArticleId",
                table: "Subjects");

            migrationBuilder.DropIndex(
                name: "IX_Articles_PrimarySubjectId",
                table: "Articles");

            migrationBuilder.DropColumn(
                name: "ArticleId",
                table: "Subjects");

            migrationBuilder.DropColumn(
                name: "PrimarySubjectId",
                table: "Articles");

            migrationBuilder.CreateTable(
                name: "SubjectItem",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    IsPrimary = table.Column<bool>(nullable: false),
                    ArticleId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubjectItem", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SubjectItem_Articles_ArticleId",
                        column: x => x.ArticleId,
                        principalTable: "Articles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SubjectItem_ArticleId",
                table: "SubjectItem",
                column: "ArticleId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SubjectItem");

            migrationBuilder.AddColumn<int>(
                name: "ArticleId",
                table: "Subjects",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PrimarySubjectId",
                table: "Articles",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Subjects_ArticleId",
                table: "Subjects",
                column: "ArticleId");

            migrationBuilder.CreateIndex(
                name: "IX_Articles_PrimarySubjectId",
                table: "Articles",
                column: "PrimarySubjectId");

            migrationBuilder.AddForeignKey(
                name: "FK_Articles_Subjects_PrimarySubjectId",
                table: "Articles",
                column: "PrimarySubjectId",
                principalTable: "Subjects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Subjects_Articles_ArticleId",
                table: "Subjects",
                column: "ArticleId",
                principalTable: "Articles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
