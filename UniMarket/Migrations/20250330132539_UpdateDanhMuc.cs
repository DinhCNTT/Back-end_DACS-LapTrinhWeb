using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UniMarket.Migrations
{
    /// <inheritdoc />
    public partial class UpdateDanhMuc : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DanhMucs_DanhMucs_MaDanhMucCha",
                table: "DanhMucs");

            migrationBuilder.AlterColumn<int>(
                name: "MaDanhMucCha",
                table: "DanhMucs",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "DanhMucCha",
                columns: table => new
                {
                    MaDanhMucCha = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenDanhMucCha = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DanhMucCha", x => x.MaDanhMucCha);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_DanhMucs_DanhMucCha_MaDanhMucCha",
                table: "DanhMucs",
                column: "MaDanhMucCha",
                principalTable: "DanhMucCha",
                principalColumn: "MaDanhMucCha",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DanhMucs_DanhMucCha_MaDanhMucCha",
                table: "DanhMucs");

            migrationBuilder.DropTable(
                name: "DanhMucCha");

            migrationBuilder.AlterColumn<int>(
                name: "MaDanhMucCha",
                table: "DanhMucs",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_DanhMucs_DanhMucs_MaDanhMucCha",
                table: "DanhMucs",
                column: "MaDanhMucCha",
                principalTable: "DanhMucs",
                principalColumn: "MaDanhMuc");
        }
    }
}
