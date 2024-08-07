using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Onyx.Migrations
{
    /// <inheritdoc />
    public partial class CreatingDefaultUsers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "98084f71-61ed-4aec-a6d6-09b99263866f", "98084f71-61ed-4aec-a6d6-09b99263866f", "Writer", "WRITER" },
                    { "a9d874e2-a943-4ce9-a921-49a4f0a6da1d", "a9d874e2-a943-4ce9-a921-49a4f0a6da1d", "Reader", "READER" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "98084f71-61ed-4aec-a6d6-09b99263866f");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "a9d874e2-a943-4ce9-a921-49a4f0a6da1d");
        }
    }
}
