using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UniMarket.Models
{
    public class DanhMuc
    {
        [Key]
        public int MaDanhMuc { get; set; }

        [Required, StringLength(100)]
        [Display(Name = "Tên danh mục con")]
        public string TenDanhMuc { get; set; }

<<<<<<< HEAD
        [Required] // 🔥 Bắt buộc phải có danh mục cha
=======
        [Required]
>>>>>>> 943974eeb10876c1b0694a7901d19e5ad515c6cb
        [Display(Name = "Mã danh mục cha")]
        public int MaDanhMucCha { get; set; }

        [ForeignKey("MaDanhMucCha")]
        [Display(Name = "Danh mục cha")]
<<<<<<< HEAD
        public DanhMucCha DanhMucCha { get; set; } // 🔥 Không thể null
    }
}
=======
        public DanhMucCha DanhMucCha { get; set; }
    }
}
>>>>>>> 943974eeb10876c1b0694a7901d19e5ad515c6cb
