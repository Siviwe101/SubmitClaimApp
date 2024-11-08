using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SubmitClaims.Data.Migrations
{
    /// <inheritdoc />
    public partial class addHR : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Lecturer",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    FullName = table.Column<string>(type: "TEXT", nullable: false),
                    Email = table.Column<string>(type: "TEXT", nullable: false),
                    PhoneNumber = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Lecturer", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LecturerClaims_LecturerId",
                table: "LecturerClaims",
                column: "LecturerId");

            migrationBuilder.AddForeignKey(
                name: "FK_LecturerClaims_Lecturer_LecturerId",
                table: "LecturerClaims",
                column: "LecturerId",
                principalTable: "Lecturer",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LecturerClaims_Lecturer_LecturerId",
                table: "LecturerClaims");

            migrationBuilder.DropTable(
                name: "Lecturer");

            migrationBuilder.DropIndex(
                name: "IX_LecturerClaims_LecturerId",
                table: "LecturerClaims");
        }
    }
}
