﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Tilde.MT.FileTranslationService.Models.Database;

#nullable disable

namespace Tilde.MT.FileTranslationService.Migrations
{
    [DbContext(typeof(TaskDbContext))]
    partial class TaskDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            modelBuilder.Entity("Tilde.MT.FileTranslationService.Models.Database.File", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)");

                    b.Property<int>("Category")
                        .HasColumnType("int");

                    b.Property<DateTime?>("DbCreatedAt")
                        .HasColumnType("datetime(6)");

                    b.Property<DateTime?>("DbUpdatedAt")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("Extension")
                        .HasColumnType("longtext");

                    b.Property<long>("Size")
                        .HasColumnType("bigint");

                    b.Property<Guid?>("TaskId")
                        .HasColumnType("char(36)");

                    b.HasKey("Id");

                    b.HasIndex("TaskId");

                    b.ToTable("Files");
                });

            modelBuilder.Entity("Tilde.MT.FileTranslationService.Models.Database.Task", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)");

                    b.Property<DateTime?>("DbCreatedAt")
                        .HasColumnType("datetime(6)");

                    b.Property<DateTime?>("DbUpdatedAt")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("Domain")
                        .HasMaxLength(200)
                        .HasColumnType("varchar(200)");

                    b.Property<string>("FileName")
                        .HasMaxLength(300)
                        .HasColumnType("varchar(300)");

                    b.Property<long>("Segments")
                        .HasColumnType("bigint");

                    b.Property<long>("SegmentsTranslated")
                        .HasColumnType("bigint");

                    b.Property<string>("SourceLanguage")
                        .HasMaxLength(2)
                        .HasColumnType("varchar(2)");

                    b.Property<string>("TargetLanguage")
                        .HasMaxLength(2)
                        .HasColumnType("varchar(2)");

                    b.Property<int>("TranslationStatus")
                        .HasColumnType("int");

                    b.Property<int>("TranslationStatusSubCode")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("Tasks");
                });

            modelBuilder.Entity("Tilde.MT.FileTranslationService.Models.Database.File", b =>
                {
                    b.HasOne("Tilde.MT.FileTranslationService.Models.Database.Task", "Task")
                        .WithMany("Files")
                        .HasForeignKey("TaskId");

                    b.Navigation("Task");
                });

            modelBuilder.Entity("Tilde.MT.FileTranslationService.Models.Database.Task", b =>
                {
                    b.Navigation("Files");
                });
#pragma warning restore 612, 618
        }
    }
}
