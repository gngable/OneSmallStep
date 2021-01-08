using Microsoft.EntityFrameworkCore.Migrations;

namespace OneSmallStep.Database.Migrations
{
    public partial class StepTimer : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "NextButtonText",
                table: "Steps",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TimerSeconds",
                table: "Steps",
                type: "INTEGER",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NextButtonText",
                table: "Steps");

            migrationBuilder.DropColumn(
                name: "TimerSeconds",
                table: "Steps");
        }
    }
}
