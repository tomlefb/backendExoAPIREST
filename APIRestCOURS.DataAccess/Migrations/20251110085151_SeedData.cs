using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace APIRestCOURS.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class SeedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "DateNaissance", "Email", "Nom", "PasswordHash", "Prenom" },
                values: new object[,]
                {
                    { 1, new DateTime(1990, 1, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), "jean.dupont@email.com", "Dupont", "oQnjaUetVt4dyhzEnw74rJrZp7GqDfQfs8TLc8H/Aeo=", "Jean" },
                    { 2, new DateTime(1985, 5, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), "marie.martin@email.com", "Martin", "oQnjaUetVt4dyhzEnw74rJrZp7GqDfQfs8TLc8H/Aeo=", "Marie" },
                    { 3, new DateTime(1995, 8, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), "pierre.durand@email.com", "Durand", "oQnjaUetVt4dyhzEnw74rJrZp7GqDfQfs8TLc8H/Aeo=", "Pierre" },
                    { 4, new DateTime(1988, 3, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), "sophie.bernard@email.com", "Bernard", "oQnjaUetVt4dyhzEnw74rJrZp7GqDfQfs8TLc8H/Aeo=", "Sophie" }
                });

            migrationBuilder.InsertData(
                table: "Accounts",
                columns: new[] { "Id", "Balance", "Iban", "OwnerId" },
                values: new object[,]
                {
                    { 1, 50000.00m, "FR7612345678901234567890123", 1 },
                    { 2, 25000.00m, "FR7612345678901234567890124", 1 },
                    { 3, 120000.00m, "FR7698765432109876543210987", 2 },
                    { 4, 85000.00m, "FR7611111111111111111111111", 3 },
                    { 5, 200000.00m, "FR7622222222222222222222222", 4 }
                });

            migrationBuilder.InsertData(
                table: "Transactions",
                columns: new[] { "Id", "AccountId", "Amount", "At", "Kind" },
                values: new object[,]
                {
                    { 1, 1, 10000.00m, new DateTime(2024, 10, 11, 10, 0, 0, 0, DateTimeKind.Utc), 0 },
                    { 2, 1, 2500.00m, new DateTime(2024, 10, 16, 14, 30, 0, 0, DateTimeKind.Utc), 1 },
                    { 3, 1, 5000.00m, new DateTime(2024, 10, 21, 9, 15, 0, 0, DateTimeKind.Utc), 0 },
                    { 4, 2, 15000.00m, new DateTime(2024, 10, 26, 11, 0, 0, 0, DateTimeKind.Utc), 0 },
                    { 5, 2, 3000.00m, new DateTime(2024, 10, 31, 16, 45, 0, 0, DateTimeKind.Utc), 1 },
                    { 6, 3, 50000.00m, new DateTime(2024, 10, 13, 10, 30, 0, 0, DateTimeKind.Utc), 0 },
                    { 7, 3, 8000.00m, new DateTime(2024, 10, 23, 13, 20, 0, 0, DateTimeKind.Utc), 1 },
                    { 8, 3, 12000.00m, new DateTime(2024, 11, 5, 15, 0, 0, 0, DateTimeKind.Utc), 1 },
                    { 9, 4, 30000.00m, new DateTime(2024, 10, 19, 12, 0, 0, 0, DateTimeKind.Utc), 0 },
                    { 10, 4, 5000.00m, new DateTime(2024, 10, 29, 17, 30, 0, 0, DateTimeKind.Utc), 1 },
                    { 11, 5, 100000.00m, new DateTime(2024, 10, 1, 9, 0, 0, 0, DateTimeKind.Utc), 0 },
                    { 12, 5, 25000.00m, new DateTime(2024, 11, 2, 14, 0, 0, 0, DateTimeKind.Utc), 1 },
                    { 13, 5, 15000.00m, new DateTime(2024, 11, 7, 11, 30, 0, 0, DateTimeKind.Utc), 0 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Transactions",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Transactions",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Transactions",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Transactions",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Transactions",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Transactions",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Transactions",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Transactions",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Transactions",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "Transactions",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "Transactions",
                keyColumn: "Id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "Transactions",
                keyColumn: "Id",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "Transactions",
                keyColumn: "Id",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "Accounts",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Accounts",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Accounts",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Accounts",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Accounts",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 4);
        }
    }
}
