using Microsoft.EntityFrameworkCore.Migrations;

namespace Scraper.API.Migrations
{
    public partial class newrealtions21 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AuthorArticles_Authors_AuthorId",
                table: "AuthorArticles");

            migrationBuilder.DropForeignKey(
                name: "FK_SubjectItem_Articles_ArticleId",
                table: "SubjectItem");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SubjectItem",
                table: "SubjectItem");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Authors",
                table: "Authors");

            migrationBuilder.RenameTable(
                name: "SubjectItem",
                newName: "SubjectItems");

            migrationBuilder.RenameTable(
                name: "Authors",
                newName: "Author");

            migrationBuilder.RenameIndex(
                name: "IX_SubjectItem_ArticleId",
                table: "SubjectItems",
                newName: "IX_SubjectItems_ArticleId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SubjectItems",
                table: "SubjectItems",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Author",
                table: "Author",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AuthorArticles_Author_AuthorId",
                table: "AuthorArticles",
                column: "AuthorId",
                principalTable: "Author",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SubjectItems_Articles_ArticleId",
                table: "SubjectItems",
                column: "ArticleId",
                principalTable: "Articles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AuthorArticles_Author_AuthorId",
                table: "AuthorArticles");

            migrationBuilder.DropForeignKey(
                name: "FK_SubjectItems_Articles_ArticleId",
                table: "SubjectItems");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SubjectItems",
                table: "SubjectItems");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Author",
                table: "Author");

            migrationBuilder.RenameTable(
                name: "SubjectItems",
                newName: "SubjectItem");

            migrationBuilder.RenameTable(
                name: "Author",
                newName: "Authors");

            migrationBuilder.RenameIndex(
                name: "IX_SubjectItems_ArticleId",
                table: "SubjectItem",
                newName: "IX_SubjectItem_ArticleId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SubjectItem",
                table: "SubjectItem",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Authors",
                table: "Authors",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AuthorArticles_Authors_AuthorId",
                table: "AuthorArticles",
                column: "AuthorId",
                principalTable: "Authors",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SubjectItem_Articles_ArticleId",
                table: "SubjectItem",
                column: "ArticleId",
                principalTable: "Articles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
