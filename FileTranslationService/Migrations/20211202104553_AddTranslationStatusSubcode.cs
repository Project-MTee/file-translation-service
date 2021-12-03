using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tilde.MT.FileTranslationService.Migrations
{
    public partial class AddTranslationStatusSubcode : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Files_FileCategories_CategoryId",
                table: "Files");

            migrationBuilder.DropForeignKey(
                name: "FK_Tasks_TranslationStatuses_TranslationStatusId",
                table: "Tasks");

            migrationBuilder.DropTable(
                name: "FileCategories");

            migrationBuilder.DropTable(
                name: "TranslationStatuses");

            migrationBuilder.DropIndex(
                name: "IX_Tasks_TranslationStatusId",
                table: "Tasks");

            migrationBuilder.DropIndex(
                name: "IX_Files_CategoryId",
                table: "Files");

            migrationBuilder.DropColumn(
                name: "TranslationStatusId",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "CategoryId",
                table: "Files");

            migrationBuilder.AddColumn<int>(
                name: "TranslationStatus",
                table: "Tasks",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TranslationStatusSubCode",
                table: "Tasks",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Category",
                table: "Files",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TranslationStatus",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "TranslationStatusSubCode",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "Category",
                table: "Files");

            migrationBuilder.AddColumn<int>(
                name: "TranslationStatusId",
                table: "Tasks",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CategoryId",
                table: "Files",
                type: "int",
                nullable: true);

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
                name: "IX_Tasks_TranslationStatusId",
                table: "Tasks",
                column: "TranslationStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_Files_CategoryId",
                table: "Files",
                column: "CategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_Files_FileCategories_CategoryId",
                table: "Files",
                column: "CategoryId",
                principalTable: "FileCategories",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Tasks_TranslationStatuses_TranslationStatusId",
                table: "Tasks",
                column: "TranslationStatusId",
                principalTable: "TranslationStatuses",
                principalColumn: "Id");
        }
    }
}
