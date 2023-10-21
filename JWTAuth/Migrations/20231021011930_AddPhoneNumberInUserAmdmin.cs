using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WEBAPI.Migrations
{
    public partial class AddPhoneNumberInUserAmdmin : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_UserAdmins",
                table: "UserAdmins");

            migrationBuilder.AddColumn<string>(
                name: "UserAdminUserName",
                table: "UserWallet",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "UserName",
                table: "UserAdmins",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PhoneNumber",
                table: "UserAdmins",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserAdmins",
                table: "UserAdmins",
                column: "UserName");

            migrationBuilder.CreateIndex(
                name: "IX_UserWallet_UserAdminUserName",
                table: "UserWallet",
                column: "UserAdminUserName");

            migrationBuilder.AddForeignKey(
                name: "FK_UserWallet_UserAdmins_UserAdminUserName",
                table: "UserWallet",
                column: "UserAdminUserName",
                principalTable: "UserAdmins",
                principalColumn: "UserName");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserWallet_UserAdmins_UserAdminUserName",
                table: "UserWallet");

            migrationBuilder.DropIndex(
                name: "IX_UserWallet_UserAdminUserName",
                table: "UserWallet");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserAdmins",
                table: "UserAdmins");

            migrationBuilder.DropColumn(
                name: "UserAdminUserName",
                table: "UserWallet");

            migrationBuilder.DropColumn(
                name: "PhoneNumber",
                table: "UserAdmins");

            migrationBuilder.AlterColumn<string>(
                name: "UserName",
                table: "UserAdmins",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserAdmins",
                table: "UserAdmins",
                column: "Id");
        }
    }
}
