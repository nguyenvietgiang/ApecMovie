using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UserServices.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateRefesh2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "AccessToken",
                table: "RefreshTokens",
                newName: "RefeshToken");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "RefeshToken",
                table: "RefreshTokens",
                newName: "AccessToken");
        }
    }
}
