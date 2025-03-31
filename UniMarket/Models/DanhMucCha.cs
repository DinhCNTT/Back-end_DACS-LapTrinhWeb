using System.ComponentModel.DataAnnotations;

namespace UniMarket.Models
{
    public class DanhMucCha
    {
        [Key]
        public int MaDanhMucCha { get; set; }

        [Required, StringLength(100)]
        [Display(Name = "Tên danh mục cha")]
        public string TenDanhMucCha { get; set; }

        [Display(Name = "Danh sách danh mục con")]
        public List<DanhMuc>? DanhMucs { get; set; } // ✅ Cho phép rỗng khi danh mục cha chưa có danh mục con
    }
}
