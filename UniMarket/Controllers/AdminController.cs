using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UniMarket.Models;
using Microsoft.AspNetCore.Cors;
using System.Linq;
using System.Threading.Tasks;
using static AuthController;
using UniMarket.DataAccess;

namespace UniMarket.Controllers
{
    [Route("api/admin")]
    [ApiController]
    [EnableCors("_myAllowSpecificOrigins")] // Áp dụng CORS cho controller này
    public class AdminController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _context; // ✅ Định nghĩa biến _context

        public AdminController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, ApplicationDbContext context) // 🔥 Thêm ApplicationDbContext vào DI
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context; // ✅ Gán _context
        }


        [HttpGet("users")]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _userManager.Users.ToListAsync();
            var userList = new List<object>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                bool isLocked = user.LockoutEnd != null && user.LockoutEnd > DateTime.Now;

                userList.Add(new
                {
                    user.Id,
                    FullName = user.FullName ?? "Không có",
                    user.UserName,
                    user.Email,
                    user.PhoneNumber,
                    Role = roles.Any() ? string.Join(", ", roles) : "Chưa có",
                    isLocked = isLocked // Trả về trạng thái khóa
                });
            }

            return Ok(userList);
        }

        [HttpPost("add-employee")]
        public async Task<IActionResult> AddEmployee([FromBody] RegisterModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = new ApplicationUser
            {
                UserName = model.Email,
                Email = model.Email,
                EmailConfirmed = true,
                FullName = model.FullName,  // Thêm dòng này
                PhoneNumber = model.PhoneNumber  // Thêm dòng này
            };

            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
                return BadRequest(result.Errors);

            await _userManager.AddToRoleAsync(user, SD.Role_Employee);
            return Ok(new { message = "Nhân viên đã được thêm thành công!" });
        }

        [HttpDelete("delete-user/{id}")]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return NotFound("Không tìm thấy người dùng!");

            var isAdmin = await _userManager.IsInRoleAsync(user, SD.Role_Admin);
            if (isAdmin)
                return BadRequest("Không thể xóa tài khoản Admin!");

            var result = await _userManager.DeleteAsync(user);
            if (!result.Succeeded)
                return BadRequest(result.Errors);

            return Ok("Xóa người dùng thành công!");
        }

        [HttpPost("toggle-lock/{id}")]
        public async Task<IActionResult> ToggleUserLock(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return NotFound("Không tìm thấy người dùng!");

            bool isLocked = user.LockoutEnd != null && user.LockoutEnd > DateTime.Now;

            if (isLocked)
            {
                user.LockoutEnd = null;
            }
            else
            {
                user.LockoutEnd = DateTime.Now.AddYears(100);
            }

            await _userManager.UpdateAsync(user);
            await _userManager.UpdateSecurityStampAsync(user);

            return Ok(new
            {
                message = isLocked ? "Tài khoản đã được mở khóa!" : "Tài khoản đã bị khóa!",
                isLocked = !isLocked
            });
        }

        [HttpPost("change-role")]
        public async Task<IActionResult> ChangeUserRole([FromBody] ChangeRoleModel model)
        {
            var user = await _userManager.FindByIdAsync(model.UserId);
            if (user == null)
                return NotFound("Không tìm thấy người dùng!");

            var currentRoles = await _userManager.GetRolesAsync(user);
            await _userManager.RemoveFromRolesAsync(user, currentRoles);

            await _userManager.AddToRoleAsync(user, model.NewRole);

            return Ok(new { message = $"Vai trò của {user.Email} đã được thay đổi thành {model.NewRole}." });
        }

        /// <summary>
        /// ✅ API gộp thêm nhân viên mới hoặc cập nhật vai trò
        /// </summary>
        [HttpPost("add-or-update-employee")]
        public async Task<IActionResult> AddOrUpdateEmployee([FromBody] EmployeeRoleModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var roleExists = await _roleManager.RoleExistsAsync(model.Role);
            if (!roleExists)
                return BadRequest("Vai trò không hợp lệ!");

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                user = new ApplicationUser
                {
                    UserName = model.Email,
                    Email = model.Email,
                    EmailConfirmed = true,
                    FullName = model.FullName,
                    PhoneNumber = model.PhoneNumber
                };

                var result = await _userManager.CreateAsync(user, model.Password);
                if (!result.Succeeded)
                    return BadRequest(result.Errors);
            }
            else
            {
                user.FullName = model.FullName;
                user.PhoneNumber = model.PhoneNumber;
                await _userManager.UpdateAsync(user);
            }

            var currentRoles = await _userManager.GetRolesAsync(user);
            await _userManager.RemoveFromRolesAsync(user, currentRoles);
            await _userManager.AddToRoleAsync(user, model.Role);

            return Ok(new { message = $"Nhân viên {user.Email} đã được cập nhật với vai trò {model.Role}." });
        }

        [HttpGet("employees")]
        public async Task<IActionResult> GetEmployees()
        {
            var users = await _userManager.Users.ToListAsync();
            var employeeList = new List<object>();
            int count = 1; // Tạo mã NV001, NV002...

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                if (roles.Contains("Employee"))
                {
                    bool isLocked = user.LockoutEnd != null && user.LockoutEnd > DateTime.Now;
                    employeeList.Add(new
                    {
                        EmployeeCode = $"NV{count:D3}", // Mã NV001, NV002,...
                        UserId = user.Id, // ID thực tế để gửi API
                        FullName = user.FullName ?? "Không có",
                        user.Email,
                        user.PhoneNumber,
                        Role = roles.FirstOrDefault() ?? "Chưa có",
                        isLocked
                    });
                    count++;
                }
            }
            return Ok(employeeList);
        }

        [HttpGet("get-parent-categories")]
        public async Task<IActionResult> GetParentCategories()
        {
            var parentCategories = await _context.DanhMucChas
                .Select(d => new { d.MaDanhMucCha, d.TenDanhMucCha })
                .ToListAsync();

            return Ok(parentCategories);
        }

        [HttpPost("add-category")]
        public async Task<IActionResult> AddCategory([FromForm] IFormFile anhDanhMuc, [FromForm] IFormFile icon, [FromForm] string tenDanhMuc, [FromForm] int maDanhMucCha)
        {
            if (maDanhMucCha == 0)
            {
                return BadRequest("Danh mục con bắt buộc phải có danh mục cha!");
            }

            var parentCategory = await _context.DanhMucChas.FindAsync(maDanhMucCha);
            if (parentCategory == null)
            {
                return BadRequest("Mã danh mục cha không hợp lệ!");
            }

            // Lưu ảnh và icon vào thư mục wwwroot/images/categories
            string imagePath = null;
            string iconPath = null;

            if (anhDanhMuc != null)
            {
                imagePath = Path.Combine("wwwroot/images/categories", anhDanhMuc.FileName);
                using (var stream = new FileStream(imagePath, FileMode.Create))
                {
                    await anhDanhMuc.CopyToAsync(stream);
                }
            }

            if (icon != null)
            {
                iconPath = Path.Combine("wwwroot/images/categories", icon.FileName);
                using (var stream = new FileStream(iconPath, FileMode.Create))
                {
                    await icon.CopyToAsync(stream);
                }
            }

            var newCategory = new DanhMuc
            {
                TenDanhMuc = tenDanhMuc,
                MaDanhMucCha = maDanhMucCha,
                AnhDanhMuc = imagePath,
                Icon = iconPath
            };

            _context.DanhMucs.Add(newCategory);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Thêm danh mục thành công!" });
        }

        [HttpPost("add-parent-category")]
        public async Task<IActionResult> AddParentCategory([FromBody] DanhMucCha model)
        {
            if (string.IsNullOrWhiteSpace(model.TenDanhMucCha))
                return BadRequest("Tên danh mục không được để trống!");

            bool exists = await _context.DanhMucChas.AnyAsync(d => d.TenDanhMucCha == model.TenDanhMucCha);
            if (exists)
                return BadRequest("Danh mục cha đã tồn tại!");

            var newCategory = new DanhMucCha { TenDanhMucCha = model.TenDanhMucCha };
            _context.DanhMucChas.Add(newCategory);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Thêm danh mục cha thành công!", category = newCategory });
        }

        [HttpGet("get-subcategories")]
        public async Task<IActionResult> GetSubCategories()
        {
            var subCategories = await _context.DanhMucs
                .Include(d => d.DanhMucCha) // Lấy thông tin danh mục cha
                .Select(d => new
                {
                    d.MaDanhMuc,
                    d.TenDanhMuc,
                    d.AnhDanhMuc,
                    d.Icon,
                    TenDanhMucCha = d.DanhMucCha.TenDanhMucCha // Chuyển MãDanhMucCha thành tên danh mục cha
                })
                .ToListAsync();

            return Ok(subCategories);
        }

        [HttpPut("update-subcategory/{id}")]
        public async Task<IActionResult> UpdateSubCategory(int id, [FromBody] DanhMuc updatedCategory)
        {
            var category = await _context.DanhMucs.FindAsync(id);
            if (category == null) return NotFound("Danh mục không tồn tại.");

            category.TenDanhMuc = updatedCategory.TenDanhMuc ?? category.TenDanhMuc;
            category.AnhDanhMuc = updatedCategory.AnhDanhMuc ?? category.AnhDanhMuc;
            category.Icon = updatedCategory.Icon ?? category.Icon;

            await _context.SaveChangesAsync();
            return Ok(new { message = "Cập nhật danh mục con thành công!" });
        }

        [HttpDelete("delete-subcategory/{id}")]
        public async Task<IActionResult> DeleteSubCategory(int id)
        {
            var category = await _context.DanhMucs.FindAsync(id);
            if (category == null) return NotFound("Danh mục không tồn tại.");

            _context.DanhMucs.Remove(category);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Xóa danh mục con thành công!" });
        }


        // Model thay đổi vai trò
        public class ChangeRoleModel
        {
            public string UserId { get; set; }
            public string NewRole { get; set; }
        }

        // Model cho API gộp thêm nhân viên và thay đổi vai trò
        public class EmployeeRoleModel
        {
            public string FullName { get; set; }
            public string Email { get; set; }
            public string PhoneNumber { get; set; }
            public string Role { get; set; }
            public string Password { get; set; }
        }

      
    }
}
