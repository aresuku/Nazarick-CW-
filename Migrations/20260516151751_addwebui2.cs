using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HairSalon.Migrations
{
    /// <inheritdoc />
    public partial class addwebui2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Receptions_Masters_MasterId",
                table: "Receptions");

            migrationBuilder.DropForeignKey(
                name: "FK_Receptions_Services_ServiceId",
                table: "Receptions");

            migrationBuilder.DropForeignKey(
                name: "FK_Receptions_Users_ClientId",
                table: "Receptions");

            migrationBuilder.DropIndex(
                name: "IX_Masters_UserId",
                table: "Masters");

            migrationBuilder.RenameIndex(
                name: "IX_Receptions_MasterId_Время записи",
                table: "Receptions",
                newName: "IX_Receptions_MasterId_Time");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_Login",
                table: "Users",
                column: "Логин",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Services_Name",
                table: "Services",
                column: "Название",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Masters_Email",
                table: "Masters",
                column: "Адрес почты",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Masters_UserId",
                table: "Masters",
                column: "UserId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Receptions_Masters_MasterId",
                table: "Receptions",
                column: "MasterId",
                principalTable: "Masters",
                principalColumn: "MasterId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Receptions_Services_ServiceId",
                table: "Receptions",
                column: "ServiceId",
                principalTable: "Services",
                principalColumn: "ServiceId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Receptions_Users_ClientId",
                table: "Receptions",
                column: "ClientId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Receptions_Masters_MasterId",
                table: "Receptions");

            migrationBuilder.DropForeignKey(
                name: "FK_Receptions_Services_ServiceId",
                table: "Receptions");

            migrationBuilder.DropForeignKey(
                name: "FK_Receptions_Users_ClientId",
                table: "Receptions");

            migrationBuilder.DropIndex(
                name: "IX_Users_Email",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_Login",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Services_Name",
                table: "Services");

            migrationBuilder.DropIndex(
                name: "IX_Masters_Email",
                table: "Masters");

            migrationBuilder.DropIndex(
                name: "IX_Masters_UserId",
                table: "Masters");

            migrationBuilder.RenameIndex(
                name: "IX_Receptions_MasterId_Time",
                table: "Receptions",
                newName: "IX_Receptions_MasterId_Время записи");

            migrationBuilder.CreateIndex(
                name: "IX_Masters_UserId",
                table: "Masters",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Receptions_Masters_MasterId",
                table: "Receptions",
                column: "MasterId",
                principalTable: "Masters",
                principalColumn: "MasterId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Receptions_Services_ServiceId",
                table: "Receptions",
                column: "ServiceId",
                principalTable: "Services",
                principalColumn: "ServiceId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Receptions_Users_ClientId",
                table: "Receptions",
                column: "ClientId",
                principalTable: "Users",
                principalColumn: "UserId");
        }
    }
}
