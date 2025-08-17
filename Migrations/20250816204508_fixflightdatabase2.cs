using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FlightAPIs.Migrations
{
    /// <inheritdoc />
    public partial class fixflightdatabase2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_FlightSchedules_plane_id",
                table: "FlightSchedules",
                column: "plane_id");

            migrationBuilder.AddForeignKey(
                name: "FK_Plan_Id",
                table: "FlightSchedules",
                column: "plane_id",
                principalTable: "Plane",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Plan_Id",
                table: "FlightSchedules");

            migrationBuilder.DropIndex(
                name: "IX_FlightSchedules_plane_id",
                table: "FlightSchedules");
        }
    }
}
