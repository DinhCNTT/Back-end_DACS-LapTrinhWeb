using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UniMarket.Migrations
{
    /// <inheritdoc />
    public partial class UpdateDanhMucLan2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DanhMucs_DanhMucCha_MaDanhMucCha",
                table: "DanhMucs");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DanhMucCha",
                table: "DanhMucCha");

            migrationBuilder.RenameTable(
                name: "DanhMucCha",
                newName: "DanhMucChas");

            migrationBuilder.AddPrimaryKey(
                name: "PK_DanhMucChas",
                table: "DanhMucChas",
                column: "MaDanhMucCha");

            migrationBuilder.AddForeignKey(
                name: "FK_DanhMucs_DanhMucChas_MaDanhMucCha",
                table: "DanhMucs",
                column: "MaDanhMucCha",
                principalTable: "DanhMucChas",
                principalColumn: "MaDanhMucCha",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DanhMucs_DanhMucChas_MaDanhMucCha",
                table: "DanhMucs");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DanhMucChas",
                table: "DanhMucChas");

            migrationBuilder.RenameTable(
                name: "DanhMucChas",
                newName: "DanhMucCha");

            migrationBuilder.AddPrimaryKey(
                name: "PK_DanhMucCha",
                table: "DanhMucCha",
                column: "MaDanhMucCha");

            migrationBuilder.AddForeignKey(
                name: "FK_DanhMucs_DanhMucCha_MaDanhMucCha",
                table: "DanhMucs",
                column: "MaDanhMucCha",
                principalTable: "DanhMucCha",
                principalColumn: "MaDanhMucCha",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
