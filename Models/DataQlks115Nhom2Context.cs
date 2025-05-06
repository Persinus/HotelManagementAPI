using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace HotelManagementAPI.Models;

public partial class DataQlks115Nhom2Context : DbContext
{
    public DataQlks115Nhom2Context()
    {
    }

    public DataQlks115Nhom2Context(DbContextOptions<DataQlks115Nhom2Context> options)
        : base(options)
    {
    }

    public virtual DbSet<DatPhong> DatPhongs { get; set; }

    public virtual DbSet<DichVu> DichVus { get; set; }

    public virtual DbSet<HoaDonThanhToanDichVu> HoaDonThanhToanDichVus { get; set; }

    public virtual DbSet<HoaDonThanhToanPhong> HoaDonThanhToanPhongs { get; set; }

    public virtual DbSet<KhachHang> KhachHangs { get; set; }

    public virtual DbSet<KhachHangDichVu> KhachHangDichVus { get; set; }

    public virtual DbSet<KhachHangPhanThuong> KhachHangPhanThuongs { get; set; }

    public virtual DbSet<NhanVien> NhanViens { get; set; }

    public virtual DbSet<PhanThuong> PhanThuongs { get; set; }

    public virtual DbSet<PhongWithTienNghi> PhongWithTienNghis { get; set; }

    public virtual DbSet<QuanTriVien> QuanTriViens { get; set; }

    public virtual DbSet<UserFeedback> UserFeedbacks { get; set; }

    public virtual DbSet<VeGiamGium> VeGiamGia { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=118.69.126.49;Database=data_QLKS_115_Nhom2;User Id=user_115_nhom2;Password=123456789;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<DatPhong>(entity =>
        {
            entity.HasKey(e => e.MaDatPhong).HasName("PK__DatPhong__6344ADEA2FD520E3");

            entity.ToTable("DatPhong");

            entity.Property(e => e.MaDatPhong).HasMaxLength(10);
            entity.Property(e => e.MaKhachHang).HasMaxLength(10);
            entity.Property(e => e.MaNhanVien).HasMaxLength(10);
            entity.Property(e => e.MaPhong).HasMaxLength(10);
            entity.Property(e => e.NgayDat)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.NgayNhanPhong).HasColumnType("datetime");
            entity.Property(e => e.NgayTraPhong).HasColumnType("datetime");

            entity.HasOne(d => d.MaKhachHangNavigation).WithMany(p => p.DatPhongs)
                .HasForeignKey(d => d.MaKhachHang)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__DatPhong__MaKhac__24E777C3");

            entity.HasOne(d => d.MaNhanVienNavigation).WithMany(p => p.DatPhongs)
                .HasForeignKey(d => d.MaNhanVien)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__DatPhong__MaNhan__25DB9BFC");

            entity.HasOne(d => d.MaPhongNavigation).WithMany(p => p.DatPhongs)
                .HasForeignKey(d => d.MaPhong)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__DatPhong__MaPhon__26CFC035");
        });

