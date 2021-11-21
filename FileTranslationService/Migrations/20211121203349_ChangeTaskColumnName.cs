using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tilde.MT.FileTranslationService.Migrations
{
    public partial class ChangeTaskColumnName : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Files_Tasks_FileTranslationMetadataId",
                table: "Files");

            migrationBuilder.RenameColumn(
                name: "FileTranslationMetadataId",
                table: "Files",
                newName: "TaskId");

            migrationBuilder.RenameIndex(
                name: "IX_Files_FileTranslationMetadataId",
                table: "Files",
                newName: "IX_Files_TaskId");

            migrationBuilder.AddForeignKey(
                name: "FK_Files_Tasks_TaskId",
                table: "Files",
                column: "TaskId",
                principalTable: "Tasks",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Files_Tasks_TaskId",
                table: "Files");

            migrationBuilder.RenameColumn(
                name: "TaskId",
                table: "Files",
                newName: "FileTranslationMetadataId");

            migrationBuilder.RenameIndex(
                name: "IX_Files_TaskId",
                table: "Files",
                newName: "IX_Files_FileTranslationMetadataId");

            migrationBuilder.AddForeignKey(
                name: "FK_Files_Tasks_FileTranslationMetadataId",
                table: "Files",
                column: "FileTranslationMetadataId",
                principalTable: "Tasks",
                principalColumn: "Id");
        }
    }
}
