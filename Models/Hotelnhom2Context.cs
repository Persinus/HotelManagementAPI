using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace HotelManagementAPI.Models;

public partial class Hotelnhom2Context : DbContext
{
    public Hotelnhom2Context()
    {
    }

    public Hotelnhom2Context(DbContextOptions<Hotelnhom2Context> options)
        : base(options)
    {
    }

    public virtual DbSet<BaoCao> BaoCaos { get; set; }

    public virtual DbSet<ChiTietBaoCao> ChiTietBaoCaos { get; set; }

    public virtual DbSet<ChiTietHoaDon> ChiTietHoaDons { get; set; }

    public virtual DbSet<DatDichVu> DatDichVus { get; set; }

    public virtual DbSet<DatPhong> DatPhongs { get; set; }

    public virtual DbSet<DichVu> DichVus { get; set; }

    public virtual DbSet<Feedback> Feedbacks { get; set; }

    public virtual DbSet<GiamGium> GiamGia { get; set; }

    public virtual DbSet<HoaDon> HoaDons { get; set; }

    public virtual DbSet<LichSuGiaoDich> LichSuGiaoDiches { get; set; }

    public virtual DbSet<NguoiDung> NguoiDungs { get; set; }

   

    public virtual DbSet<Phong> Phongs { get; set; }

    public virtual DbSet<PhongAnh> PhongAnhs { get; set; }

    public virtual DbSet<ThanhToan> ThanhToans { get; set; }

    public virtual DbSet<TienNghi> TienNghis { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=118.69.126.49;Database=data_QLKS_115_Nhom2;User Id=user_115_nhom2;Password=123456789;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<BaoCao>(entity =>
        {
            entity.HasKey(e => e.MaBaoCao).HasName("PK__BaoCao__25A9188CFE4D38CB");

            entity.Property(e => e.ThoiGian).HasDefaultValueSql("(getdate())");
        });

        modelBuilder.Entity<ChiTietBaoCao>(entity =>
        {
            entity.HasKey(e => e.MaChiTiet).HasName("PK__ChiTietB__CDF0A114D9662651");

            entity.HasOne(d => d.MaBaoCaoNavigation).WithMany(p => p.ChiTietBaoCaos)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ChiTietBa__MaBao__1F198FD4");
        });

        modelBuilder.Entity<ChiTietHoaDon>(entity =>
        {
            entity.HasKey(e => e.MaChiTiet).HasName("PK__ChiTietH__CDF0A114167F4AE2");

            entity.Property(e => e.SoLuong).HasDefaultValue(1);

            entity.HasOne(d => d.MaHoaDonNavigation).WithMany(p => p.ChiTietHoaDons)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ChiTietHo__MaHoa__07420643");
        });

        modelBuilder.Entity<DatDichVu>(entity =>
        {
            entity.HasKey(e => e.MaDatDichVu).HasName("PK__DatDichV__35B4F60ADECC2C14");

            entity.Property(e => e.SoLuong).HasDefaultValue(1);

            entity.HasOne(d => d.MaDatPhongNavigation).WithMany(p => p.DatDichVus)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__DatDichVu__MaDat__01892CED");

            entity.HasOne(d => d.MaDichVuNavigation).WithMany(p => p.DatDichVus)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__DatDichVu__MaDic__027D5126");

            entity.HasOne(d => d.MaHoaDonNavigation).WithMany(p => p.DatDichVus).HasConstraintName("FK__DatDichVu__MaHoa__0371755F");
        });

        modelBuilder.Entity<DatPhong>(entity =>
        {
            entity.HasKey(e => e.MaDatPhong).HasName("PK__DatPhong__6344ADEAE1DFB5FB");

            entity.Property(e => e.NgayDat).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.MaNguoiDungNavigation).WithMany(p => p.DatPhongs)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__DatPhong__MaNguo__7152C524");

            entity.HasOne(d => d.MaPhongNavigation).WithMany(p => p.DatPhongs)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__DatPhong__MaPhon__7246E95D");
        });

        modelBuilder.Entity<DichVu>(entity =>
        {
            entity.HasKey(e => e.MaDichVu).HasName("PK__DichVu__C0E6DE8F7A8A9E20");

            entity.Property(e => e.DonViTinh).HasDefaultValue("1 l?n");
            entity.Property(e => e.LoaiDichVu).HasDefaultValue("Khac");
            entity.Property(e => e.TrangThai).HasDefaultValue((byte)1);
        });

       modelBuilder.Entity<Feedback>(entity =>
{
    entity.HasKey(e => e.MaFeedback).HasName("PK__Feedback__63042CF68D0745C0");

    entity.Property(e => e.NgayFeedback).HasDefaultValueSql("(getdate())");

    entity.Property(e => e.PhanLoai)
        .HasMaxLength(10)
        .IsRequired()
        .HasColumnName("PhanLoai");

    entity.HasOne(d => d.MaNguoiDungNavigation)
        .WithMany(p => p.Feedbacks)
        .OnDelete(DeleteBehavior.ClientSetNull)
        .HasConstraintName("FK__Feedback__MaNguo__11BF94B6");

    entity.HasOne(d => d.MaPhongNavigation)
        .WithMany(p => p.Feedbacks)
        .OnDelete(DeleteBehavior.ClientSetNull)
        .HasConstraintName("FK__Feedback__MaPhon__10CB707D");
});

