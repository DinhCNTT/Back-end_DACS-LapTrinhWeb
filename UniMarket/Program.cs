using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
<<<<<<< HEAD
using Newtonsoft.Json;
using System.Text;
using UniMarket.DataAccess;
using UniMarket.Models;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

// 1️⃣ CORS
=======
using System.Text;
using UniMarket.DataAccess;
using UniMarket.Models;

var builder = WebApplication.CreateBuilder(args);

// Định nghĩa chính sách CORS với AllowCredentials
>>>>>>> 943974eeb10876c1b0694a7901d19e5ad515c6cb
var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
builder.Services.AddCors(options =>
{
    options.AddPolicy(MyAllowSpecificOrigins, policy =>
    {
<<<<<<< HEAD
        policy.WithOrigins("http://localhost:5173") // Đảm bảo frontend đang chạy trên localhost:5173
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

// 2️⃣ DbContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// 3️⃣ Identity
=======
        policy.WithOrigins("http://localhost:5173") // Đúng với cổng frontend
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials(); // ✅ Cần thiết để gửi JWT / Cookies
    });
});

// Cấu hình Database với Entity Framework Core
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Cấu hình Identity
>>>>>>> 943974eeb10876c1b0694a7901d19e5ad515c6cb
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders()
    .AddDefaultUI();

<<<<<<< HEAD
// Cấu hình Cookie để API không redirect HTML khi lỗi
builder.Services.ConfigureApplicationCookie(options =>
{
    options.Events.OnRedirectToLogin = context =>
    {
        if (context.Request.Path.StartsWithSegments("/api"))
        {
            context.Response.StatusCode = 401;
            context.Response.ContentType = "application/json";
            return context.Response.WriteAsync("{\"message\": \"Unauthorized - Vui lòng đăng nhập.\"}");
        }

        context.Response.Redirect(context.RedirectUri);
        return Task.CompletedTask;
    };
});

// 4️⃣ JWT Configuration
=======
// Cấu hình xác thực JWT
>>>>>>> 943974eeb10876c1b0694a7901d19e5ad515c6cb
var jwtSettings = builder.Configuration.GetSection("Jwt");
var key = Encoding.UTF8.GetBytes(jwtSettings["Key"] ?? throw new ArgumentNullException("Jwt:Key không được để trống"));

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
<<<<<<< HEAD
        options.Events = new JwtBearerEvents
        {
            OnChallenge = context =>
            {
                context.HandleResponse();
                context.Response.StatusCode = 401;
                context.Response.ContentType = "application/json";
                return context.Response.WriteAsync("{\"message\": \"Unauthorized - Token không hợp lệ hoặc đã hết hạn.\"}");
            }
        };

=======
        options.RequireHttpsMetadata = false;
        options.SaveToken = true;
>>>>>>> 943974eeb10876c1b0694a7901d19e5ad515c6cb
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(key)
        };
    });

<<<<<<< HEAD
builder.Services.AddAuthorization();

// 5️⃣ Swagger Configuration
=======
// Cấu hình Swagger (OpenAPI) hỗ trợ Bearer Token
>>>>>>> 943974eeb10876c1b0694a7901d19e5ad515c6cb
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "UniMarket API",
        Version = "v1",
        Description = "API cho hệ thống mua bán UniMarket",
        Contact = new OpenApiContact
        {
            Name = "Nguyễn Xuân Đạt",
            Email = "contact@unimarket.com"
        }
    });

<<<<<<< HEAD
=======
    // Cấu hình bảo mật Bearer Token trong Swagger
>>>>>>> 943974eeb10876c1b0694a7901d19e5ad515c6cb
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Nhập JWT Token vào đây. Ví dụ: Bearer {your-token}"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
<<<<<<< HEAD
            new string[] { }
=======
            new string[] {}
>>>>>>> 943974eeb10876c1b0694a7901d19e5ad515c6cb
        }
    });
});

