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

    public virtual DbSet<LichSuTinhTrangPhong> LichSuTinhTrangPhongs { get; set; }

    public virtual DbSet<NguoiDung> NguoiDungs { get; set; }

    public virtual DbSet<NhanVien> NhanViens { get; set; }

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
            entity.HasKey(e => e.MaBaoCao).HasName("PK__BaoCao__25A9188C7C1CC3B7");

            entity.ToTable("BaoCao");

            entity.Property(e => e.MaBaoCao).HasMaxLength(20);
            entity.Property(e => e.LoaiBaoCao).HasMaxLength(50);
            entity.Property(e => e.NoiDung).HasColumnType("text");
            entity.Property(e => e.ThoiGian)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
        });

        modelBuilder.Entity<ChiTietBaoCao>(entity =>
        {
            entity.HasKey(e => e.MaChiTiet).HasName("PK__ChiTietB__CDF0A11496F0E597");

            entity.ToTable("ChiTietBaoCao");

            entity.Property(e => e.MaChiTiet).HasMaxLength(20);
            entity.Property(e => e.GiaTri).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.MaBaoCao).HasMaxLength(20);
            entity.Property(e => e.NoiDungChiTiet).HasMaxLength(255);

            entity.HasOne(d => d.MaBaoCaoNavigation).WithMany(p => p.ChiTietBaoCaos)
                .HasForeignKey(d => d.MaBaoCao)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ChiTietBa__MaBao__23BE4960");
        });

        modelBuilder.Entity<ChiTietHoaDon>(entity =>
        {
            entity.HasKey(e => e.MaChiTiet).HasName("PK__ChiTietH__CDF0A114BB0C0EA3");

            entity.ToTable("ChiTietHoaDon");

            entity.Property(e => e.MaChiTiet).HasMaxLength(20);
            entity.Property(e => e.LoaiKhoanMuc).HasMaxLength(20);
            entity.Property(e => e.MaHoaDon).HasMaxLength(20);
            entity.Property(e => e.MaKhoanMuc).HasMaxLength(20);
            entity.Property(e => e.SoLuong).HasDefaultValue(1);
            entity.Property(e => e.ThanhTien).HasColumnType("decimal(18, 2)");

            entity.HasOne(d => d.MaHoaDonNavigation).WithMany(p => p.ChiTietHoaDons)
                .HasForeignKey(d => d.MaHoaDon)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ChiTietHo__MaHoa__0BE6BFCF");
        });

        modelBuilder.Entity<DatDichVu>(entity =>
        {
            entity.HasKey(e => e.MaDatDichVu).HasName("PK__DatDichV__35B4F60A850EE2A9");

            entity.ToTable("DatDichVu");

            entity.Property(e => e.MaDatDichVu).HasMaxLength(20);
            entity.Property(e => e.MaDatPhong).HasMaxLength(20);
            entity.Property(e => e.MaDichVu).HasMaxLength(20);
            entity.Property(e => e.MaHoaDon).HasMaxLength(20);
            entity.Property(e => e.SoLuong).HasDefaultValue(1);

            entity.HasOne(d => d.MaDatPhongNavigation).WithMany(p => p.DatDichVus)
                .HasForeignKey(d => d.MaDatPhong)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__DatDichVu__MaDat__062DE679");

            entity.HasOne(d => d.MaDichVuNavigation).WithMany(p => p.DatDichVus)
                .HasForeignKey(d => d.MaDichVu)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__DatDichVu__MaDic__07220AB2");

            entity.HasOne(d => d.MaHoaDonNavigation).WithMany(p => p.DatDichVus)
                .HasForeignKey(d => d.MaHoaDon)
                .HasConstraintName("FK__DatDichVu__MaHoa__08162EEB");
        });

        modelBuilder.Entity<DatPhong>(entity =>
        {
            entity.HasKey(e => e.MaDatPhong).HasName("PK__DatPhong__6344ADEADC29C850");

            entity.ToTable("DatPhong");

            entity.Property(e => e.MaDatPhong).HasMaxLength(20);
            entity.Property(e => e.MaNguoiDung).HasMaxLength(20);
            entity.Property(e => e.MaPhong).HasMaxLength(20);
            entity.Property(e => e.NgayCheckIn).HasColumnType("datetime");
            entity.Property(e => e.NgayCheckOut).HasColumnType("datetime");
            entity.Property(e => e.NgayDat)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.TinhTrangDatPhong).HasMaxLength(1);

            entity.HasOne(d => d.MaNguoiDungNavigation).WithMany(p => p.DatPhongs)
                .HasForeignKey(d => d.MaNguoiDung)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__DatPhong__MaNguo__75F77EB0");

            entity.HasOne(d => d.MaPhongNavigation).WithMany(p => p.DatPhongs)
                .HasForeignKey(d => d.MaPhong)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__DatPhong__MaPhon__76EBA2E9");
        });

        modelBuilder.Entity<DichVu>(entity =>
        {
            entity.HasKey(e => e.MaDichVu).HasName("PK__DichVu__C0E6DE8F21EB9074");

            entity.ToTable("DichVu");

            entity.Property(e => e.MaDichVu).HasMaxLength(20);
            entity.Property(e => e.DonGia).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.DonViTinh)
                .HasMaxLength(50)
                .HasDefaultValue("1 l?n");
            entity.Property(e => e.LoaiDichVu)
                .HasMaxLength(50)
                .HasDefaultValue("Khac");
            entity.Property(e => e.MoTaDichVu).HasMaxLength(300);
            entity.Property(e => e.TenDichVu).HasMaxLength(100);
            entity.Property(e => e.TrangThai).HasDefaultValue(1);
        });

        modelBuilder.Entity<Feedback>(entity =>
        {
            entity.HasKey(e => e.MaFeedback).HasName("PK__Feedback__63042CF6BCE611B0");

            entity.ToTable("Feedback");

            entity.Property(e => e.MaFeedback).HasMaxLength(20);
            entity.Property(e => e.BinhLuan).HasMaxLength(1000);
            entity.Property(e => e.MaNguoiDung).HasMaxLength(20);
            entity.Property(e => e.MaPhong).HasMaxLength(20);
            entity.Property(e => e.NgayFeedback)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.MaNguoiDungNavigation).WithMany(p => p.Feedbacks)
                .HasForeignKey(d => d.MaNguoiDung)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Feedback__MaNguo__16644E42");

            entity.HasOne(d => d.MaPhongNavigation).WithMany(p => p.Feedbacks)
                .HasForeignKey(d => d.MaPhong)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Feedback__MaPhon__15702A09");
        });

        modelBuilder.Entity<GiamGium>(entity =>
        {
            entity.HasKey(e => e.MaGiamGia).HasName("PK__GiamGia__EF9458E4D1853A2D");

            entity.Property(e => e.MaGiamGia).HasMaxLength(20);
            entity.Property(e => e.GiaTriGiam).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.LoaiGiamGia).HasMaxLength(20);
            entity.Property(e => e.MoTa).HasMaxLength(255);
            entity.Property(e => e.NgayBatDau).HasColumnType("datetime");
            entity.Property(e => e.NgayKetThuc).HasColumnType("datetime");
            entity.Property(e => e.TenGiamGia).HasMaxLength(50);
        });

        modelBuilder.Entity<HoaDon>(entity =>
        {
            entity.HasKey(e => e.MaHoaDon).HasName("PK__HoaDon__835ED13B225A21B9");

            entity.ToTable("HoaDon");

            entity.Property(e => e.MaHoaDon).HasMaxLength(20);
            entity.Property(e => e.MaDatPhong).HasMaxLength(20);
            entity.Property(e => e.MaNguoiDung).HasMaxLength(20);
            entity.Property(e => e.NgayTaoHoaDon)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.NgayThanhToan).HasColumnType("datetime");
            entity.Property(e => e.TinhTrangHoaDon).HasMaxLength(1);
            entity.Property(e => e.TongTien).HasColumnType("decimal(18, 2)");

            entity.HasOne(d => d.MaDatPhongNavigation).WithMany(p => p.HoaDons)
                .HasForeignKey(d => d.MaDatPhong)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__HoaDon__MaDatPho__025D5595");

            entity.HasOne(d => d.MaNguoiDungNavigation).WithMany(p => p.HoaDons)
                .HasForeignKey(d => d.MaNguoiDung)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__HoaDon__MaNguoiD__0169315C");
        });

        modelBuilder.Entity<LichSuGiaoDich>(entity =>
        {
            entity.HasKey(e => e.MaGiaoDich).HasName("PK__LichSuGi__0A2A24EB6884E4BC");

            entity.ToTable("LichSuGiaoDich");

            entity.Property(e => e.MaGiaoDich).HasMaxLength(20);
            entity.Property(e => e.LoaiGiaoDich).HasMaxLength(50);
            entity.Property(e => e.MaNguoiDung).HasMaxLength(20);
            entity.Property(e => e.MoTa).HasMaxLength(255);
            entity.Property(e => e.ThoiGianGiaoDich)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.MaNguoiDungNavigation).WithMany(p => p.LichSuGiaoDiches)
                .HasForeignKey(d => d.MaNguoiDung)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__LichSuGia__MaNgu__1E05700A");
        });

        modelBuilder.Entity<LichSuTinhTrangPhong>(entity =>
        {
            entity.HasKey(e => e.MaLichSu).HasName("PK__LichSuTi__C443222A325F3850");

            entity.ToTable("LichSuTinhTrangPhong");

            entity.Property(e => e.MaLichSu).HasMaxLength(20);
            entity.Property(e => e.MaPhong).HasMaxLength(20);
            entity.Property(e => e.ThoiGianThayDoi)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.TinhTrangCu).HasMaxLength(1);
            entity.Property(e => e.TinhTrangMoi).HasMaxLength(1);

            entity.HasOne(d => d.MaPhongNavigation).WithMany(p => p.LichSuTinhTrangPhongs)
                .HasForeignKey(d => d.MaPhong)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__LichSuTin__MaPho__278EDA44");
        });

        modelBuilder.Entity<NguoiDung>(entity =>
        {
            entity.HasKey(e => e.MaNguoiDung).HasName("PK__NguoiDun__C539D76281BD516E");

            entity.ToTable("NguoiDung");

            entity.HasIndex(e => e.SoDienThoai, "UQ__NguoiDun__0389B7BD6D57A484").IsUnique();

            entity.HasIndex(e => e.CanCuocCongDan, "UQ__NguoiDun__9A7D3DD7C9F74477").IsUnique();

            entity.HasIndex(e => e.Email, "UQ__NguoiDun__A9D10534B5A02BE4").IsUnique();

            entity.Property(e => e.MaNguoiDung).HasMaxLength(20);
            entity.Property(e => e.CanCuocCongDan).HasMaxLength(20);
            entity.Property(e => e.DiaChi).HasMaxLength(100);
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.GioiTinh).HasMaxLength(10);
            entity.Property(e => e.HinhAnhUrl)
                .HasMaxLength(255)
                .HasDefaultValue("https://i.pinimg.com/736x/20/ef/6b/20ef6b554ea249790281e6677abc4160.jpg")
                .HasColumnName("HinhAnhURL");
            entity.Property(e => e.HoTen).HasMaxLength(50);
            entity.Property(e => e.LoaiDangNhap)
                .HasMaxLength(20)
                .HasDefaultValue("ThuCong");
            entity.Property(e => e.MatKhau).HasMaxLength(255);
            entity.Property(e => e.NgayTao)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.SoDienThoai).HasMaxLength(15);
            entity.Property(e => e.TenTaiKhoan).HasMaxLength(50);
            entity.Property(e => e.Vaitro)
                .HasMaxLength(20)
                .HasDefaultValue("KhachHang");
        });

        modelBuilder.Entity<NhanVien>(entity =>
        {
            entity.HasKey(e => e.MaNhanVien).HasName("PK__NhanVien__77B2CA473710AD55");

            entity.ToTable("NhanVien");

            entity.Property(e => e.MaNhanVien).HasMaxLength(20);
            entity.Property(e => e.CaLamViec).HasMaxLength(50);
            entity.Property(e => e.ChucVu).HasMaxLength(50);
            entity.Property(e => e.Luong).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.MaNguoiDung).HasMaxLength(20);
            entity.Property(e => e.NgayVaoLam)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.MaNguoiDungNavigation).WithMany(p => p.NhanViens)
                .HasForeignKey(d => d.MaNguoiDung)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__NhanVien__MaNguo__1A34DF26");
        });

        modelBuilder.Entity<Phong>(entity =>
        {
            entity.HasKey(e => e.MaPhong).HasName("PK__Phong__20BD5E5B28FE730A");

            entity.ToTable("Phong");

            entity.Property(e => e.MaPhong).HasMaxLength(20);
            entity.Property(e => e.DonViTinh)
                .HasMaxLength(50)
                .HasDefaultValue("1 dêm");
            entity.Property(e => e.GiaPhong).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.KieuGiuong).HasMaxLength(50);
            entity.Property(e => e.LoaiPhong).HasMaxLength(50);
            entity.Property(e => e.MoTa).HasMaxLength(500);
            entity.Property(e => e.MotaPhong).HasColumnType("text");
            entity.Property(e => e.SoGiuong).HasDefaultValue(1);
            entity.Property(e => e.SoLuongPhong).HasDefaultValue(1);
            entity.Property(e => e.SoSaoTrungBinh).HasColumnType("decimal(3, 2)");
            entity.Property(e => e.SucChua).HasDefaultValue(2);
            entity.Property(e => e.TinhTrang).HasMaxLength(1);
            entity.Property(e => e.UrlAnhChinh).HasMaxLength(255);

            entity.HasMany(d => d.MaGiamGia).WithMany(p => p.MaPhongs)
                .UsingEntity<Dictionary<string, object>>(
                    "PhongGiamGium",
                    r => r.HasOne<GiamGium>().WithMany()
                        .HasForeignKey("MaGiamGia")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__Phong_Gia__MaGia__6991A7CB"),
                    l => l.HasOne<Phong>().WithMany()
                        .HasForeignKey("MaPhong")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__Phong_Gia__MaPho__689D8392"),
                    j =>
                    {
                        j.HasKey("MaPhong", "MaGiamGia").HasName("PK__Phong_Gi__7E441BD534414CDE");
                        j.ToTable("Phong_GiamGia");
                        j.IndexerProperty<string>("MaPhong").HasMaxLength(20);
                        j.IndexerProperty<string>("MaGiamGia").HasMaxLength(20);
                    });

            entity.HasMany(d => d.MaTienNghis).WithMany(p => p.MaPhongs)
                .UsingEntity<Dictionary<string, object>>(
                    "PhongTienNghi",
                    r => r.HasOne<TienNghi>().WithMany()
                        .HasForeignKey("MaTienNghi")
                        .HasConstraintName("FK__Phong_Tie__MaTie__62E4AA3C"),
                    l => l.HasOne<Phong>().WithMany()
                        .HasForeignKey("MaPhong")
                        .HasConstraintName("FK__Phong_Tie__MaPho__61F08603"),
                    j =>
                    {
                        j.HasKey("MaPhong", "MaTienNghi").HasName("PK__Phong_Ti__EE6AE6AF3FD912E3");
                        j.ToTable("Phong_TienNghi");
                        j.IndexerProperty<string>("MaPhong").HasMaxLength(20);
                        j.IndexerProperty<string>("MaTienNghi").HasMaxLength(20);
                    });
        });

        modelBuilder.Entity<PhongAnh>(entity =>
        {
            entity.HasKey(e => e.MaAnh).HasName("PK__PhongAnh__356240DFEE68591A");

            entity.ToTable("PhongAnh");

            entity.Property(e => e.MaPhong).HasMaxLength(20);
            entity.Property(e => e.UrlAnh).HasMaxLength(255);

            entity.HasOne(d => d.MaPhongNavigation).WithMany(p => p.PhongAnhs)
                .HasForeignKey(d => d.MaPhong)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__PhongAnh__MaPhon__5D2BD0E6");
        });

        modelBuilder.Entity<ThanhToan>(entity =>
        {
            entity.HasKey(e => e.MaThanhToan).HasName("PK__ThanhToa__D4B25844504CACF5");

            entity.ToTable("ThanhToan");

            entity.Property(e => e.MaThanhToan).HasMaxLength(20);
            entity.Property(e => e.MaHoaDon).HasMaxLength(20);
            entity.Property(e => e.NgayThanhToan)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.PhuongThucThanhToan).HasMaxLength(50);
            entity.Property(e => e.SoTienThanhToan).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.TinhTrangThanhToan).HasMaxLength(1);

            entity.HasOne(d => d.MaHoaDonNavigation).WithMany(p => p.ThanhToans)
                .HasForeignKey(d => d.MaHoaDon)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ThanhToan__MaHoa__10AB74EC");
        });

        modelBuilder.Entity<TienNghi>(entity =>
        {
            entity.HasKey(e => e.MaTienNghi).HasName("PK__TienNghi__ED7B8F4DC1501450");

            entity.ToTable("TienNghi");

            entity.Property(e => e.MaTienNghi).HasMaxLength(20);
            entity.Property(e => e.MoTa).HasMaxLength(500);
            entity.Property(e => e.TenTienNghi).HasMaxLength(100);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
