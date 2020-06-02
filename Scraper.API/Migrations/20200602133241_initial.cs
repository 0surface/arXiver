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
                    ScrapedDate = table.Column<DateTime>(nullable: false)
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
                name: "ScientificFields",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScientificFields", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SubjectItem",
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
                    table.PrimaryKey("PK_SubjectItem", x => x.Id);
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
                    ArticleId = table.Column<int>(nullable: false),
                    Id = table.Column<int>(nullable: false)
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

            migrationBuilder.CreateTable(
                name: "Disciplines",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(nullable: true),
                    FieldId = table.Column<int>(nullable: true),
                    Code = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Disciplines", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Disciplines_ScientificFields_FieldId",
                        column: x => x.FieldId,
                        principalTable: "ScientificFields",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SubjectItemArticles",
                columns: table => new
                {
                    SubjectItemId = table.Column<int>(nullable: false),
                    ArticleId = table.Column<int>(nullable: false),
                    Id = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubjectItemArticles", x => new { x.SubjectItemId, x.ArticleId });
                    table.ForeignKey(
                        name: "FK_SubjectItemArticles_Articles_ArticleId",
                        column: x => x.ArticleId,
                        principalTable: "Articles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SubjectItemArticles_SubjectItem_SubjectItemId",
                        column: x => x.SubjectItemId,
                        principalTable: "SubjectItem",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Subjects",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    DisciplineId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Subjects", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Subjects_Disciplines_DisciplineId",
                        column: x => x.DisciplineId,
                        principalTable: "Disciplines",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AuthorArticles_ArticleId",
                table: "AuthorArticles",
                column: "ArticleId");

            migrationBuilder.CreateIndex(
                name: "IX_Disciplines_FieldId",
                table: "Disciplines",
                column: "FieldId");

            migrationBuilder.CreateIndex(
                name: "IX_SubjectItemArticles_ArticleId",
                table: "SubjectItemArticles",
                column: "ArticleId");

            migrationBuilder.CreateIndex(
                name: "IX_Subjects_DisciplineId",
                table: "Subjects",
                column: "DisciplineId");

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
                name: "Subjects");

            migrationBuilder.DropTable(
                name: "Versions");

            migrationBuilder.DropTable(
                name: "Authors");

            migrationBuilder.DropTable(
                name: "SubjectItem");

            migrationBuilder.DropTable(
                name: "Disciplines");

            migrationBuilder.DropTable(
                name: "Articles");

            migrationBuilder.DropTable(
                name: "ScientificFields");
        }
    }
}
