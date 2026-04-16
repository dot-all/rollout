using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Rollout.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddTargetingRulesJson : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TargetingRules",
                table: "FeatureFlags",
                type: "jsonb",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TargetingRules",
                table: "FeatureFlags");
        }
    }
}
