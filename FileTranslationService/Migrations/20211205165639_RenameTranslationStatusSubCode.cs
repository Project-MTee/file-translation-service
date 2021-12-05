using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tilde.MT.FileTranslationService.Migrations
{
    public partial class RenameTranslationStatusSubCode : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TranslationStatusSubCode",
                table: "Tasks",
                newName: "TranslationSubstatus");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TranslationSubstatus",
                table: "Tasks",
                newName: "TranslationStatusSubCode");
        }
    }
}
