using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace UniMarket.Models
{
    public class TinhThanh
    {
        [Key]
        public int MaTinhThanh { get; set; }

        [Required(ErrorMessage = "Tên tỉnh/thành không được để trống.")]
        [StringLength(100, ErrorMessage = "Tên tỉnh/thành không được vượt quá 100 ký tự.")]
        [DisplayName("Tên tỉnh/thành phố")]
        public string TenTinhThanh { get; set; }

<<<<<<< HEAD

=======
>>>>>>> 943974eeb10876c1b0694a7901d19e5ad515c6cb
        public ICollection<QuanHuyen>? QuanHuyens { get; set; }
    }
}
