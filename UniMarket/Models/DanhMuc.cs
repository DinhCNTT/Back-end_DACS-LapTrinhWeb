using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace UniMarket.Models
{
    public class DanhMuc
    {
        [Key]
        public int MaDanhMuc { get; set; }

        [Required, StringLength(100)]
        [Display(Name = "Tên danh mục con")]
        public string TenDanhMuc { get; set; }

        [Required] // 🔥 Bắt buộc phải có danh mục cha
        [Display(Name = "Mã danh mục cha")]
        public int MaDanhMucCha { get; set; }

        [ForeignKey("MaDanhMucCha")]
        [Display(Name = "Danh mục cha")]
        public DanhMucCha DanhMucCha { get; set; } // 🔥 Không thể null

        [Display(Name = "Ảnh danh mục")]
        public string? AnhDanhMuc { get; set; } // Lưu đường dẫn ảnh

        [Display(Name = "Icon danh mục")]
        public string? Icon { get; set; } // Lưu đường dẫn icon
    }
}
