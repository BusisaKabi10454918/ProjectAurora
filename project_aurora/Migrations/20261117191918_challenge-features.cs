using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace project_aurora.Migrations
{
    /// <inheritdoc />
    public partial class challengefeatures : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FileName",
                table: "ChallengeSubmissions",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "AllowedLanguages",
                table: "Challenges",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FileName",
                table: "ChallengeSubmissions");

            migrationBuilder.DropColumn(
                name: "AllowedLanguages",
                table: "Challenges");
        }
    }
}
