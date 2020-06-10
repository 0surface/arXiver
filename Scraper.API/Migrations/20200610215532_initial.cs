using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Scraper.API.Migrations
{
    public partial class initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Articles",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ArxivId = table.Column<string>(nullable: true),
                    HtmlLink = table.Column<string>(nullable: true),
                    PdfUrl = table.Column<string>(nullable: true),
                    OtherFormatUrl = table.Column<string>(nullable: true),
                    Title = table.Column<string>(nullable: true),
                    AbstractText = table.Column<string>(nullable: true),
                    Comments = table.Column<string>(nullable: true),
                    JournalReference = table.Column<string>(nullable: true),
                    JournalReferenceHtmlLink = table.Column<string>(nullable: true),
                    DisplayDate = table.Column<DateTime>(nullable: false),
                    ScrapedDate = table.Column<DateTime>(nullable: false),
                    ScrapeContext = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Articles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Authors",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Authors", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SubjectItems",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    IsPrimary = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubjectItems", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Subjects",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    GroupCode = table.Column<string>(nullable: true),
                    GroupName = table.Column<string>(nullable: true),
                    Discipline = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Subjects", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SubjectItemArticles",
                columns: table => new
                {
                    ArticleId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ArticleId1 = table.Column<int>(nullable: false),
                    SubjectItemId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubjectItemArticles", x => x.ArticleId);
                    table.ForeignKey(
                        name: "FK_SubjectItemArticles_Articles_ArticleId1",
                        column: x => x.ArticleId1,
                        principalTable: "Articles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Versions",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    VersionedId = table.Column<string>(nullable: true),
                    HtmlLink = table.Column<string>(nullable: true),
                    SubmissionDate = table.Column<DateTime>(nullable: false),
                    Tag = table.Column<string>(nullable: true),
                    CitationSubjectCode = table.Column<string>(nullable: true),
                    SizeInKiloBytes = table.Column<int>(nullable: false),
                    ArticleId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Versions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Versions_Articles_ArticleId",
                        column: x => x.ArticleId,
                        principalTable: "Articles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AuthorArticles",
                columns: table => new
                {
                    AuthorId = table.Column<int>(nullable: false),
                    ArticleId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuthorArticles", x => new { x.AuthorId, x.ArticleId });
                    table.ForeignKey(
                        name: "FK_AuthorArticles_Articles_ArticleId",
                        column: x => x.ArticleId,
                        principalTable: "Articles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AuthorArticles_Authors_AuthorId",
                        column: x => x.AuthorId,
                        principalTable: "Authors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AuthorArticles_ArticleId",
                table: "AuthorArticles",
                column: "ArticleId");

            migrationBuilder.CreateIndex(
                name: "IX_SubjectItemArticles_ArticleId1",
                table: "SubjectItemArticles",
                column: "ArticleId1");

            migrationBuilder.CreateIndex(
                name: "IX_Versions_ArticleId",
                table: "Versions",
                column: "ArticleId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AuthorArticles");

            migrationBuilder.DropTable(
                name: "SubjectItemArticles");

            migrationBuilder.DropTable(
                name: "SubjectItems");

            migrationBuilder.DropTable(
                name: "Subjects");

            migrationBuilder.DropTable(
                name: "Versions");

            migrationBuilder.DropTable(
                name: "Authors");

            migrationBuilder.DropTable(
                name: "Articles");
        }
    }
}
