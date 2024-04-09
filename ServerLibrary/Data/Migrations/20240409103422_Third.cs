using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ServerLibrary.Data.Migrations
{
    /// <inheritdoc />
    public partial class Third : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ApplicationUserId",
                table: "UserAccounts",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "TransferConfirmDate",
                table: "Transactions",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.CreateIndex(
                name: "IX_UserAccounts_ApplicationUserId",
                table: "UserAccounts",
                column: "ApplicationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserAccounts_BankId",
                table: "UserAccounts",
                column: "BankId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserAccounts_ApplicationUsers_ApplicationUserId",
                table: "UserAccounts",
                column: "ApplicationUserId",
                principalTable: "ApplicationUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UserAccounts_Banks_BankId",
                table: "UserAccounts",
                column: "BankId",
                principalTable: "Banks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserAccounts_ApplicationUsers_ApplicationUserId",
                table: "UserAccounts");

            migrationBuilder.DropForeignKey(
                name: "FK_UserAccounts_Banks_BankId",
                table: "UserAccounts");

            migrationBuilder.DropIndex(
                name: "IX_UserAccounts_ApplicationUserId",
                table: "UserAccounts");

            migrationBuilder.DropIndex(
                name: "IX_UserAccounts_BankId",
                table: "UserAccounts");

            migrationBuilder.DropColumn(
                name: "ApplicationUserId",
                table: "UserAccounts");

            migrationBuilder.AlterColumn<DateTime>(
                name: "TransferConfirmDate",
                table: "Transactions",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);
        }
    }
}