        modelBuilder.Entity<GiamGium>(entity =>
        {
            entity.HasKey(e => e.MaGiamGia).HasName("PK__GiamGia__EF9458E4A95B060A");
        });

        modelBuilder.Entity<HoaDon>(entity =>
        {
            entity.HasKey(e => e.MaHoaDon).HasName("PK__HoaDon__835ED13B75D2C79D");

            entity.Property(e => e.NgayTaoHoaDon).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.MaDatPhongNavigation).WithMany(p => p.HoaDons)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__HoaDon__MaDatPho__7DB89C09");

            entity.HasOne(d => d.MaNguoiDungNavigation).WithMany(p => p.HoaDons)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__HoaDon__MaNguoiD__7CC477D0");
        });

        modelBuilder.Entity<LichSuGiaoDich>(entity =>
        {
            entity.HasKey(e => e.MaGiaoDich).HasName("PK__LichSuGi__0A2A24EBEB8C6320");

            entity.Property(e => e.ThoiGianGiaoDich).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.MaNguoiDungNavigation).WithMany(p => p.LichSuGiaoDiches)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__LichSuGia__MaNgu__1960B67E");
        });

        modelBuilder.Entity<NguoiDung>(entity =>
        {
            entity.HasKey(e => e.MaNguoiDung).HasName("PK__NguoiDun__C539D76271D1E9B0");

            entity.Property(e => e.HinhAnhUrl).HasDefaultValue("https://i.pinimg.com/736x/20/ef/6b/20ef6b554ea249790281e6677abc4160.jpg");
            entity.Property(e => e.NgayTao).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.Vaitro).HasDefaultValue("KhachHang");
        });

      
        modelBuilder.Entity<Phong>(entity =>
        {
            entity.HasKey(e => e.MaPhong).HasName("PK__Phong__20BD5E5B3E27A9BD");

            entity.Property(e => e.DonViTinh).HasDefaultValue("1 dêm");
            entity.Property(e => e.SoGiuong).HasDefaultValue(1);
            entity.Property(e => e.SoLuongPhong).HasDefaultValue(1);
            entity.Property(e => e.SucChua).HasDefaultValue(2);

            entity.HasMany(d => d.MaGiamGia).WithMany(p => p.MaPhongs)
                .UsingEntity<Dictionary<string, object>>(
                    "PhongGiamGium",
                    r => r.HasOne<GiamGium>().WithMany()
                        .HasForeignKey("MaGiamGia")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__Phong_Gia__MaGia__64ECEE3F"),
                    l => l.HasOne<Phong>().WithMany()
                        .HasForeignKey("MaPhong")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__Phong_Gia__MaPho__63F8CA06"),
                    j =>
                    {
                        j.HasKey("MaPhong", "MaGiamGia").HasName("PK__Phong_Gi__7E441BD5C7C12056");
                        j.ToTable("Phong_GiamGia");
                        j.IndexerProperty<string>("MaPhong").HasMaxLength(6);
                        j.IndexerProperty<string>("MaGiamGia").HasMaxLength(6);
                    });

            entity.HasMany(d => d.MaTienNghis).WithMany(p => p.MaPhongs)
                .UsingEntity<Dictionary<string, object>>(
                    "PhongTienNghi",
                    r => r.HasOne<TienNghi>().WithMany()
                        .HasForeignKey("MaTienNghi")
                        .HasConstraintName("FK__Phong_Tie__MaTie__5E3FF0B0"),
                    l => l.HasOne<Phong>().WithMany()
                        .HasForeignKey("MaPhong")
                        .HasConstraintName("FK__Phong_Tie__MaPho__5D4BCC77"),
                    j =>
                    {
                        j.HasKey("MaPhong", "MaTienNghi").HasName("PK__Phong_Ti__EE6AE6AF744EE6E8");
                        j.ToTable("Phong_TienNghi");
                        j.IndexerProperty<string>("MaPhong").HasMaxLength(6);
                        j.IndexerProperty<string>("MaTienNghi").HasMaxLength(6);
                    });
        });

        modelBuilder.Entity<PhongAnh>(entity =>
        {
            entity.HasKey(e => e.MaAnh).HasName("PK__PhongAnh__356240DFA45F6A84");

            entity.HasOne(d => d.MaPhongNavigation).WithMany(p => p.PhongAnhs).HasConstraintName("FK__PhongAnh__MaPhon__5887175A");
        });

        modelBuilder.Entity<ThanhToan>(entity =>
        {
            entity.HasKey(e => e.MaThanhToan).HasName("PK__ThanhToa__D4B2584443D246C5");

            entity.Property(e => e.NgayThanhToan).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.MaHoaDonNavigation).WithMany(p => p.ThanhToans)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ThanhToan__MaHoa__0C06BB60");
        });

        modelBuilder.Entity<TienNghi>(entity =>
        {
            entity.HasKey(e => e.MaTienNghi).HasName("PK__TienNghi__ED7B8F4D1BA362DD");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
