using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TicketServices.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Tickets",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    MovieID = table.Column<Guid>(type: "uuid", nullable: false),
                    SeatNumber = table.Column<int>(type: "integer", nullable: false),
                    ShowTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    PaymentStatus = table.Column<int>(type: "integer", nullable: false),
                    UserID = table.Column<Guid>(type: "uuid", nullable: false),
                    Status = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tickets", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Tickets");
        }
    }
}
