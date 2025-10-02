using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EazyMenu.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddReservationConfiguration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reservations_ApplicationUsers_CustomerId",
                table: "Reservations");

            migrationBuilder.DropForeignKey(
                name: "FK_Reservations_Restaurants_RestaurantId",
                table: "Reservations");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "Reservations",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "SpecialRequests",
                table: "Reservations",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ReservationNumber",
                table: "Reservations",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "CustomerPhone",
                table: "Reservations",
                type: "nvarchar(15)",
                maxLength: 15,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "CustomerName",
                table: "Reservations",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "CustomerEmail",
                table: "Reservations",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CancellationReason",
                table: "Reservations",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Reservations_ReservationDate",
                table: "Reservations",
                column: "ReservationDate");

            migrationBuilder.CreateIndex(
                name: "IX_Reservations_ReservationNumber",
                table: "Reservations",
                column: "ReservationNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Reservations_RestaurantId_ReservationDate_Status",
                table: "Reservations",
                columns: new[] { "RestaurantId", "ReservationDate", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_Reservations_Status",
                table: "Reservations",
                column: "Status");

            migrationBuilder.AddForeignKey(
                name: "FK_Reservations_ApplicationUsers_CustomerId",
                table: "Reservations",
                column: "CustomerId",
                principalTable: "ApplicationUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Reservations_Restaurants_RestaurantId",
                table: "Reservations",
                column: "RestaurantId",
                principalTable: "Restaurants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reservations_ApplicationUsers_CustomerId",
                table: "Reservations");

            migrationBuilder.DropForeignKey(
                name: "FK_Reservations_Restaurants_RestaurantId",
                table: "Reservations");

            migrationBuilder.DropIndex(
                name: "IX_Reservations_ReservationDate",
                table: "Reservations");

            migrationBuilder.DropIndex(
                name: "IX_Reservations_ReservationNumber",
                table: "Reservations");

            migrationBuilder.DropIndex(
                name: "IX_Reservations_RestaurantId_ReservationDate_Status",
                table: "Reservations");

            migrationBuilder.DropIndex(
                name: "IX_Reservations_Status",
                table: "Reservations");

            migrationBuilder.AlterColumn<int>(
                name: "Status",
                table: "Reservations",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20);

            migrationBuilder.AlterColumn<string>(
                name: "SpecialRequests",
                table: "Reservations",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ReservationNumber",
                table: "Reservations",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20);

            migrationBuilder.AlterColumn<string>(
                name: "CustomerPhone",
                table: "Reservations",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(15)",
                oldMaxLength: 15);

            migrationBuilder.AlterColumn<string>(
                name: "CustomerName",
                table: "Reservations",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "CustomerEmail",
                table: "Reservations",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CancellationReason",
                table: "Reservations",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Reservations_ApplicationUsers_CustomerId",
                table: "Reservations",
                column: "CustomerId",
                principalTable: "ApplicationUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Reservations_Restaurants_RestaurantId",
                table: "Reservations",
                column: "RestaurantId",
                principalTable: "Restaurants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
