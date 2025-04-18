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

    public virtual DbSet<NguoiDung> NguoiDungs { get; set; }

    public virtual DbSet<NhanVien> NhanViens { get; set; }

    public virtual DbSet<PhongWithTienNghi> PhongWithTienNghis { get; set; }

    public virtual DbSet<QuanTriVien> QuanTriViens { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=118.69.126.49;Database=data_QLKS_115_Nhom2;User Id=user_115_nhom2;Password=123456789;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<DatPhong>(entity =>
        {
            entity.HasKey(e => e.MaDatPhong).HasName("PK__DatPhong__6344ADEAAA3BEDF5");

            entity.Property(e => e.NgayDat).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.MaKhachHangNavigation).WithMany(p => p.DatPhongs)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__DatPhong__MaKhac__61316BF4");

            entity.HasOne(d => d.MaNhanVienNavigation).WithMany(p => p.DatPhongs)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__DatPhong__MaNhan__6225902D");

            entity.HasOne(d => d.MaPhongNavigation).WithMany(p => p.DatPhongs)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__DatPhong__MaPhon__6319B466");
        });

        modelBuilder.Entity<DichVu>(entity =>
        {
            entity.HasKey(e => e.MaChiTietDichVu).HasName("PK__DichVu__11EFCA674FE70F8A");
        });

        modelBuilder.Entity<HoaDonThanhToanDichVu>(entity =>
        {
            entity.HasKey(e => e.MaHoaDonDichVu).HasName("PK__HoaDonTh__7C7332D7FFD7C00A");

            entity.Property(e => e.NgayLapHoaDon).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.MaChiTietDichVuNavigation).WithMany(p => p.HoaDonThanhToanDichVus).HasConstraintName("FK__HoaDonTha__MaChi__4A4E069C");

            entity.HasOne(d => d.MaKhachHangNavigation).WithMany(p => p.HoaDonThanhToanDichVus).HasConstraintName("FK__HoaDonTha__MaKha__4959E263");
        });

        modelBuilder.Entity<HoaDonThanhToanPhong>(entity =>
        {
            entity.HasKey(e => e.MaHoaDon).HasName("PK__HoaDonTh__835ED13B7F3A7D0C");

            entity.Property(e => e.NgayLapHoaDon).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.MaDatPhongNavigation).WithMany(p => p.HoaDonThanhToanPhongs)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__HoaDonTha__MaDat__69C6B1F5");

            entity.HasOne(d => d.MaNhanVienNavigation).WithMany(p => p.HoaDonThanhToanPhongs)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__HoaDonTha__MaNha__6ABAD62E");
        });

        modelBuilder.Entity<KhachHang>(entity =>
        {
            entity.HasKey(e => e.MaKhachHang).HasName("PK__KhachHan__88D2F0E5C3B1F2A2");

            entity.Property(e => e.NgayCapNhat).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.NgayTao).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.TrangThai).HasDefaultValue("Active");

            entity.HasOne(d => d.MaNguoiDungNavigation).WithOne(p => p.KhachHang)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_KhachHang_NguoiDung");
        });

        modelBuilder.Entity<KhachHangDichVu>(entity =>
        {
            entity.HasKey(e => new { e.MaKhachHang, e.MaChiTietDichVu }).HasName("PK__KhachHan__E9CC0C43BDDB1F5E");

            entity.Property(e => e.SoLuong).HasDefaultValue(1);

            entity.HasOne(d => d.MaChiTietDichVuNavigation).WithMany(p => p.KhachHangDichVus)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__KhachHang__MaChi__3CF40B7E");

            entity.HasOne(d => d.MaKhachHangNavigation).WithMany(p => p.KhachHangDichVus)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__KhachHang__MaKha__3BFFE745");
        });

        modelBuilder.Entity<NguoiDung>(entity =>
        {
            entity.HasKey(e => e.MaNguoiDung).HasName("PK__NguoiDun__C539D762083D68CD");

            entity.Property(e => e.NgayCapNhat).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.NgayTao).HasDefaultValueSql("(getdate())");
        });

        modelBuilder.Entity<NhanVien>(entity =>
        {
            entity.HasKey(e => e.MaNhanVien).HasName("PK__NhanVien__77B2CA47BBC1409A");

            entity.Property(e => e.TrangThai).HasDefaultValue("Active");

            entity.HasOne(d => d.MaNguoiDungNavigation).WithOne(p => p.NhanVien)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_NhanVien_NguoiDung");
        });

        modelBuilder.Entity<PhongWithTienNghi>(entity =>
        {
            entity.HasKey(e => e.MaPhong).HasName("PK__PhongWit__20BD5E5B0FBD7222");

            entity.Property(e => e.SoLuongPhong).HasDefaultValue(1);
        });

        modelBuilder.Entity<QuanTriVien>(entity =>
        {
            entity.HasKey(e => e.MaQuanTri).HasName("PK__QuanTriV__05FA934838D55430");

            entity.Property(e => e.NgayCapNhat).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.NgayTao).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.MaNguoiDungNavigation).WithOne(p => p.QuanTriVien)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_QuanTri_NguoiDung");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
