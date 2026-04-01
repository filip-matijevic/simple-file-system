using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace simple_file_system.API.Migrations
{
    /// <inheritdoc />
    public partial class UniqueNamePerDirectory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Nodes_Name_ParentId_Type",
                table: "Nodes");

            migrationBuilder.DropIndex(
                name: "IX_Nodes_Name_Type",
                table: "Nodes");

            migrationBuilder.CreateIndex(
                name: "IX_Nodes_Name",
                table: "Nodes",
                column: "Name",
                unique: true,
                filter: "[ParentId] IS NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Nodes_Name_ParentId",
                table: "Nodes",
                columns: new[] { "Name", "ParentId" },
                unique: true,
                filter: "[ParentId] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Nodes_Name",
                table: "Nodes");

            migrationBuilder.DropIndex(
                name: "IX_Nodes_Name_ParentId",
                table: "Nodes");

            migrationBuilder.CreateIndex(
                name: "IX_Nodes_Name_ParentId_Type",
                table: "Nodes",
                columns: new[] { "Name", "ParentId", "Type" },
                unique: true,
                filter: "[ParentId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Nodes_Name_Type",
                table: "Nodes",
                columns: new[] { "Name", "Type" },
                unique: true,
                filter: "[ParentId] IS NULL");
        }
    }
}
