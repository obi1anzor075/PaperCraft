using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace PaperCraft.Migrations
{
    /// <inheritdoc />
    public partial class AddPhotos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 15);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 16);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 17);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 18);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 19);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 20);

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 1,
                column: "ImageUrl",
                value: "https://s3.ibta.ru/goods/141180/42e90c64e373e4289de2aee5972c951a_l.jpg");

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 2,
                column: "ImageUrl",
                value: "https://s3.ibta.ru/goods/402797/6231df38054d848d9842cb5c48cee431_l.jpg");

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 3,
                column: "ImageUrl",
                value: "https://s3.ibta.ru/goods/150491/3d1b800b86fca20ec4720ec41354bb62_l.jpg");

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 4,
                column: "ImageUrl",
                value: "https://s3.ibta.ru/goods/180286/6b21f01f60312703ea352df2bd608752_l.jpg");

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 5,
                column: "ImageUrl",
                value: "https://s3.ibta.ru/goods/111274/74e39a55982c7b2c92a94b0c5a711795_l.jpg");

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 6,
                column: "ImageUrl",
                value: "https://s3.ibta.ru/goods/220012/6d2547ea2cde13a2d7dacff0ce8504e8_l.jpg");

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 7,
                column: "ImageUrl",
                value: "https://s3.ibta.ru/goods/225753/f50219f336419e2578c5ab08050257b4_l.jpg");

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 8,
                column: "ImageUrl",
                value: "https://s3.ibta.ru/goods/250462/697149d326a54f6e6ba1882d76ee3994_l.jpg");

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 9,
                column: "ImageUrl",
                value: "https://s3.ibta.ru/goods/181690/9df61fc824c0f8facb7d19251e6061e4_l.jpg");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 1,
                column: "ImageUrl",
                value: "/images/pen1.jpg");

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 2,
                column: "ImageUrl",
                value: "/images/notebook1.jpg");

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 3,
                column: "ImageUrl",
                value: "/images/markers.jpg");

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 4,
                column: "ImageUrl",
                value: "/images/mech-pencil.jpg");

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 5,
                column: "ImageUrl",
                value: "/images/notepad.jpg");

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 6,
                column: "ImageUrl",
                value: "/images/paperclips.jpg");

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 7,
                column: "ImageUrl",
                value: "/images/folder.jpg");

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 8,
                column: "ImageUrl",
                value: "/images/calculator.jpg");

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 9,
                column: "ImageUrl",
                value: "/images/color-pencils.jpg");

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "Id", "CategoryId", "CreatedAt", "Description", "ImageUrl", "IsActive", "Name", "Price" },
                values: new object[,]
                {
                    { 10, 9, new DateTime(2025, 5, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Не оставляет следов", "/images/eraser.jpg", true, "Ластик", 29.00m },
                    { 11, 10, new DateTime(2025, 5, 1, 0, 0, 0, 0, DateTimeKind.Utc), "15 г", "/images/glue-stick.jpg", true, "Клей-карандаш", 49.00m },
                    { 12, 6, new DateTime(2025, 5, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Удобные ручки, 21 см", "/images/scissors.jpg", true, "Ножницы офисные", 119.00m },
                    { 13, 6, new DateTime(2025, 5, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Набор разноцветных", "/images/sticky-tabs.jpg", true, "Стикеры-закладки", 39.99m },
                    { 14, 8, new DateTime(2025, 5, 1, 0, 0, 0, 0, DateTimeKind.Utc), "12 цветов, водные", "/images/markers2.jpg", true, "Фломастеры", 99.00m },
                    { 15, 1, new DateTime(2025, 5, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Простая и надёжная", "/images/pen2.jpg", true, "Ручка с синей пастой", 19.00m },
                    { 16, 6, new DateTime(2025, 5, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Пластиковая, прозрачная", "/images/ruler.jpg", true, "Линейка 30 см", 25.00m },
                    { 17, 11, new DateTime(2025, 5, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Комплект 10 шт., А5", "/images/covers.jpg", true, "Обложки для тетрадей", 59.00m },
                    { 18, 12, new DateTime(2025, 5, 1, 0, 0, 0, 0, DateTimeKind.Utc), "На молнии, текстиль", "/images/pencil-case.jpg", true, "Пенал", 249.00m },
                    { 19, 6, new DateTime(2025, 5, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Металлический, до 10 листов", "/images/hole-punch.jpg", true, "Дырокол", 179.00m },
                    { 20, 6, new DateTime(2025, 5, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Прозрачный, 18 мм", "/images/tape.jpg", true, "Скотч канцелярский", 33.00m }
                });
        }
    }
}
