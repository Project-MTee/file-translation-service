using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Tilde.MT.FileTranslationService.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "FileCategories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Description = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FileCategories", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "TranslationStatuses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Description = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TranslationStatuses", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Tasks",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    FileName = table.Column<string>(type: "varchar(300)", maxLength: 300, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    SourceLanguage = table.Column<string>(type: "varchar(2)", maxLength: 2, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    TargetLanguage = table.Column<string>(type: "varchar(2)", maxLength: 2, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Domain = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DbCreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    DbUpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    Segments = table.Column<long>(type: "bigint", nullable: false),
                    SegmentsTranslated = table.Column<long>(type: "bigint", nullable: false),
                    TranslationStatusId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tasks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Tasks_TranslationStatuses_TranslationStatusId",
                        column: x => x.TranslationStatusId,
                        principalTable: "TranslationStatuses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Files",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    Extension = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CategoryId = table.Column<int>(type: "int", nullable: true),
                    Size = table.Column<long>(type: "bigint", nullable: false),
                    DbCreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    DbUpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    FileTranslationMetadataId = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Files", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Files_FileCategories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "FileCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Files_Tasks_FileTranslationMetadataId",
                        column: x => x.FileTranslationMetadataId,
                        principalTable: "Tasks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.InsertData(
                table: "FileCategories",
                columns: new[] { "Id", "Description" },
                values: new object[,]
                {
                    { 1, "Source" },
                    { 2, "SourceConverted" },
                    { 3, "Translated" },
                    { 4, "TranslatedConverted" }
                });

            migrationBuilder.InsertData(
                table: "TranslationStatuses",
                columns: new[] { "Id", "Description" },
                values: new object[,]
                {
                    { 1, "Queuing" },
                    { 2, "Initializing" },
                    { 3, "Extracting" },
                    { 4, "Waiting" },
                    { 5, "Translating" },
                    { 6, "Saving" },
                    { 7, "Completed" },
                    { 8, "Error" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Files_CategoryId",
                table: "Files",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Files_FileTranslationMetadataId",
                table: "Files",
                column: "FileTranslationMetadataId");

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_TranslationStatusId",
                table: "Tasks",
                column: "TranslationStatusId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Files");

            migrationBuilder.DropTable(
                name: "FileCategories");

            migrationBuilder.DropTable(
                name: "Tasks");

            migrationBuilder.DropTable(
                name: "TranslationStatuses");
        }
    }
}
