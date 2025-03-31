using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UniMarket.Migrations
{
    /// <inheritdoc />
    public partial class TestKoChanEF : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DanhMucs",
                columns: table => new
                {
                    MaDanhMuc = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenDanhMuc = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    AnhDanhMuc = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Icon = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    MaDanhMucCha = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DanhMucs", x => x.MaDanhMuc);
                    table.ForeignKey(
                        name: "FK_DanhMucs_DanhMucs_MaDanhMucCha",
                        column: x => x.MaDanhMucCha,
                        principalTable: "DanhMucs",
                        principalColumn: "MaDanhMuc");
                });

            migrationBuilder.CreateTable(
                name: "TinhThanhs",
                columns: table => new
                {
                    MaTinhThanh = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenTinhThanh = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TinhThanhs", x => x.MaTinhThanh);
                });

            migrationBuilder.CreateTable(
                name: "QuanHuyens",
                columns: table => new
                {
                    MaQuanHuyen = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenQuanHuyen = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    MaTinhThanh = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuanHuyens", x => x.MaQuanHuyen);
                    table.ForeignKey(
                        name: "FK_QuanHuyens_TinhThanhs_MaTinhThanh",
                        column: x => x.MaTinhThanh,
                        principalTable: "TinhThanhs",
                        principalColumn: "MaTinhThanh",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TinDangs",
                columns: table => new
                {
                    MaTinDang = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaNguoiBan = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    MaDanhMuc = table.Column<int>(type: "int", nullable: false),
                    TieuDe = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    MoTa = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Gia = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CoTheThoaThuan = table.Column<bool>(type: "bit", nullable: false),
                    TinhTrang = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DiaChi = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    MaTinhThanh = table.Column<int>(type: "int", nullable: true),
                    MaQuanHuyen = table.Column<int>(type: "int", nullable: true),
                    NgayDang = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NgayCapNhat = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TrangThai = table.Column<byte>(type: "tinyint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TinDangs", x => x.MaTinDang);
                    table.ForeignKey(
                        name: "FK_TinDangs_AspNetUsers_MaNguoiBan",
                        column: x => x.MaNguoiBan,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TinDangs_DanhMucs_MaDanhMuc",
                        column: x => x.MaDanhMuc,
                        principalTable: "DanhMucs",
                        principalColumn: "MaDanhMuc",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TinDangs_QuanHuyens_MaQuanHuyen",
                        column: x => x.MaQuanHuyen,
                        principalTable: "QuanHuyens",
                        principalColumn: "MaQuanHuyen");
                    table.ForeignKey(
                        name: "FK_TinDangs_TinhThanhs_MaTinhThanh",
                        column: x => x.MaTinhThanh,
                        principalTable: "TinhThanhs",
                        principalColumn: "MaTinhThanh");
                });

            migrationBuilder.CreateTable(
                name: "AnhTinDangs",
                columns: table => new
                {
                    MaAnh = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaTinDang = table.Column<int>(type: "int", nullable: false),
                    DuongDan = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AnhTinDangs", x => x.MaAnh);
                    table.ForeignKey(
                        name: "FK_AnhTinDangs_TinDangs_MaTinDang",
                        column: x => x.MaTinDang,
                        principalTable: "TinDangs",
                        principalColumn: "MaTinDang",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AnhTinDangs_MaTinDang",
                table: "AnhTinDangs",
                column: "MaTinDang");

            migrationBuilder.CreateIndex(
                name: "IX_DanhMucs_MaDanhMucCha",
                table: "DanhMucs",
                column: "MaDanhMucCha");

            migrationBuilder.CreateIndex(
                name: "IX_QuanHuyens_MaTinhThanh",
                table: "QuanHuyens",
                column: "MaTinhThanh");

            migrationBuilder.CreateIndex(
                name: "IX_TinDangs_MaDanhMuc",
                table: "TinDangs",
                column: "MaDanhMuc");

            migrationBuilder.CreateIndex(
                name: "IX_TinDangs_MaNguoiBan",
                table: "TinDangs",
                column: "MaNguoiBan");

            migrationBuilder.CreateIndex(
                name: "IX_TinDangs_MaQuanHuyen",
                table: "TinDangs",
                column: "MaQuanHuyen");

            migrationBuilder.CreateIndex(
                name: "IX_TinDangs_MaTinhThanh",
                table: "TinDangs",
                column: "MaTinhThanh");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AnhTinDangs");

            migrationBuilder.DropTable(
                name: "TinDangs");

            migrationBuilder.DropTable(
                name: "DanhMucs");

            migrationBuilder.DropTable(
                name: "QuanHuyens");

            migrationBuilder.DropTable(
                name: "TinhThanhs");
        }
    }
}
