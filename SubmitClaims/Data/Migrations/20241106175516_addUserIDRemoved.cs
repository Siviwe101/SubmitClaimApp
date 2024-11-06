using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SubmitClaims.Data.Migrations
{
    /// <inheritdoc />
    public partial class addUserIDRemoved : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserId",
                table: "LecturerClaims");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "LecturerClaims",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }
    }
}
