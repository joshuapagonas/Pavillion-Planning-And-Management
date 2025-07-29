using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace webapi.Migrations
{
    /// <inheritdoc />
    public partial class BaselineSync : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "event_id",
                table: "Registration");

            migrationBuilder.DropColumn(
                name: "requested_park",
                table: "Registration");

            migrationBuilder.DropColumn(
                name: "waitlist",
                table: "Registration");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "Users",
                newName: "user_id");

            migrationBuilder.AddColumn<string>(
                name: "email",
                table: "Users",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "password_hash",
                table: "Users",
                type: "nvarchar(60)",
                maxLength: 60,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "phone_number",
                table: "Users",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "registration_date",
                table: "Registration",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AlterColumn<string>(
                name: "geolocation",
                table: "Parks",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255);

            migrationBuilder.AlterColumn<string>(
                name: "event_description",
                table: "Events",
                type: "nvarchar(4000)",
                maxLength: 4000,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255);

            migrationBuilder.AddColumn<bool>(
                name: "is_public_event",
                table: "Events",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "num_of_attendees",
                table: "Events",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "park_id",
                table: "Events",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "registration_id",
                table: "Events",
                type: "int",
                maxLength: 255,
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "user_id",
                table: "Events",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "user_id1",
                table: "Events",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Registration_park_id",
                table: "Registration",
                column: "park_id");

            migrationBuilder.CreateIndex(
                name: "IX_Registration_user_id",
                table: "Registration",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_Events_park_id",
                table: "Events",
                column: "park_id");

            migrationBuilder.CreateIndex(
                name: "IX_Events_user_id1",
                table: "Events",
                column: "user_id1");

            migrationBuilder.AddForeignKey(
                name: "FK_Events_Parks_park_id",
                table: "Events",
                column: "park_id",
                principalTable: "Parks",
                principalColumn: "park_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Events_Users_user_id1",
                table: "Events",
                column: "user_id1",
                principalTable: "Users",
                principalColumn: "user_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Registration_Parks_park_id",
                table: "Registration",
                column: "park_id",
                principalTable: "Parks",
                principalColumn: "park_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Registration_Users_user_id",
                table: "Registration",
                column: "user_id",
                principalTable: "Users",
                principalColumn: "user_id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Events_Parks_park_id",
                table: "Events");

            migrationBuilder.DropForeignKey(
                name: "FK_Events_Users_user_id1",
                table: "Events");

            migrationBuilder.DropForeignKey(
                name: "FK_Registration_Parks_park_id",
                table: "Registration");

            migrationBuilder.DropForeignKey(
                name: "FK_Registration_Users_user_id",
                table: "Registration");

            migrationBuilder.DropIndex(
                name: "IX_Registration_park_id",
                table: "Registration");

            migrationBuilder.DropIndex(
                name: "IX_Registration_user_id",
                table: "Registration");

            migrationBuilder.DropIndex(
                name: "IX_Events_park_id",
                table: "Events");

            migrationBuilder.DropIndex(
                name: "IX_Events_user_id1",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "email",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "password_hash",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "phone_number",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "registration_date",
                table: "Registration");

            migrationBuilder.DropColumn(
                name: "is_public_event",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "num_of_attendees",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "park_id",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "registration_id",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "user_id",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "user_id1",
                table: "Events");

            migrationBuilder.RenameColumn(
                name: "user_id",
                table: "Users",
                newName: "UserId");

            migrationBuilder.AddColumn<int>(
                name: "event_id",
                table: "Registration",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "requested_park",
                table: "Registration",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "waitlist",
                table: "Registration",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<string>(
                name: "geolocation",
                table: "Parks",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "event_description",
                table: "Events",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(4000)",
                oldMaxLength: 4000);
        }
    }
}
