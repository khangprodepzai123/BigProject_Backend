using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace _65131433_BTL1.Models;

public partial class PhongKhamDbContext : DbContext
{
    public PhongKhamDbContext()
    {
    }

    public PhongKhamDbContext(DbContextOptions<PhongKhamDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<BacSi> BacSis { get; set; }

    public virtual DbSet<BenhNhan> BenhNhans { get; set; }

    public virtual DbSet<ChiTietHoaDon> ChiTietHoaDons { get; set; }

    public virtual DbSet<ChuanDoan> ChuanDoans { get; set; }

    public virtual DbSet<HoaDon> HoaDons { get; set; }

    public virtual DbSet<KhamBenh> KhamBenhs { get; set; }

    public virtual DbSet<NhanVien> NhanViens { get; set; }

    public virtual DbSet<TaiKhoanBenhNhan> TaiKhoanBenhNhans { get; set; }

    public virtual DbSet<Thuoc> Thuocs { get; set; }

    public virtual DbSet<ToaThuoc> ToaThuocs { get; set; }

    public virtual DbSet<BenhAn> BenhAns { get; set; }

    public virtual DbSet<BenhAnToaThuoc> BenhAnToaThuocs { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=localhost\\SQLEXPRESS;Database=PhongKhamOnline;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<BacSi>(entity =>
        {
            entity.HasKey(e => e.MaBs).HasName("PK__BacSi__272475F627C3C469");

            entity.ToTable("BacSi");

            entity.Property(e => e.MaBs)
                .HasMaxLength(10)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.HoTenBs).HasMaxLength(100);
        });

        modelBuilder.Entity<BenhNhan>(entity =>
        {
            entity.HasKey(e => e.MaBn).HasName("PK__BenhNhan__2724758D4B1E221B");

            entity.ToTable("BenhNhan");

            entity.Property(e => e.MaBn)
                .HasMaxLength(10)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.Bhyt)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("BHYT");
            entity.Property(e => e.DiaChi).HasMaxLength(200);
            entity.Property(e => e.DoiTuong).HasMaxLength(50);
            entity.Property(e => e.Gt)
                .HasMaxLength(10)
                .HasColumnName("GT");
            entity.Property(e => e.HoTenBn).HasMaxLength(100);
            entity.Property(e => e.Sdt)
                .HasMaxLength(15)
                .IsUnicode(false)
                .HasColumnName("SDT");
        });

        modelBuilder.Entity<ChiTietHoaDon>(entity =>
        {
            entity.HasKey(e => new { e.MaHd, e.MaThuoc }).HasName("PK__ChiTietH__339EB9A21FB9B98C");

            entity.ToTable("ChiTietHoaDon");

            entity.Property(e => e.MaHd)
                .HasMaxLength(10)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.MaThuoc)
                .HasMaxLength(10)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.DonGia).HasColumnType("decimal(12, 0)");

            entity.HasOne(d => d.MaHdNavigation).WithMany(p => p.ChiTietHoaDons)
                .HasForeignKey(d => d.MaHd)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ChiTiet_HoaDon");

