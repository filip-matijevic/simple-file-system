using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace simple_file_system.API.Migrations
{
    /// <inheritdoc />
    public partial class UniqueConstraintForNameInParent : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Nodes_Name_ParentId_Type",
                table: "Nodes",
                columns: new[] { "Name", "ParentId", "Type" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Nodes_Name_ParentId_Type",
                table: "Nodes");
        }
    }
}
