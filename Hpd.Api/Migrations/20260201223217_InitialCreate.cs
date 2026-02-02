using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Hpd.Api.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_MetricEvent",
                table: "MetricEvent");

            migrationBuilder.RenameTable(
                name: "MetricEvent",
                newName: "MetricEvents");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MetricEvents",
                table: "MetricEvents",
                column: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_MetricEvents",
                table: "MetricEvents");

            migrationBuilder.RenameTable(
                name: "MetricEvents",
                newName: "MetricEvent");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MetricEvent",
                table: "MetricEvent",
                column: "Id");
        }
    }
}
