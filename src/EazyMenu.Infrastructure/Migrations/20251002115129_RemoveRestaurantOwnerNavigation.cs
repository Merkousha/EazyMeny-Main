using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EazyMenu.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemoveRestaurantOwnerNavigation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Restaurants_ApplicationUsers_OwnerId",
                table: "Restaurants");

            migrationBuilder.AddColumn<Guid>(
                name: "ApplicationUserId",
                table: "Restaurants",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Restaurants_ApplicationUserId",
                table: "Restaurants",
                column: "ApplicationUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Restaurants_ApplicationUsers_ApplicationUserId",
                table: "Restaurants",
                column: "ApplicationUserId",
                principalTable: "ApplicationUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Restaurants_AspNetUsers_OwnerId",
                table: "Restaurants",
                column: "OwnerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Restaurants_ApplicationUsers_ApplicationUserId",
                table: "Restaurants");

            migrationBuilder.DropForeignKey(
                name: "FK_Restaurants_AspNetUsers_OwnerId",
                table: "Restaurants");

            migrationBuilder.DropIndex(
                name: "IX_Restaurants_ApplicationUserId",
                table: "Restaurants");

            migrationBuilder.DropColumn(
                name: "ApplicationUserId",
                table: "Restaurants");

            migrationBuilder.AddForeignKey(
                name: "FK_Restaurants_ApplicationUsers_OwnerId",
                table: "Restaurants",
                column: "OwnerId",
                principalTable: "ApplicationUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
