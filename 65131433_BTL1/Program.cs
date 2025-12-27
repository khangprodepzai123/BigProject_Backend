

//using Microsoft.EntityFrameworkCore;
//using _65131433_BTL1.Models;
//using _65131433_BTL1.Services;  // ← THÊM DÒNG NÀY

//var builder = WebApplication.CreateBuilder(args);

//// Add services to the container.
//builder.Services.AddControllers();
//builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen();

//// ==================== CÁC DỊCH VỤ BẮT BUỘC ====================

//// 1. Kết nối Database
//builder.Services.AddDbContext<PhongKhamDbContext>(options =>
//    options.UseSqlServer(builder.Configuration.GetConnectionString("PhongKhamConnection")));

//// 2. JWT Service ← THÊM DÒNG NÀY (QUAN TRỌNG!)
//builder.Services.AddScoped<IJwtService, JwtService>();

//// 3. CORS
//builder.Services.AddCors(options =>
//{
//    options.AddPolicy("AllowAll", policy =>
//    {
//        policy.AllowAnyOrigin()
//              .AllowAnyMethod()
//              .AllowAnyHeader();
//    });
//});

//// 4. Razor Pages
//builder.Services.AddRazorPages();

//// ===========================================================

//var app = builder.Build();

//// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
//    app.UseSwagger();
//    app.UseSwaggerUI();
//}

//app.UseCors("AllowAll");
////app.UseHttpsRedirection();
//app.UseAuthorization();

//app.MapControllers();
//app.MapRazorPages();

//app.Run();

using Microsoft.EntityFrameworkCore;
using _65131433_BTL1.Models;
using _65131433_BTL1.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<PhongKhamDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("PhongKhamConnection")));

builder.Services.AddScoped<IJwtService, JwtService>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

builder.Services.AddRazorPages();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowAll");

// ← THÊM EXCEPTION HANDLER NÀY
app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        var exception = context.Features.Get<Microsoft.AspNetCore.Diagnostics.IExceptionHandlerFeature>();
        if (exception?.Error != null)
        {
            Console.WriteLine($"❌ EXCEPTION: {exception.Error.Message}");
            Console.WriteLine($"Stack Trace: {exception.Error.StackTrace}");
        }
    });
});

app.UseAuthorization();
app.MapControllers();
app.MapRazorPages();

app.Run();
