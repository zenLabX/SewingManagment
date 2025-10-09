using SewingManagment.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient; // 可選，僅當你需要直接使用 SqlClient
using Microsoft.EntityFrameworkCore.SqlServer; // 加入這一行以確保 UseSqlServer 擴充方法可用

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// 配置 Session (如果沒有自動配置 Session) 並且在 builder.Build() 之前加入
builder.Services.AddDistributedMemoryCache(); // 暫存
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Session 過期時間
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

// Middleware
app.UseSession(); // 這行必須在 UseRouting() 之前才能正常運作，如果沒有 UseSession() 或 AddSession()，Session Set/Get 可能會出錯。

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
