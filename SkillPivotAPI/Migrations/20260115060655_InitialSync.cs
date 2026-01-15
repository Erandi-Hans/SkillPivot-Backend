using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SkillPivotAPI.Migrations
{
    /// <inheritdoc />
    public partial class InitialSync : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
           
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Firstname",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "IsVerified",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Lastname",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "OTPExpiry",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Verificationcode",
                table: "Users");
        }
    }
}
