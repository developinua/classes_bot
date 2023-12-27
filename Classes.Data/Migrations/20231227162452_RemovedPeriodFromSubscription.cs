using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Classes.Data.Migrations
{
    /// <inheritdoc />
    public partial class RemovedPeriodFromSubscription : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Period",
                table: "Subscription");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Period",
                table: "Subscription",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }
    }
}
