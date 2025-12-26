using Microsoft.EntityFrameworkCore;
using _65131433_BTL1.Models;   // Namespace của model và DbContext (do RootNamespace của bạn là _65131433_BTL1)

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// ==================== CÁC DỊCH VỤ BẮT BUỘC ====================

// 1. Kết nối Database
builder.Services.AddDbContext<PhongKhamDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("PhongKhamConnection")));

// 2. CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// 3. BẮT BUỘC THÊM 2 DÒNG NÀY ĐỂ RAZOR PAGES HOẠT ĐỘNG
builder.Services.AddRazorPages();   // ← THÊM DÒNG NÀY

// ===========================================================

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowAll");

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

// BẮT BUỘC THÊM DÒNG NÀY ĐỂ RAZOR PAGES ĐƯỢC ROUTING
app.MapRazorPages();   // ← THÊM DÒNG NÀY

app.Run();