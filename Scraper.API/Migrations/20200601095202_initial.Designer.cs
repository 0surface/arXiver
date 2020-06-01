﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Scraper.API.Infrastructure;

namespace Scraper.API.Migrations
{
    [DbContext(typeof(ArticleContext))]
    [Migration("20200601095202_initial")]
    partial class initial
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.4")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Scraper.Domain.AggregatesModel.ArticleAggregate.Article", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("AbstractText")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ArxivId")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Comments")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("HtmlLink")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("JournalReference")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("JournalReferenceHtmlLink")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("PrimarySubjectId")
                        .HasColumnType("int");

                    b.Property<string>("Title")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("PrimarySubjectId");

                    b.ToTable("Articles");
                });

            modelBuilder.Entity("Scraper.Domain.AggregatesModel.ArticleAggregate.Author", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Code")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Authors");
                });

            modelBuilder.Entity("Scraper.Domain.AggregatesModel.ArticleAggregate.AuthorArticle", b =>
                {
                    b.Property<int>("AuthorId")
                        .HasColumnType("int");

                    b.Property<int>("ArticleId")
                        .HasColumnType("int");

                    b.Property<int>("Id")
                        .HasColumnType("int");

                    b.HasKey("AuthorId", "ArticleId");

                    b.HasIndex("ArticleId");

                    b.ToTable("AuthorArticles");
                });

            modelBuilder.Entity("Scraper.Domain.AggregatesModel.ArticleAggregate.Version", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int?>("ArticleId")
                        .HasColumnType("int");

                    b.Property<string>("CitationSubjectCode")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("HtmlLink")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("SizeInKiloBytes")
                        .HasColumnType("int");

                    b.Property<DateTime>("SubmissionDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("Tag")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("VersionedId")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("ArticleId");

                    b.ToTable("Version");
                });

            modelBuilder.Entity("Scraper.Domain.AggregatesModel.SubjectAggregate.Discipline", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Code")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("FieldId")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("FieldId");

                    b.ToTable("Discipline");
                });

            modelBuilder.Entity("Scraper.Domain.AggregatesModel.SubjectAggregate.ScientificField", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("ScientificField");
                });

            modelBuilder.Entity("Scraper.Domain.AggregatesModel.SubjectAggregate.Subject", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int?>("ArticleId")
                        .HasColumnType("int");

                    b.Property<string>("Code")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("DisciplineId")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("ArticleId");

                    b.HasIndex("DisciplineId");

                    b.ToTable("Subjects");
                });

            modelBuilder.Entity("Scraper.Domain.AggregatesModel.ArticleAggregate.Article", b =>
                {
                    b.HasOne("Scraper.Domain.AggregatesModel.SubjectAggregate.Subject", "PrimarySubject")
                        .WithMany()
                        .HasForeignKey("PrimarySubjectId");
                });

            modelBuilder.Entity("Scraper.Domain.AggregatesModel.ArticleAggregate.AuthorArticle", b =>
                {
                    b.HasOne("Scraper.Domain.AggregatesModel.ArticleAggregate.Article", null)
                        .WithMany("AuthorArticles")
                        .HasForeignKey("ArticleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Scraper.Domain.AggregatesModel.ArticleAggregate.Author", null)
                        .WithMany("AuthorArticles")
                        .HasForeignKey("AuthorId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Scraper.Domain.AggregatesModel.ArticleAggregate.Version", b =>
                {
                    b.HasOne("Scraper.Domain.AggregatesModel.ArticleAggregate.Article", null)
                        .WithMany("Versions")
                        .HasForeignKey("ArticleId");
                });

            modelBuilder.Entity("Scraper.Domain.AggregatesModel.SubjectAggregate.Discipline", b =>
                {
                    b.HasOne("Scraper.Domain.AggregatesModel.SubjectAggregate.ScientificField", "Field")
                        .WithMany()
                        .HasForeignKey("FieldId");
                });

            modelBuilder.Entity("Scraper.Domain.AggregatesModel.SubjectAggregate.Subject", b =>
                {
                    b.HasOne("Scraper.Domain.AggregatesModel.ArticleAggregate.Article", null)
                        .WithMany("SubjectItems")
                        .HasForeignKey("ArticleId");

                    b.HasOne("Scraper.Domain.AggregatesModel.SubjectAggregate.Discipline", "Discipline")
                        .WithMany()
                        .HasForeignKey("DisciplineId");
                });
#pragma warning restore 612, 618
        }
    }
}
