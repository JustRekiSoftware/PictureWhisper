using Microsoft.EntityFrameworkCore.Migrations;

namespace PictureWhisper.Client.Domain.Migrations
{
    public partial class InitailCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "T_HistoryInfo",
                columns: table => new
                {
                    HI_ID = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    HI_WallpaperID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_T_HistoryInfo", x => x.HI_ID);
                });

            migrationBuilder.CreateTable(
                name: "T_RecommendInfo",
                columns: table => new
                {
                    RI_ID = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    RI_Num = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_T_RecommendInfo", x => x.RI_ID);
                });

            migrationBuilder.CreateTable(
                name: "T_SettingInfo",
                columns: table => new
                {
                    STI_ID = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    STI_AutoSetWallpaper = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_T_SettingInfo", x => x.STI_ID);
                });

            migrationBuilder.CreateTable(
                name: "T_SigninInfo",
                columns: table => new
                {
                    SI_ID = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    SI_UserID = table.Column<int>(nullable: false),
                    SI_Email = table.Column<string>(maxLength: 32, nullable: true),
                    SI_Password = table.Column<string>(maxLength: 64, nullable: true),
                    SI_Avatar = table.Column<string>(maxLength: 128, nullable: true),
                    SI_Type = table.Column<short>(nullable: false),
                    SI_Status = table.Column<short>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_T_SigninInfo", x => x.SI_ID);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "T_HistoryInfo");

            migrationBuilder.DropTable(
                name: "T_RecommendInfo");

            migrationBuilder.DropTable(
                name: "T_SettingInfo");

            migrationBuilder.DropTable(
                name: "T_SigninInfo");
        }
    }
}
