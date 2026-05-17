using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace HairSalon.Migrations
{
    /// <inheritdoc />
    public partial class updateSeeder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Receptions",
                keyColumn: "Id",
                keyValue: 1,
                column: "Время записи",
                value: new DateTime(2138, 12, 1, 20, 30, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.UpdateData(
                table: "Receptions",
                keyColumn: "Id",
                keyValue: 2,
                column: "Время записи",
                value: new DateTime(2138, 12, 2, 19, 30, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.UpdateData(
                table: "Receptions",
                keyColumn: "Id",
                keyValue: 3,
                column: "Время записи",
                value: new DateTime(2138, 12, 3, 12, 30, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 1,
                column: "Имя пользователя",
                value: "Momonga");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 2,
                columns: new[] { "Логин", "Имя пользователя" },
                values: new object[] { "Sorcerer King", "Ainz Ooal Gown" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 3,
                column: "Имя пользователя",
                value: "Gun");

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "UserId", "Email", "Активен", "Логин", "Пароль", "Роль", "Имя пользователя" },
                values: new object[,]
                {
                    { 4, "albedo@gmail.com", true, "albedo", "hash4", "User", "LvAnz" },
                    { 5, "Garuganchua@gmail.com", true, "Garuganchua", "hash5", "User", "Garuganchua" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 5);

            migrationBuilder.UpdateData(
                table: "Receptions",
                keyColumn: "Id",
                keyValue: 1,
                column: "Время записи",
                value: new DateTime(2138, 12, 1, 23, 59, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.UpdateData(
                table: "Receptions",
                keyColumn: "Id",
                keyValue: 2,
                column: "Время записи",
                value: new DateTime(2138, 12, 2, 23, 59, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.UpdateData(
                table: "Receptions",
                keyColumn: "Id",
                keyValue: 3,
                column: "Время записи",
                value: new DateTime(2138, 12, 3, 23, 59, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 1,
                column: "Имя пользователя",
                value: "Сатору Судзуки");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 2,
                columns: new[] { "Логин", "Имя пользователя" },
                values: new object[] { "Ainz", "Айнз ОалГоун" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 3,
                column: "Имя пользователя",
                value: "Таня Дёгурешафф");
        }
    }
}
