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

        [Required]
        [Display(Name = "Mã danh mục cha")]
        public int MaDanhMucCha { get; set; }

        [ForeignKey("MaDanhMucCha")]
        [Display(Name = "Danh mục cha")]
        public DanhMucCha DanhMucCha { get; set; }
    }
}