using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Western_Mutual_Api.Migrations
{
    /// <inheritdoc />
    public partial class MigrationName4 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "email",
                table: "Buyers",
                newName: "Email");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Email",
                table: "Buyers",
                newName: "email");
        }
    }
}
