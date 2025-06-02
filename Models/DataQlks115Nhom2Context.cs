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

    public virtual DbSet<BaiViet> BaiViets { get; set; }

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

    public virtual DbSet<NoiQuy> NoiQuies { get; set; }

    public virtual DbSet<Phong> Phongs { get; set; }

    public virtual DbSet<PhongAnh> PhongAnhs { get; set; }

    public virtual DbSet<PhongYeuThich> PhongYeuThiches { get; set; }

    public virtual DbSet<ThanhToan> ThanhToans { get; set; }

    public virtual DbSet<TienNghi> TienNghis { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=118.69.126.49;Database=data_QLKS_115_Nhom2;User Id=user_115_nhom2;Password=123456789;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<BaiViet>(entity =>
        {
            entity.HasKey(e => e.MaBaiViet).HasName("PK__BaiViet__AEDD564735FEF244");

            entity.ToTable("BaiViet");

            entity.Property(e => e.MaBaiViet).HasMaxLength(10);
            entity.Property(e => e.HinhAnhUrl)
                .HasMaxLength(255)
                .HasDefaultValueSql("(NULL)")
                .HasColumnName("HinhAnhURL");
            entity.Property(e => e.MaNguoiDung).HasMaxLength(6);
            entity.Property(e => e.NgayDang)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.NoiDung).HasColumnType("text");
            entity.Property(e => e.TieuDe).HasMaxLength(255);
            entity.Property(e => e.TrangThai)
                .HasMaxLength(20)
                .HasDefaultValue("ChoDuyet");

            entity.HasOne(d => d.MaNguoiDungNavigation).WithMany(p => p.BaiViets)
                .HasForeignKey(d => d.MaNguoiDung)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__BaiViet__MaNguoi__416EA7D8");
        });

        modelBuilder.Entity<BaoCao>(entity =>
        {
            entity.HasKey(e => e.MaBaoCao).HasName("PK__BaoCao__25A9188CFE4D38CB");

            entity.ToTable("BaoCao");

            entity.Property(e => e.MaBaoCao).HasMaxLength(6);
            entity.Property(e => e.LoaiBaoCao).HasMaxLength(50);
            entity.Property(e => e.NoiDung).HasColumnType("text");
            entity.Property(e => e.ThoiGian)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
        });

        modelBuilder.Entity<ChiTietBaoCao>(entity =>
        {
            entity.HasKey(e => e.MaChiTiet).HasName("PK__ChiTietB__CDF0A114D9662651");

            entity.ToTable("ChiTietBaoCao");

            entity.Property(e => e.MaChiTiet).HasMaxLength(6);
            entity.Property(e => e.GiaTri).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.MaBaoCao).HasMaxLength(6);
            entity.Property(e => e.NoiDungChiTiet).HasMaxLength(255);

            entity.HasOne(d => d.MaBaoCaoNavigation).WithMany(p => p.ChiTietBaoCaos)
                .HasForeignKey(d => d.MaBaoCao)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ChiTietBa__MaBao__1F198FD4");
        });

        modelBuilder.Entity<ChiTietHoaDon>(entity =>
        {
            entity.HasKey(e => e.MaChiTiet).HasName("PK__ChiTietH__CDF0A114167F4AE2");

            entity.ToTable("ChiTietHoaDon");

            entity.Property(e => e.MaChiTiet).HasMaxLength(6);
            entity.Property(e => e.LoaiKhoanMuc).HasMaxLength(12);
            entity.Property(e => e.MaHoaDon).HasMaxLength(6);
            entity.Property(e => e.MaKhoanMuc).HasMaxLength(12);
            entity.Property(e => e.SoLuong).HasDefaultValue(1);
            entity.Property(e => e.ThanhTien).HasColumnType("decimal(18, 2)");

            entity.HasOne(d => d.MaHoaDonNavigation).WithMany(p => p.ChiTietHoaDons)
                .HasForeignKey(d => d.MaHoaDon)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ChiTietHo__MaHoa__07420643");
        });

        modelBuilder.Entity<DatDichVu>(entity =>
        {
            entity.HasKey(e => e.MaDatDichVu).HasName("PK__DatDichV__35B4F60ADECC2C14");

            entity.ToTable("DatDichVu");

            entity.Property(e => e.MaDatDichVu).HasMaxLength(6);
            entity.Property(e => e.MaDatPhong).HasMaxLength(6);
            entity.Property(e => e.MaDichVu).HasMaxLength(6);
            entity.Property(e => e.SoLuong).HasDefaultValue(1);

            entity.HasOne(d => d.MaDatPhongNavigation).WithMany(p => p.DatDichVus)
                .HasForeignKey(d => d.MaDatPhong)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__DatDichVu__MaDat__01892CED");

            entity.HasOne(d => d.MaDichVuNavigation).WithMany(p => p.DatDichVus)
                .HasForeignKey(d => d.MaDichVu)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__DatDichVu__MaDic__027D5126");
        });

        modelBuilder.Entity<DatPhong>(entity =>
        {
            entity.HasKey(e => e.MaDatPhong).HasName("PK__DatPhong__6344ADEAE1DFB5FB");

            entity.ToTable("DatPhong");

            entity.Property(e => e.MaDatPhong).HasMaxLength(6);
            entity.Property(e => e.MaNguoiDung).HasMaxLength(6);
            entity.Property(e => e.MaPhong).HasMaxLength(6);
            entity.Property(e => e.NgayCheckIn).HasColumnType("datetime");
            entity.Property(e => e.NgayCheckOut).HasColumnType("datetime");
            entity.Property(e => e.NgayDat)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.MaNguoiDungNavigation).WithMany(p => p.DatPhongs)
                .HasForeignKey(d => d.MaNguoiDung)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__DatPhong__MaNguo__7152C524");

            entity.HasOne(d => d.MaPhongNavigation).WithMany(p => p.DatPhongs)
                .HasForeignKey(d => d.MaPhong)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__DatPhong__MaPhon__7246E95D");
        });

        modelBuilder.Entity<DichVu>(entity =>
        {
            entity.HasKey(e => e.MaDichVu).HasName("PK__DichVu__C0E6DE8F7A8A9E20");

            entity.ToTable("DichVu");

            entity.Property(e => e.MaDichVu).HasMaxLength(6);
            entity.Property(e => e.DonGia).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.DonViTinh)
                .HasMaxLength(50)
                .HasDefaultValue("1 l?n");
            entity.Property(e => e.HinhAnhDichVu).HasMaxLength(255);
            entity.Property(e => e.LoaiDichVu)
                .HasMaxLength(50)
                .HasDefaultValue("Khac");
            entity.Property(e => e.MoTaDichVu).HasMaxLength(300);
            entity.Property(e => e.TenDichVu).HasMaxLength(100);
            entity.Property(e => e.TrangThai).HasDefaultValue((byte)1);
        });

        modelBuilder.Entity<Feedback>(entity =>
        {
            entity.HasKey(e => e.MaFeedback).HasName("PK__Feedback__63042CF68D0745C0");

            entity.ToTable("Feedback");

            entity.Property(e => e.MaFeedback).HasMaxLength(6);
            entity.Property(e => e.BinhLuan).HasMaxLength(500);
            entity.Property(e => e.MaNguoiDung).HasMaxLength(6);
            entity.Property(e => e.MaPhong).HasMaxLength(6);
            entity.Property(e => e.NgayFeedback)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.PhanLoai).HasMaxLength(10);

            entity.HasOne(d => d.MaNguoiDungNavigation).WithMany(p => p.Feedbacks)
                .HasForeignKey(d => d.MaNguoiDung)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Feedback__MaNguo__11BF94B6");

            entity.HasOne(d => d.MaPhongNavigation).WithMany(p => p.Feedbacks)
                .HasForeignKey(d => d.MaPhong)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Feedback__MaPhon__10CB707D");
        });

        modelBuilder.Entity<GiamGium>(entity =>
        {
            entity.HasKey(e => e.MaGiamGia).HasName("PK__GiamGia__EF9458E4A95B060A");

            entity.Property(e => e.MaGiamGia).HasMaxLength(6);
            entity.Property(e => e.GiaTriGiam).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.LoaiGiamGia).HasMaxLength(10);
            entity.Property(e => e.MoTa).HasMaxLength(255);
            entity.Property(e => e.NgayBatDau).HasColumnType("datetime");
            entity.Property(e => e.NgayKetThuc).HasColumnType("datetime");
            entity.Property(e => e.TenGiamGia).HasMaxLength(50);
        });

        modelBuilder.Entity<HoaDon>(entity =>
        {
            entity.HasKey(e => e.MaHoaDon).HasName("PK__HoaDon__835ED13B75D2C79D");

            entity.ToTable("HoaDon");

            entity.Property(e => e.MaHoaDon).HasMaxLength(6);
            entity.Property(e => e.MaDatPhong).HasMaxLength(6);
            entity.Property(e => e.MaNguoiDung).HasMaxLength(6);
            entity.Property(e => e.NgayTaoHoaDon)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.NgayThanhToan).HasColumnType("datetime");
            entity.Property(e => e.TongTien).HasColumnType("decimal(18, 2)");

            entity.HasOne(d => d.MaDatPhongNavigation).WithMany(p => p.HoaDons)
                .HasForeignKey(d => d.MaDatPhong)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__HoaDon__MaDatPho__7DB89C09");

            entity.HasOne(d => d.MaNguoiDungNavigation).WithMany(p => p.HoaDons)
                .HasForeignKey(d => d.MaNguoiDung)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__HoaDon__MaNguoiD__7CC477D0");
        });

        modelBuilder.Entity<LichSuGiaoDich>(entity =>
        {
            entity.HasKey(e => e.MaGiaoDich).HasName("PK__LichSuGi__0A2A24EBEB8C6320");

            entity.ToTable("LichSuGiaoDich");

            entity.Property(e => e.MaGiaoDich).HasMaxLength(6);
            entity.Property(e => e.LoaiGiaoDich).HasMaxLength(50);
            entity.Property(e => e.MaNguoiDung).HasMaxLength(6);
            entity.Property(e => e.MoTa).HasMaxLength(255);
            entity.Property(e => e.ThoiGianGiaoDich)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.MaNguoiDungNavigation).WithMany(p => p.LichSuGiaoDiches)
                .HasForeignKey(d => d.MaNguoiDung)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__LichSuGia__MaNgu__1960B67E");
        });

        modelBuilder.Entity<NguoiDung>(entity =>
        {
            entity.HasKey(e => e.MaNguoiDung).HasName("PK__NguoiDun__C539D76271D1E9B0");

            entity.ToTable("NguoiDung");

            entity.HasIndex(e => e.CanCuocCongDan, "UQ_CanCuocCongDan").IsUnique();

            entity.HasIndex(e => e.SoDienThoai, "UQ_SoDienThoai").IsUnique();

            entity.HasIndex(e => e.Email, "UQ__NguoiDun__A9D10534910DC410").IsUnique();

            entity.Property(e => e.MaNguoiDung).HasMaxLength(6);
            entity.Property(e => e.CanCuocCongDan).HasMaxLength(60);
            entity.Property(e => e.DiaChi).HasMaxLength(100);
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.GioiTinh).HasMaxLength(10);
            entity.Property(e => e.HinhAnhUrl)
                .HasMaxLength(255)
                .HasDefaultValue("https://i.pinimg.com/736x/20/ef/6b/20ef6b554ea249790281e6677abc4160.jpg")
                .HasColumnName("HinhAnhURL");
            entity.Property(e => e.HoTen).HasMaxLength(50);
            entity.Property(e => e.MatKhau).HasMaxLength(255);
            entity.Property(e => e.NgayTao)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.SoDienThoai).HasMaxLength(15);
            entity.Property(e => e.TenTaiKhoan).HasMaxLength(50);
            entity.Property(e => e.Vaitro)
                .HasMaxLength(15)
                .HasDefaultValue("KhachHang");
        });

        modelBuilder.Entity<NoiQuy>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__NoiQuy__3213E83FDCC090D1");

            entity.ToTable("NoiQuy");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.HinhAnh)
                .HasMaxLength(500)
                .HasColumnName("hinhAnh");
            entity.Property(e => e.NoiDung).HasColumnName("noiDung");
            entity.Property(e => e.SoThuTu).HasColumnName("soThuTu");
            entity.Property(e => e.TieuDe)
                .HasMaxLength(255)
                .HasColumnName("tieuDe");
        });

        modelBuilder.Entity<Phong>(entity =>
        {
            entity.HasKey(e => e.MaPhong).HasName("PK__Phong__20BD5E5B3E27A9BD");

            entity.ToTable("Phong");

            entity.Property(e => e.MaPhong).HasMaxLength(6);
            entity.Property(e => e.DonViTinh)
                .HasMaxLength(50)
                .HasDefaultValue("1 dêm");
            entity.Property(e => e.GiaPhong).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.KieuGiuong).HasMaxLength(50);
            entity.Property(e => e.LoaiPhong).HasMaxLength(50);
            entity.Property(e => e.MoTa).HasMaxLength(500);
            entity.Property(e => e.SoGiuong).HasDefaultValue(1);
            entity.Property(e => e.SoLuongPhong).HasDefaultValue(1);
            entity.Property(e => e.SoSaoTrungBinh).HasColumnType("decimal(3, 2)");
            entity.Property(e => e.SucChua).HasDefaultValue(2);
            entity.Property(e => e.UrlAnhChinh).HasMaxLength(255);

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

            entity.ToTable("PhongAnh");

            entity.Property(e => e.MaAnh).HasMaxLength(6);
            entity.Property(e => e.MaPhong).HasMaxLength(6);
            entity.Property(e => e.UrlAnh).HasMaxLength(255);

            entity.HasOne(d => d.MaPhongNavigation).WithMany(p => p.PhongAnhs)
                .HasForeignKey(d => d.MaPhong)
                .HasConstraintName("FK__PhongAnh__MaPhon__5887175A");
        });

        modelBuilder.Entity<PhongYeuThich>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__PhongYeu__3214EC0736C827D4");

            entity.ToTable("PhongYeuThich");

            entity.HasIndex(e => new { e.MaPhong, e.MaNguoiDung }, "UQ_PhongYeuThich").IsUnique();

            entity.Property(e => e.MaNguoiDung).HasMaxLength(6);
            entity.Property(e => e.MaPhong).HasMaxLength(6);
            entity.Property(e => e.NgayYeuThich)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.MaNguoiDungNavigation).WithMany(p => p.PhongYeuThiches)
                .HasForeignKey(d => d.MaNguoiDung)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PhongYeuThich_NguoiDung");

            entity.HasOne(d => d.MaPhongNavigation).WithMany(p => p.PhongYeuThiches)
                .HasForeignKey(d => d.MaPhong)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PhongYeuThich_Phong");
        });

        modelBuilder.Entity<ThanhToan>(entity =>
        {
            entity.HasKey(e => e.MaThanhToan).HasName("PK__ThanhToa__D4B2584443D246C5");

            entity.ToTable("ThanhToan");

            entity.Property(e => e.MaThanhToan).HasMaxLength(6);
            entity.Property(e => e.MaHoaDon).HasMaxLength(6);
            entity.Property(e => e.NgayThanhToan)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.PhuongThucThanhToan).HasMaxLength(50);
            entity.Property(e => e.SoTienThanhToan).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.TinhTrangThanhToan).HasMaxLength(1);

            entity.HasOne(d => d.MaHoaDonNavigation).WithMany(p => p.ThanhToans)
                .HasForeignKey(d => d.MaHoaDon)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ThanhToan__MaHoa__0C06BB60");
        });

        modelBuilder.Entity<TienNghi>(entity =>
        {
            entity.HasKey(e => e.MaTienNghi).HasName("PK__TienNghi__ED7B8F4D1BA362DD");

            entity.ToTable("TienNghi");

            entity.Property(e => e.MaTienNghi).HasMaxLength(6);
            entity.Property(e => e.MoTa).HasMaxLength(500);
            entity.Property(e => e.TenTienNghi).HasMaxLength(100);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
