using System.ComponentModel.DataAnnotations;
<<<<<<< HEAD
using System.Collections.Generic;
=======
using System.ComponentModel.DataAnnotations.Schema;
>>>>>>> 943974eeb10876c1b0694a7901d19e5ad515c6cb

namespace UniMarket.Models
{
    public class DanhMucCha
    {
        [Key]
        public int MaDanhMucCha { get; set; }

        [Required, StringLength(100)]
        [Display(Name = "Tên danh mục cha")]
        public string TenDanhMucCha { get; set; }

<<<<<<< HEAD
        [Display(Name = "Danh sách danh mục con")]
        public List<DanhMuc>? DanhMucs { get; set; } // ✅ Cho phép rỗng khi chưa có danh mục con

        [Display(Name = "Ảnh danh mục cha")]
        public string? AnhDanhMucCha { get; set; } // ✅ Lưu đường dẫn ảnh danh mục cha
        // Thêm thuộc tính Icon vào DanhMucCha
        [Display(Name = "Icon danh mục cha")]
        public string? Icon { get; set; } // Lưu đường dẫn icon

    }
}
=======
        [Display(Name = "Ảnh danh mục cha")]
        public string? AnhDanhMuc { get; set; } // Lưu đường dẫn ảnh

        [Display(Name = "Icon danh mục cha")]
        public string? Icon { get; set; } // Lưu đường dẫn iconAC

        [Display(Name = "Danh sách danh mục con")]
        public List<DanhMuc>? DanhMucs { get; set; } // Cho phép rỗng khi danh mục cha chưa có danh mục con
    }
}
>>>>>>> 943974eeb10876c1b0694a7901d19e5ad515c6cb
