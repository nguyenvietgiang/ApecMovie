using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MovieServices.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class editImgName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ImageUrl",
                table: "Movies",
                newName: "Image");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Image",
                table: "Movies",
                newName: "ImageUrl");
        }
    }
}