        modelBuilder.Entity<DichVu>(entity =>
        {
            entity.HasKey(e => e.MaDichVu).HasName("PK__DichVu__11EFCA674FE70F8A");

            entity.ToTable("DichVu");

            entity.Property(e => e.MaDichVu)
                .HasMaxLength(10)
                .HasColumnName("maDichVu");
            entity.Property(e => e.DonGia).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.MoTaDichVu).HasMaxLength(300);
            entity.Property(e => e.TenDichVu).HasMaxLength(100);
            entity.Property(e => e.TrangThai).HasDefaultValue(1);
            entity.Property(e => e.UrlAnh).HasMaxLength(255);
        });

        modelBuilder.Entity<HoaDonThanhToanDichVu>(entity =>
        {
            entity.HasKey(e => e.MaHoaDonDichVu).HasName("PK__HoaDonTh__7C7332D7F3C6550B");

            entity.ToTable("HoaDonThanhToanDichVu");

            entity.Property(e => e.MaHoaDonDichVu).HasMaxLength(10);
            entity.Property(e => e.MaChiTietDichVu).HasMaxLength(10);
            entity.Property(e => e.MaKhachHang).HasMaxLength(10);
            entity.Property(e => e.NgayLapHoaDon)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.ThanhTien).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.TrangThaiThanhToan).HasMaxLength(1);

            entity.HasOne(d => d.MaChiTietDichVuNavigation).WithMany(p => p.HoaDonThanhToanDichVus)
                .HasForeignKey(d => d.MaChiTietDichVu)
                .HasConstraintName("FK__HoaDonTha__MaChi__39E294A9");

            entity.HasOne(d => d.MaKhachHangNavigation).WithMany(p => p.HoaDonThanhToanDichVus)
                .HasForeignKey(d => d.MaKhachHang)
                .HasConstraintName("FK__HoaDonTha__MaKha__38EE7070");
        });

        modelBuilder.Entity<HoaDonThanhToanPhong>(entity =>
        {
            entity.HasKey(e => e.MaHoaDon).HasName("PK__HoaDonTh__835ED13B969A0D2A");

            entity.ToTable("HoaDonThanhToanPhong");

            entity.Property(e => e.MaHoaDon).HasMaxLength(10);
            entity.Property(e => e.MaDatPhong).HasMaxLength(10);
            entity.Property(e => e.MaNhanVien).HasMaxLength(10);
            entity.Property(e => e.NgayLapHoaDon)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.TongTien).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.TrangThaiThanhToan).HasMaxLength(1);

            entity.HasOne(d => d.MaDatPhongNavigation).WithMany(p => p.HoaDonThanhToanPhongs)
                .HasForeignKey(d => d.MaDatPhong)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__HoaDonTha__MaDat__3335971A");

            entity.HasOne(d => d.MaNhanVienNavigation).WithMany(p => p.HoaDonThanhToanPhongs)
                .HasForeignKey(d => d.MaNhanVien)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__HoaDonTha__MaNha__3429BB53");
        });

        modelBuilder.Entity<KhachHang>(entity =>
        {
            entity.HasKey(e => e.MaKhachHang).HasName("PK__KhachHan__88D2F0E5C3B1F2A2");

            entity.ToTable("KhachHang");

            entity.HasIndex(e => e.SoDienThoai, "UQ__KhachHan__0389B7BDB9FC1CCF").IsUnique();

            entity.HasIndex(e => e.CanCuocCongDan, "UQ__KhachHan__9A7D3DD775B63E1A").IsUnique();

            entity.Property(e => e.MaKhachHang).HasMaxLength(10);
            entity.Property(e => e.CanCuocCongDan).HasMaxLength(20);
            entity.Property(e => e.DiaChi).HasMaxLength(100);
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .HasDefaultValue("default@example.com");
            entity.Property(e => e.GioiTinh).HasMaxLength(10);
            entity.Property(e => e.HinhAnh).HasMaxLength(255);
            entity.Property(e => e.HoTen).HasMaxLength(50);
            entity.Property(e => e.Jwk).HasColumnName("JWK");
            entity.Property(e => e.MatKhau)
                .HasMaxLength(255)
                .HasDefaultValue("defaultPassword");
            entity.Property(e => e.NgayCapNhat)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.NgayTao)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.NgheNghiep).HasMaxLength(50);
            entity.Property(e => e.SoDienThoai).HasMaxLength(15);
            entity.Property(e => e.TenTaiKhoan)
                .HasMaxLength(50)
                .HasDefaultValue("defaultUser");
            entity.Property(e => e.TrangThai)
                .HasMaxLength(20)
                .HasDefaultValue("Active");
        });

        modelBuilder.Entity<KhachHangDichVu>(entity =>
        {
            entity.HasKey(e => new { e.MaKhachHang, e.MaChiTietDichVu }).HasName("PK__KhachHan__E9CC0C43C37FEF1F");

            entity.ToTable("KhachHang_DichVu");

            entity.Property(e => e.MaKhachHang).HasMaxLength(10);
            entity.Property(e => e.MaChiTietDichVu).HasMaxLength(10);
            entity.Property(e => e.SoLuong).HasDefaultValue(1);

            entity.HasOne(d => d.MaChiTietDichVuNavigation).WithMany(p => p.KhachHangDichVus)
                .HasForeignKey(d => d.MaChiTietDichVu)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__KhachHang__MaChi__2C88998B");

            entity.HasOne(d => d.MaKhachHangNavigation).WithMany(p => p.KhachHangDichVus)
                .HasForeignKey(d => d.MaKhachHang)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__KhachHang__MaKha__2B947552");
        });

        modelBuilder.Entity<KhachHangPhanThuong>(entity =>
        {
            entity.HasKey(e => new { e.MaKhachHang, e.MaPhanThuong }).HasName("PK__KhachHan__ECC2B5083C2C46A4");

            entity.ToTable("KhachHang_PhanThuong");

            entity.Property(e => e.MaKhachHang).HasMaxLength(10);
            entity.Property(e => e.MaPhanThuong).HasMaxLength(10);
            entity.Property(e => e.NgayNhan)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.SoLuong).HasDefaultValue(1);

            entity.HasOne(d => d.MaKhachHangNavigation).WithMany(p => p.KhachHangPhanThuongs)
                .HasForeignKey(d => d.MaKhachHang)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__KhachHang__MaKha__1F2E9E6D");

            entity.HasOne(d => d.MaPhanThuongNavigation).WithMany(p => p.KhachHangPhanThuongs)
                .HasForeignKey(d => d.MaPhanThuong)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__KhachHang__MaPha__2022C2A6");
        });

        modelBuilder.Entity<NhanVien>(entity =>
        {
            entity.HasKey(e => e.MaNhanVien).HasName("PK__NhanVien__77B2CA47BBC1409A");

            entity.ToTable("NhanVien");

            entity.HasIndex(e => e.SoDienThoai, "UQ__NhanVien__0389B7BDF1BB0F64").IsUnique();

            entity.HasIndex(e => e.CanCuocCongDan, "UQ__NhanVien__9A7D3DD72123CDF2").IsUnique();

            entity.Property(e => e.MaNhanVien).HasMaxLength(10);
            entity.Property(e => e.CanCuocCongDan).HasMaxLength(20);
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .HasDefaultValue("default@example.com");
            entity.Property(e => e.HinhAnh).HasMaxLength(255);
            entity.Property(e => e.HoTen).HasMaxLength(50);
            entity.Property(e => e.Jwk).HasColumnName("JWK");
            entity.Property(e => e.MatKhau)
                .HasMaxLength(255)
                .HasDefaultValue("defaultPassword");
            entity.Property(e => e.SoDienThoai).HasMaxLength(15);
            entity.Property(e => e.TenTaiKhoan)
                .HasMaxLength(50)
                .HasDefaultValue("defaultUser");
            entity.Property(e => e.TrangThai)
                .HasMaxLength(20)
                .HasDefaultValue("Active");
        });

        modelBuilder.Entity<PhanThuong>(entity =>
        {
            entity.HasKey(e => e.MaPhanThuong).HasName("PK__PhanThuo__41045ED70899273E");

            entity.ToTable("PhanThuong");

            entity.Property(e => e.MaPhanThuong).HasMaxLength(10);
            entity.Property(e => e.MoTa).HasMaxLength(255);
            entity.Property(e => e.TenPhanThuong).HasMaxLength(100);
            entity.Property(e => e.UrlHinhAnh).HasMaxLength(255);
        });

        modelBuilder.Entity<PhongWithTienNghi>(entity =>
        {
            entity.HasKey(e => e.MaPhong).HasName("PK__PhongWit__20BD5E5B0FBD7222");

            entity.ToTable("PhongWithTienNghi");

            entity.Property(e => e.MaPhong).HasMaxLength(10);
            entity.Property(e => e.GiaPhong).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.KieuGiuong).HasMaxLength(50);
            entity.Property(e => e.LoaiPhong).HasMaxLength(50);
            entity.Property(e => e.MoTa).HasMaxLength(500);
            entity.Property(e => e.SoLuongPhong).HasDefaultValue(1);
            entity.Property(e => e.TinhTrang).HasMaxLength(1);
            entity.Property(e => e.UrlAnhChinh).HasMaxLength(255);
            entity.Property(e => e.UrlAnhPhu1).HasMaxLength(255);
            entity.Property(e => e.UrlAnhPhu2).HasMaxLength(255);
        });

        modelBuilder.Entity<QuanTriVien>(entity =>
        {
            entity.HasKey(e => e.MaQuanTri).HasName("PK__QuanTriV__05FA934843830AF3");

            entity.ToTable("QuanTriVien");

            entity.HasIndex(e => e.SoDienThoai, "UQ__QuanTriV__0389B7BD93D4E474").IsUnique();

            entity.HasIndex(e => e.Email, "UQ__QuanTriV__A9D10534858E7DDA").IsUnique();

            entity.Property(e => e.MaQuanTri).HasMaxLength(10);
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.Jwk).HasColumnName("JWK");
            entity.Property(e => e.MatKhau).HasMaxLength(255);
            entity.Property(e => e.NgayCapNhat)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.NgayTao)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.SoDienThoai).HasMaxLength(15);
            entity.Property(e => e.TenAdmin).HasMaxLength(50);
            entity.Property(e => e.TenTaiKhoan).HasMaxLength(50);
        });

        modelBuilder.Entity<UserFeedback>(entity =>
        {
            entity.HasKey(e => new { e.MaKhachHang, e.MaPhong }).HasName("PK__User_Fee__2AD9250066CBCE44");

            entity.ToTable("User_Feedback");

            entity.Property(e => e.MaKhachHang).HasMaxLength(10);
            entity.Property(e => e.MaPhong).HasMaxLength(10);
            entity.Property(e => e.NgayFeedback)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.NoiDung).HasMaxLength(500);

            entity.HasOne(d => d.MaKhachHangNavigation).WithMany(p => p.UserFeedbacks)
                .HasForeignKey(d => d.MaKhachHang)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__User_Feed__MaKha__3EA749C6");

            entity.HasOne(d => d.MaPhongNavigation).WithMany(p => p.UserFeedbacks)
                .HasForeignKey(d => d.MaPhong)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__User_Feed__MaPho__3F9B6DFF");
        });

        modelBuilder.Entity<VeGiamGium>(entity =>
        {
            entity.HasKey(e => e.MaVe).HasName("PK__VeGiamGi__2725100FBF4F9705");

            entity.Property(e => e.MaVe).HasMaxLength(10);
            entity.Property(e => e.GhiChu).HasMaxLength(255);
            entity.Property(e => e.MaKhachHang).HasMaxLength(10);
            entity.Property(e => e.NgayCapNhat)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.NgayTao)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.PhanTramGiam).HasColumnType("decimal(5, 2)");
            entity.Property(e => e.SoLuong).HasDefaultValue(1);

            entity.HasOne(d => d.MaKhachHangNavigation).WithMany(p => p.VeGiamGia)
                .HasForeignKey(d => d.MaKhachHang)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_VeGiamGia_KhachHang");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
