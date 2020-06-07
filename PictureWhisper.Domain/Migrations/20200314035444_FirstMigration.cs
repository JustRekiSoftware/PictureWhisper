using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace PictureWhisper.Domain.Migrations
{
    public partial class FirstMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "T_Comment",
                columns: table => new
                {
                    C_ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    C_PublisherID = table.Column<int>(nullable: false),
                    C_ReceiverID = table.Column<int>(nullable: false),
                    C_WallpaperID = table.Column<int>(nullable: false),
                    C_Content = table.Column<string>(maxLength: 256, nullable: false),
                    C_ReplyNum = table.Column<int>(nullable: false),
                    C_Date = table.Column<DateTime>(type: "datetime", nullable: false),
                    C_Status = table.Column<short>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_T_Comment", x => x.C_ID);
                });

            migrationBuilder.CreateTable(
                name: "T_Favorite",
                columns: table => new
                {
                    FVRT_ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FVRT_WallpaperID = table.Column<int>(nullable: false),
                    FVRT_FavoritorID = table.Column<int>(nullable: false),
                    FVRT_Date = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_T_Favorite", x => x.FVRT_ID);
                });

            migrationBuilder.CreateTable(
                name: "T_Follow",
                columns: table => new
                {
                    FLW_ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FLW_FollowerID = table.Column<int>(nullable: false),
                    FLW_FollowedID = table.Column<int>(nullable: false),
                    FLW_Date = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_T_Follow", x => x.FLW_ID);
                });

            migrationBuilder.CreateTable(
                name: "T_Like",
                columns: table => new
                {
                    L_ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    L_WallpaperID = table.Column<int>(nullable: false),
                    L_LikerID = table.Column<int>(nullable: false),
                    L_Date = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_T_Like", x => x.L_ID);
                });

            migrationBuilder.CreateTable(
                name: "T_Relpy",
                columns: table => new
                {
                    RPL_ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RPL_PublisherID = table.Column<int>(nullable: false),
                    RPL_ReceiverID = table.Column<int>(nullable: false),
                    RPL_CommentID = table.Column<int>(nullable: false),
                    RPL_Content = table.Column<string>(maxLength: 256, nullable: false),
                    RPL_Date = table.Column<DateTime>(type: "datetime", nullable: false),
                    RPL_Status = table.Column<short>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_T_Relpy", x => x.RPL_ID);
                });

            migrationBuilder.CreateTable(
                name: "T_Report",
                columns: table => new
                {
                    RPT_ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RPT_ReporterID = table.Column<int>(nullable: false),
                    RPT_Type = table.Column<short>(nullable: false),
                    RPT_ReportedID = table.Column<int>(nullable: false),
                    RPT_Reason = table.Column<short>(nullable: false),
                    RPT_Additional = table.Column<string>(maxLength: 256, nullable: false),
                    RPT_Date = table.Column<DateTime>(type: "datetime", nullable: false),
                    RPT_Status = table.Column<short>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_T_Report", x => x.RPT_ID);
                });

            migrationBuilder.CreateTable(
                name: "T_ReportReason",
                columns: table => new
                {
                    RR_ID = table.Column<short>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RR_Info = table.Column<string>(maxLength: 16, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_T_ReportReason", x => x.RR_ID);
                });

            migrationBuilder.CreateTable(
                name: "T_Review",
                columns: table => new
                {
                    RV_ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RV_ReviewerID = table.Column<int>(nullable: false),
                    RV_Type = table.Column<short>(nullable: false),
                    RV_ReviewedID = table.Column<int>(nullable: false),
                    RV_Result = table.Column<bool>(nullable: false),
                    RV_MsgToReporterID = table.Column<int>(nullable: false),
                    RV_MsgToReportedID = table.Column<int>(nullable: false),
                    RV_Date = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_T_Review", x => x.RV_ID);
                });

            migrationBuilder.CreateTable(
                name: "T_User",
                columns: table => new
                {
                    U_ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    U_Email = table.Column<string>(maxLength: 32, nullable: false),
                    U_Name = table.Column<string>(maxLength: 16, nullable: false),
                    U_Password = table.Column<string>(maxLength: 64, nullable: false),
                    U_Info = table.Column<string>(maxLength: 256, nullable: false),
                    U_Tag = table.Column<string>(maxLength: 128, nullable: false),
                    U_Avatar = table.Column<string>(maxLength: 128, nullable: false),
                    U_FollowedNum = table.Column<int>(nullable: false),
                    U_FollowerNum = table.Column<int>(nullable: false),
                    U_Type = table.Column<short>(nullable: false),
                    U_Status = table.Column<short>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_T_User", x => x.U_ID);
                });

            migrationBuilder.CreateTable(
                name: "T_Wallpaper",
                columns: table => new
                {
                    W_ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    W_PublisherID = table.Column<int>(nullable: false),
                    W_Location = table.Column<string>(maxLength: 128, nullable: false),
                    W_Title = table.Column<string>(maxLength: 128, nullable: false),
                    W_Story = table.Column<string>(maxLength: 1024, nullable: false),
                    W_Type = table.Column<short>(nullable: false),
                    W_Tag = table.Column<string>(maxLength: 128, nullable: false),
                    W_LikeNum = table.Column<int>(nullable: false),
                    W_FavoriteNum = table.Column<int>(nullable: false),
                    W_Date = table.Column<DateTime>(type: "datetime", nullable: false),
                    W_Status = table.Column<short>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_T_Wallpaper", x => x.W_ID);
                });

            migrationBuilder.CreateTable(
                name: "T_WallpaperType",
                columns: table => new
                {
                    WT_ID = table.Column<short>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    WT_Name = table.Column<string>(maxLength: 8, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_T_WallpaperType", x => x.WT_ID);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "T_Comment");

            migrationBuilder.DropTable(
                name: "T_Favorite");

            migrationBuilder.DropTable(
                name: "T_Follow");

            migrationBuilder.DropTable(
                name: "T_Like");

            migrationBuilder.DropTable(
                name: "T_Relpy");

            migrationBuilder.DropTable(
                name: "T_Report");

            migrationBuilder.DropTable(
                name: "T_ReportReason");

            migrationBuilder.DropTable(
                name: "T_Review");

            migrationBuilder.DropTable(
                name: "T_User");

            migrationBuilder.DropTable(
                name: "T_Wallpaper");

            migrationBuilder.DropTable(
                name: "T_WallpaperType");
        }
    }
}
