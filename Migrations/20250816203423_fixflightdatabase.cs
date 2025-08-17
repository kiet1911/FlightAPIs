using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FlightAPIs.Migrations
{
    /// <inheritdoc />
    public partial class fixflightdatabase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_FlightSchedules_to_airport",
                table: "FlightSchedules",
                column: "to_airport");

            migrationBuilder.AddForeignKey(
                name: "FK_AirPort_To",
                table: "FlightSchedules",
                column: "to_airport",
                principalTable: "AirPort",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AirPort_To",
                table: "FlightSchedules");

            migrationBuilder.DropIndex(
                name: "IX_FlightSchedules_to_airport",
                table: "FlightSchedules");
        }
    }
}
