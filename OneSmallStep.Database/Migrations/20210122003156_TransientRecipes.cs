using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace OneSmallStep.Database.Migrations
{
    public partial class TransientRecipes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AccessCode",
                table: "Recipes",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ExpirationDate",
                table: "Recipes",
                type: "TEXT",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AccessCode",
                table: "Recipes");

            migrationBuilder.DropColumn(
                name: "ExpirationDate",
                table: "Recipes");
        }
    }
}
