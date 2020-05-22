using Microsoft.EntityFrameworkCore.Migrations;

namespace PictureWhisper.Domain.Migrations
{
    public partial class UpdateWStoryLength : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "W_Story",
                table: "T_Wallpaper",
                maxLength: 3072,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(1024)",
                oldMaxLength: 1024);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "W_Story",
                table: "T_Wallpaper",
                type: "nvarchar(1024)",
                maxLength: 1024,
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 3072);
        }
    }
}
