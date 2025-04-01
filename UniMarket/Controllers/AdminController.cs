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
                .Select(d => new
                {
                    d.MaDanhMucCha,
                    d.TenDanhMucCha,
                    d.AnhDanhMuc,
                    d.Icon
                })
                .ToListAsync();

            return Ok(parentCategories);
        }

        [HttpPost("add-category")]
        public async Task<IActionResult> AddCategory([FromForm] string tenDanhMuc, [FromForm] int maDanhMucCha)
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

            var newCategory = new DanhMuc
            {
                TenDanhMuc = tenDanhMuc,
                MaDanhMucCha = maDanhMucCha
            };

            _context.DanhMucs.Add(newCategory);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Thêm danh mục thành công!" });
        }

        [HttpPost("add-parent-category")]
        public async Task<IActionResult> AddParentCategory([FromForm] IFormFile anhDanhMuc, [FromForm] IFormFile icon, [FromForm] string tenDanhMucCha)
        {
            if (string.IsNullOrWhiteSpace(tenDanhMucCha))
                return BadRequest("Tên danh mục không được để trống!");

            bool exists = await _context.DanhMucChas.AnyAsync(d => d.TenDanhMucCha == tenDanhMucCha);
            if (exists)
                return BadRequest("Danh mục cha đã tồn tại!");

            // Lưu ảnh và icon vào thư mục wwwroot/images/categories
            string imagePath = null;
            string iconPath = null;

            if (anhDanhMuc != null)
            {
                imagePath = Path.Combine("images/categories", anhDanhMuc.FileName);
                var fullPath = Path.Combine("wwwroot", imagePath);
                using (var stream = new FileStream(fullPath, FileMode.Create))
                {
                    await anhDanhMuc.CopyToAsync(stream);
                }
            }

            if (icon != null)
            {
                iconPath = Path.Combine("images/categories", icon.FileName);
                var fullPath = Path.Combine("wwwroot", iconPath);
                using (var stream = new FileStream(fullPath, FileMode.Create))
                {
                    await icon.CopyToAsync(stream);
                }
            }

            var newCategory = new DanhMucCha
            {
                TenDanhMucCha = tenDanhMucCha,
                AnhDanhMuc = imagePath,
                Icon = iconPath
            };

            _context.DanhMucChas.Add(newCategory);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Thêm danh mục cha thành công!", category = newCategory });
        }

        [HttpGet("get-subcategories")]
        public async Task<IActionResult> GetSubCategories()
        {
            var subCategories = await _context.DanhMucs
                .Include(d => d.DanhMucCha)
                .Select(d => new
                {
                    d.MaDanhMuc,
                    d.TenDanhMuc,
                    TenDanhMucCha = d.DanhMucCha.TenDanhMucCha,
                    AnhDanhMuc = d.DanhMucCha.AnhDanhMuc, // Lấy từ danh mục cha
                    Icon = d.DanhMucCha.Icon // Lấy từ danh mục cha
                })
                .ToListAsync();

            return Ok(subCategories);
        }
        //quản lý danh mục con

        [HttpPut("update-subcategory/{id}")]
        public async Task<IActionResult> UpdateSubCategory(int id, [FromBody] DanhMuc updatedCategory)
        {
            var category = await _context.DanhMucs.FindAsync(id);
            if (category == null) return NotFound("Danh mục không tồn tại.");

            category.TenDanhMuc = updatedCategory.TenDanhMuc ?? category.TenDanhMuc;
            category.MaDanhMucCha = updatedCategory.MaDanhMucCha;

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
        //quản lý danh mục cha
        [HttpPut("update-parent-category/{id}")]
        public async Task<IActionResult> UpdateParentCategory(int id,
     [FromForm] IFormFile? anhDanhMuc, // Thêm dấu ? để cho phép null
    [FromForm] IFormFile? icon,
    [FromForm] string? tenDanhMucCha) // Cho phép null
        {
            var category = await _context.DanhMucChas.FindAsync(id);
            if (category == null)
                return NotFound("Danh mục cha không tồn tại.");

            if (!string.IsNullOrWhiteSpace(tenDanhMucCha))
            {
                // Kiểm tra trùng tên (trừ chính nó)
                bool exists = await _context.DanhMucChas
                    .AnyAsync(d => d.TenDanhMucCha == tenDanhMucCha && d.MaDanhMucCha != id);

                if (exists)
                    return BadRequest("Tên danh mục cha đã tồn tại!");

                category.TenDanhMucCha = tenDanhMucCha;
            }
            // Thêm debug log
            Console.WriteLine($"Updating category {id} with name: {tenDanhMucCha}");

            // Xử lý ảnh mới
            if (anhDanhMuc != null)
            {
                string imagePath = Path.Combine("images/categories", anhDanhMuc.FileName);
                var fullPath = Path.Combine("wwwroot", imagePath);

                using (var stream = new FileStream(fullPath, FileMode.Create))
                {
                    await anhDanhMuc.CopyToAsync(stream);
                }

                // Xóa ảnh cũ nếu có
                if (!string.IsNullOrEmpty(category.AnhDanhMuc))
                {
                    var oldImagePath = Path.Combine("wwwroot", category.AnhDanhMuc);
                    if (System.IO.File.Exists(oldImagePath))
                    {
                        System.IO.File.Delete(oldImagePath);
                    }
                }

                category.AnhDanhMuc = imagePath;
            }

            // Xử lý icon mới
            if (icon != null)
            {
                string iconPath = Path.Combine("images/categories", icon.FileName);
                var fullPath = Path.Combine("wwwroot", iconPath);

                using (var stream = new FileStream(fullPath, FileMode.Create))
                {
                    await icon.CopyToAsync(stream);
                }

                // Xóa icon cũ nếu có
                if (!string.IsNullOrEmpty(category.Icon))
                {
                    var oldIconPath = Path.Combine("wwwroot", category.Icon);
                    if (System.IO.File.Exists(oldIconPath))
                    {
                        System.IO.File.Delete(oldIconPath);
                    }
                }

                category.Icon = iconPath;
            }

            await _context.SaveChangesAsync();
            return Ok(new
            {
                message = "Cập nhật danh mục cha thành công!",
                category = new
                {
                    category.MaDanhMucCha,
                    category.TenDanhMucCha,
                    category.AnhDanhMuc,
                    category.Icon
                }
            });
        }

        [HttpDelete("delete-parent-category/{id}")]
        public async Task<IActionResult> DeleteParentCategory(int id)
        {
            var category = await _context.DanhMucChas
                .Include(c => c.DanhMucs) // Include các danh mục con
                .FirstOrDefaultAsync(c => c.MaDanhMucCha == id);

            if (category == null)
                return NotFound("Danh mục cha không tồn tại.");

            if (category.DanhMucs.Any())
            {
                var subCategoryNames = category.DanhMucs.Select(sub => sub.TenDanhMuc).ToList();
                var subCategoryList = string.Join(", ", subCategoryNames);
                Console.WriteLine($"Không thể xóa danh mục cha khi còn danh mục con: {subCategoryList}"); // Logging
                return BadRequest(new { message = $"Không thể xóa danh mục cha khi còn danh mục con: {subCategoryList}. Vui lòng xóa các danh mục con trước." });
            }

            // Xóa ảnh và icon nếu có
            if (!string.IsNullOrEmpty(category.AnhDanhMuc))
            {
                var imagePath = Path.Combine("wwwroot", category.AnhDanhMuc);
                if (System.IO.File.Exists(imagePath))
                {
                    System.IO.File.Delete(imagePath);
                }
            }

            if (!string.IsNullOrEmpty(category.Icon))
            {
                var iconPath = Path.Combine("wwwroot", category.Icon);
                if (System.IO.File.Exists(iconPath))
                {
                    System.IO.File.Delete(iconPath);
                }
            }

            _context.DanhMucChas.Remove(category);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Xóa danh mục cha thành công!" });
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
