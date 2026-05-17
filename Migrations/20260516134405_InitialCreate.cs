using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace HairSalon.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ClientId",
                table: "Receptions",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "Masters",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Логин = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Пароль = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Имяпользователя = table.Column<string>(name: "Имя пользователя", type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Роль = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Активен = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.UserId);
                });

            migrationBuilder.UpdateData(
                table: "Masters",
                keyColumn: "MasterId",
                keyValue: 1,
                column: "UserId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "Masters",
                keyColumn: "MasterId",
                keyValue: 2,
                column: "UserId",
                value: 2);

            migrationBuilder.UpdateData(
                table: "Masters",
                keyColumn: "MasterId",
                keyValue: 3,
                column: "UserId",
                value: 3);

            migrationBuilder.UpdateData(
                table: "Receptions",
                keyColumn: "Id",
                keyValue: 1,
                column: "ClientId",
                value: null);

            migrationBuilder.UpdateData(
                table: "Receptions",
                keyColumn: "Id",
                keyValue: 2,
                column: "ClientId",
                value: null);

            migrationBuilder.UpdateData(
                table: "Receptions",
                keyColumn: "Id",
                keyValue: 3,
                column: "ClientId",
                value: null);

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "UserId", "Email", "Активен", "Логин", "Пароль", "Роль", "Имя пользователя" },
                values: new object[,]
                {
                    { 1, "YGGDRASIL@gmail.com", true, "Satoru", "hash1", "Master", "Сатору Судзуки" },
                    { 2, "Overlord@gmail.com", true, "Ainz", "hash2", "Master", "Айнз ОалГоун" },
                    { 3, "Degurechaff@gmail.com", true, "Tanya", "hash3", "Master", "Таня Дёгурешафф" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Receptions_ClientId",
                table: "Receptions",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_Masters_UserId",
                table: "Masters",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Masters_Users_UserId",
                table: "Masters",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Receptions_Users_ClientId",
                table: "Receptions",
                column: "ClientId",
                principalTable: "Users",
                principalColumn: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Masters_Users_UserId",
                table: "Masters");

            migrationBuilder.DropForeignKey(
                name: "FK_Receptions_Users_ClientId",
                table: "Receptions");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Receptions_ClientId",
                table: "Receptions");

            migrationBuilder.DropIndex(
                name: "IX_Masters_UserId",
                table: "Masters");

            migrationBuilder.DropColumn(
                name: "ClientId",
                table: "Receptions");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Masters");
        }
    }
}
