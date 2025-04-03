using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using UniMarket.Models;

namespace UniMarket.DataAccess
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        // 🔥 Thêm DbSet để chắc chắn ApplicationUser được ánh xạ vào database
        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public DbSet<TinDang> TinDangs { get; set; }
        public DbSet<DanhMuc> DanhMucs { get; set; }
        public DbSet<AnhTinDang> AnhTinDangs { get; set; }
        public DbSet<TinhThanh> TinhThanhs { get; set; }
        public DbSet<QuanHuyen> QuanHuyens { get; set; }
        public DbSet<DanhMucCha> DanhMucChas { get; set; } // ✅ Kiểm tra có DbSet<DanhMuc> không

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<DanhMuc>()
                .HasOne(d => d.DanhMucCha)
                .WithMany(d => d.DanhMucs)
                .HasForeignKey(d => d.MaDanhMucCha)
                .OnDelete(DeleteBehavior.Restrict); // Không xóa tự động các danh mục con khi danh mục cha bị xóa
        }

    }
}