<<<<<<< HEAD
// 6️⃣ Controller + xử lý lỗi model và sử dụng Newtonsoft.Json
builder.Services.AddControllers()
    .AddNewtonsoftJson(options =>
    {
        options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;  // Giải quyết tuần hoàn
    })
    .ConfigureApiBehaviorOptions(options =>
    {
        options.InvalidModelStateResponseFactory = context =>
        {
            var errors = context.ModelState
                .Where(x => x.Value?.Errors.Count > 0)
                .Select(x => new
                {
                    Field = x.Key,
                    Errors = x.Value?.Errors.Select(e => e.ErrorMessage).ToArray()
                });

            return new BadRequestObjectResult(new
            {
                Message = "Dữ liệu không hợp lệ.",
                Errors = errors
            });
        };
    });

var app = builder.Build();

// 7️⃣ Middleware
=======
var app = builder.Build();

// Cấu hình Middleware
>>>>>>> 943974eeb10876c1b0694a7901d19e5ad515c6cb
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "UniMarket API v1");
        c.RoutePrefix = "swagger";
    });
}

<<<<<<< HEAD
app.UseCors(MyAllowSpecificOrigins); // Áp dụng CORS cho tất cả yêu cầu
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.UseStaticFiles(); // wwwroot
=======
app.UseCors(MyAllowSpecificOrigins); // ✅ Quan trọng: Đặt ngay trước Authentication
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.UseStaticFiles(); // Cho phép ứng dụng phục vụ các tệp tĩnh từ thư mục wwwroot
app.MapRazorPages();
app.MapControllers();
>>>>>>> 943974eeb10876c1b0694a7901d19e5ad515c6cb
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
        Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/categories")),
    RequestPath = "/images/categories"
});

<<<<<<< HEAD
// Cấu hình phục vụ ảnh từ thư mục "wwwroot/images/Posts"
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
        Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/Posts")),
    RequestPath = "/images/Posts" // Đây là đường dẫn bạn sẽ sử dụng trong frontend
});

app.MapRazorPages();
app.MapControllers();

// 8️⃣ Tạo role & admin mặc định
=======
// Đảm bảo tạo vai trò và tài khoản Admin khi khởi động
>>>>>>> 943974eeb10876c1b0694a7901d19e5ad515c6cb
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    await InitializeRolesAndAdmin(services);
}

<<<<<<< HEAD
// 9️⃣ Run app
await app.RunAsync();

=======
// Chạy ứng dụng (async)
await app.RunAsync();

// Hàm khởi tạo vai trò và tài khoản Admin mặc định
>>>>>>> 943974eeb10876c1b0694a7901d19e5ad515c6cb
async Task InitializeRolesAndAdmin(IServiceProvider serviceProvider)
{
    var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

<<<<<<< HEAD
    string[] roleNames = { "Admin", "Employee", "User" };
=======
    string[] roleNames = { SD.Role_Admin, SD.Role_Employee, SD.Role_User };
>>>>>>> 943974eeb10876c1b0694a7901d19e5ad515c6cb

    foreach (var roleName in roleNames)
    {
        if (!await roleManager.RoleExistsAsync(roleName))
        {
            await roleManager.CreateAsync(new IdentityRole(roleName));
        }
    }

    string adminEmail = "admin@unimarket.com";
<<<<<<< HEAD
    string adminPassword = "Admin@123";
=======
    string adminPassword = "Admin@123"; // Thay đổi khi triển khai thực tế
>>>>>>> 943974eeb10876c1b0694a7901d19e5ad515c6cb

    var adminUser = await userManager.FindByEmailAsync(adminEmail);
    if (adminUser == null)
    {
        var newAdmin = new ApplicationUser
        {
            UserName = adminEmail,
            Email = adminEmail,
            EmailConfirmed = true,
            FullName = "Admin User"
        };

        var createAdminResult = await userManager.CreateAsync(newAdmin, adminPassword);
        if (createAdminResult.Succeeded)
        {
<<<<<<< HEAD
            await userManager.AddToRoleAsync(newAdmin, "Admin");
        }
        else
        {
            Console.WriteLine("Lỗi tạo admin: " + string.Join(", ", createAdminResult.Errors.Select(e => e.Description)));
=======
            await userManager.AddToRoleAsync(newAdmin, SD.Role_Admin);
        }
        else
        {
            Console.WriteLine("Lỗi khi tạo Admin: " + string.Join(", ", createAdminResult.Errors.Select(e => e.Description)));
>>>>>>> 943974eeb10876c1b0694a7901d19e5ad515c6cb
        }
    }
}