            entity.HasOne(d => d.MaThuocNavigation).WithMany(p => p.ChiTietHoaDons)
                .HasForeignKey(d => d.MaThuoc)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ChiTiet_Thuoc");
        });

        modelBuilder.Entity<ChuanDoan>(entity =>
        {
            entity.HasKey(e => e.MaCd).HasName("PK__ChuanDoa__27258E64D9722A9E");

            entity.ToTable("ChuanDoan");

            entity.Property(e => e.MaCd)
                .HasMaxLength(10)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.MoTa).HasMaxLength(500);
            entity.Property(e => e.TenCd).HasMaxLength(200);
        });

        modelBuilder.Entity<HoaDon>(entity =>
        {
            entity.HasKey(e => e.MaHd).HasName("PK__HoaDon__2725A6C0D6C176F9");

            entity.ToTable("HoaDon");

            entity.HasIndex(e => e.MaKham, "UQ__HoaDon__653E9A7BB69B932E").IsUnique();

            entity.Property(e => e.MaHd)
                .HasMaxLength(10)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.DiemTichLuySuDung).HasDefaultValue(0);
            entity.Property(e => e.MaKham)
                .HasMaxLength(10)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.MaNv)
                .HasMaxLength(10)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.NgayLap).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.ThanhTien).HasColumnType("decimal(12, 0)");

            entity.HasOne(d => d.MaKhamNavigation).WithOne(p => p.HoaDon)
                .HasForeignKey<HoaDon>(d => d.MaKham)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_HoaDon_KhamBenh");

            entity.HasOne(d => d.MaNvNavigation).WithMany(p => p.HoaDons)
                .HasForeignKey(d => d.MaNv)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_HoaDon_NhanVien");
        });

        modelBuilder.Entity<KhamBenh>(entity =>
        {
            entity.HasKey(e => e.MaKham).HasName("PK__KhamBenh__653E9A7A72B89118");

            entity.ToTable("KhamBenh");

            entity.HasIndex(e => e.MaBn, "UQ_KhamBenh_MaBn").IsUnique();

            entity.Property(e => e.MaKham)
                .HasMaxLength(10)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.ChuanDoan).HasMaxLength(500);
            entity.Property(e => e.HuongXuTri).HasMaxLength(500);
            entity.Property(e => e.KhamBoPhan).HasMaxLength(1000);
            entity.Property(e => e.LoaiKham).HasMaxLength(50);
            entity.Property(e => e.LyDoKham).HasMaxLength(500);
            entity.Property(e => e.MaBn)
                .HasMaxLength(10)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.MaBs)
                .HasMaxLength(10)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.MaCd)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasDefaultValue("CD001")
                .IsFixedLength();
            entity.Property(e => e.NgayKham).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.QuaTrinhBenhLy).HasMaxLength(1000);
            entity.Property(e => e.TienSuBenhNhan).HasMaxLength(500);
            entity.Property(e => e.TienSuGiaDinh).HasMaxLength(500);
            entity.Property(e => e.TrangThai)
                .HasMaxLength(50)
                .HasDefaultValue("Đã khám");
            entity.Property(e => e.XuTriKham).HasMaxLength(100);

            entity.HasOne(d => d.MaBnNavigation).WithOne(p => p.KhamBenh)
                .HasForeignKey<KhamBenh>(d => d.MaBn)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_KhamBenh_BenhNhan");

            entity.HasOne(d => d.MaBsNavigation).WithMany(p => p.KhamBenhs)
                .HasForeignKey(d => d.MaBs)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_KhamBenh_BacSi");

            entity.HasOne(d => d.MaCdNavigation).WithMany(p => p.KhamBenhs)
                .HasForeignKey(d => d.MaCd)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_KhamBenh_ChuanDoan");
        });

        modelBuilder.Entity<NhanVien>(entity =>
        {
            entity.HasKey(e => e.MaNv).HasName("PK__NhanVien__2725D76A89F344F9");

            entity.ToTable("NhanVien");

            entity.Property(e => e.MaNv)
                .HasMaxLength(10)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.HoTenNv).HasMaxLength(100);
        });

        modelBuilder.Entity<TaiKhoanBenhNhan>(entity =>
        {
            entity.HasKey(e => e.MaTk).HasName("PK__TaiKhoan__27250050DE04A84A");

            entity.ToTable("TaiKhoanBenhNhan");

            entity.HasIndex(e => e.MaBn, "UQ__TaiKhoan__2724758CEF6C20EF").IsUnique();

            entity.HasIndex(e => e.TenDangNhap, "UQ__TaiKhoan__55F68FC03AD97DE8").IsUnique();

            entity.Property(e => e.MaTk)
                .HasMaxLength(10)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.DiemTichLuy).HasDefaultValue(0);
            entity.Property(e => e.MaBn)
                .HasMaxLength(10)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.MatKhau)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.TenDangNhap)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.HoTenBn)
                .HasMaxLength(255);

            entity.HasOne(d => d.MaBnNavigation).WithOne(p => p.TaiKhoanBenhNhan)
                .HasForeignKey<TaiKhoanBenhNhan>(d => d.MaBn)
                .HasConstraintName("FK_TaiKhoan_BenhNhan");
        });

        modelBuilder.Entity<Thuoc>(entity =>
        {
            entity.HasKey(e => e.MaThuoc).HasName("PK__Thuoc__4BB1F62012D22D35");

            entity.ToTable("Thuoc");

            entity.Property(e => e.MaThuoc)
                .HasMaxLength(10)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.GiaBan).HasColumnType("decimal(12, 0)");
            entity.Property(e => e.Hdsd)
                .HasMaxLength(500)
                .HasColumnName("HDSD");
            entity.Property(e => e.TenThuoc).HasMaxLength(100);
        });

        modelBuilder.Entity<ToaThuoc>(entity =>
        {
            entity.HasKey(e => new { e.MaKham, e.MaThuoc }).HasName("PK__ToaThuoc__7185851825F4C152");

            entity.ToTable("ToaThuoc");

            entity.Property(e => e.MaKham)
                .HasMaxLength(10)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.MaThuoc)
                .HasMaxLength(10)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.CachDung).HasMaxLength(300);
            entity.Property(e => e.LieuDung).HasMaxLength(200);

            entity.HasOne(d => d.MaKhamNavigation).WithMany(p => p.ToaThuocs)
                .HasForeignKey(d => d.MaKham)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ToaThuoc_KhamBenh");

            entity.HasOne(d => d.MaThuocNavigation).WithMany(p => p.ToaThuocs)
                .HasForeignKey(d => d.MaThuoc)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ToaThuoc_Thuoc");
        });

        modelBuilder.Entity<BenhAn>(entity =>
        {
            entity.HasKey(e => e.MaBenhAn).HasName("PK__BenhAn__27250050DE04A84B");

            entity.ToTable("BenhAn");

            entity.Property(e => e.MaBenhAn)
                .HasMaxLength(10)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.MaKham)
                .HasMaxLength(10)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.MaBn)
                .HasMaxLength(10)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.MaBs)
                .HasMaxLength(10)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.ChuanDoan).HasMaxLength(500);
            entity.Property(e => e.HuongXuTri).HasMaxLength(500);
            entity.Property(e => e.KhamBoPhan).HasMaxLength(1000);
            entity.Property(e => e.LoaiKham).HasMaxLength(50);
            entity.Property(e => e.LyDoKham).HasMaxLength(500);
            entity.Property(e => e.QuaTrinhBenhLy).HasMaxLength(1000);
            entity.Property(e => e.TienSuBenhNhan).HasMaxLength(500);
            entity.Property(e => e.TienSuGiaDinh).HasMaxLength(500);
            entity.Property(e => e.XuTriKham).HasMaxLength(100);
            entity.Property(e => e.NgayLuu).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.MaBnNavigation).WithMany()
                .HasForeignKey(d => d.MaBn)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_BenhAn_BenhNhan");

            entity.HasOne(d => d.MaBsNavigation).WithMany()
                .HasForeignKey(d => d.MaBs)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_BenhAn_BacSi");
        });

        modelBuilder.Entity<BenhAnToaThuoc>(entity =>
        {
            entity.HasKey(e => new { e.MaBenhAn, e.MaThuoc }).HasName("PK__BenhAnTo__7185851825F4C153");

            entity.ToTable("BenhAnToaThuoc");

            entity.Property(e => e.MaBenhAn)
                .HasMaxLength(10)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.MaThuoc)
                .HasMaxLength(10)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.CachDung).HasMaxLength(300);
            entity.Property(e => e.LieuDung).HasMaxLength(200);

            entity.HasOne(d => d.MaBenhAnNavigation).WithMany(p => p.BenhAnToaThuocs)
                .HasForeignKey(d => d.MaBenhAn)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_BenhAnToaThuoc_BenhAn");

            entity.HasOne(d => d.MaThuocNavigation).WithMany()
                .HasForeignKey(d => d.MaThuoc)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_BenhAnToaThuoc_Thuoc");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
