-- Bảng NguoiDung
CREATE TABLE NguoiDung (
    MaNguoiDung NVARCHAR(6) PRIMARY KEY,        -- Mã định danh người dùng tự động tăng
    Vaitro NVARCHAR(6) NOT NULL DEFAULT 'KhachHang', -- Vai trò mặc định là Khách hàng
    Email NVARCHAR(100) UNIQUE NOT NULL,             -- Email (đăng nhập)
    TenTaiKhoan NVARCHAR(50) NULL,                   -- Tên tài khoản (đăng nhập, có thể NULL nếu dùng Google/Facebook)
    MatKhau NVARCHAR(255) NULL,                      -- Mật khẩu (NULL nếu dùng Google/Facebook)
    HoTen NVARCHAR(50) NULL,                         -- Họ và tên
    CanCuocCongDan INT UNIQUE NULL,          -- Số CCCD
    SoDienThoai INT UNIQUE NULL,             -- Số điện thoại
    DiaChi NVARCHAR(100) NULL,                       -- Địa chỉ
    NgaySinh DATE NULL,                              -- Ngày sinh
    GioiTinh NVARCHAR(10) NULL,                      -- Giới tính
    HinhAnhURL NVARCHAR(255) NOT NULL DEFAULT 'https://i.pinimg.com/736x/20/ef/6b/20ef6b554ea249790281e6677abc4160.jpg', -- URL ảnh mặc định
    NgayTao DATETIME DEFAULT GETDATE()               -- Ngày tạo
);


ALTER TABLE NguoiDung
ALTER COLUMN CanCuocCongDan NVARCHAR(20);  -- hoặc NVARCHAR(12) nếu chắc chắn là CCCD Việt Nam

ALTER TABLE NguoiDung
ALTER COLUMN SoDienThoai NVARCHAR(15);     -- đủ dài để chứa số điện thoại Việt Nam, kể cả mã quốc gia

ALTER TABLE NguoiDung
ALTER COLUMN Vaitro NVARCHAR(15); 

ALTER TABLE NguoiDung DROP CONSTRAINT UQ__NguoiDun__9A7D3DD7ACBA3F05;
ALTER TABLE NguoiDung ALTER COLUMN CanCuocCongDan NVARCHAR(20);
ALTER TABLE NguoiDung ADD CONSTRAINT UQ_CanCuocCongDan UNIQUE (CanCuocCongDan);

ALTER TABLE NguoiDung DROP CONSTRAINT UQ__NguoiDun__0389B7BD3800E751; -- Cho SoDienThoai
ALTER TABLE NguoiDung ALTER COLUMN SoDienThoai NVARCHAR(15);
ALTER TABLE NguoiDung ADD CONSTRAINT UQ_SoDienThoai UNIQUE (SoDienThoai);

select * from NguoiDung

INSERT INTO NguoiDung (
    MaNguoiDung, Vaitro, Email, TenTaiKhoan, MatKhau, HoTen,
    CanCuocCongDan, SoDienThoai, DiaChi, NgaySinh, GioiTinh
)
VALUES (
    'ND002', 'QuanTriVien', 'admin@example.com', 'admin01', 'admin123',
    N'Nguyễn Quản Trị', 123456789, 912345678, N'Hà Nội', '1990-01-01', N'Nam'
);

INSERT INTO NguoiDung (
    MaNguoiDung, Vaitro, Email, TenTaiKhoan, MatKhau, HoTen,
    CanCuocCongDan, SoDienThoai, DiaChi, NgaySinh, GioiTinh
)
VALUES (
    'ND004', 'NhanVien', 'admin@xample.com', 'admin01', 'admin123',
    N'Nguyễn Quản Trị', 123456786, 9123459678, N'Hà Nội', '1990-01-01', N'Nam'
); 



INSERT INTO NguoiDung (
    MaNguoiDung, Vaitro, Email, TenTaiKhoan, MatKhau, HoTen,
    CanCuocCongDan, SoDienThoai, DiaChi, NgaySinh, GioiTinh
)
VALUES (
    'ND005', 'NhanVien', 'admisn@xample.com', '11111', '11111',
    N'Nguyễn Quản Trị', 1234586243, 9512345678, N'Hà Nội', '1990-01-01', N'Nam'
); 
-- Bảng Phong
CREATE TABLE Phong (
    MaPhong NVARCHAR(6) PRIMARY KEY,           -- Mã phòng tự động tăng
    LoaiPhong NVARCHAR(50) NOT NULL,                 -- Loại phòng
    GiaPhong DECIMAL(18,2) NOT NULL,                 -- Giá phòng gốc
    TinhTrang TINYINT CHECK (TinhTrang IN (1, 2, 3, 4)) NOT NULL, -- 1: Trống, 2: Đã đặt, 3: Đang sử dụng, 4: Đang vệ sinh
    SoLuongPhong INT NOT NULL DEFAULT 1,             -- Số lượng phòng
    Tang INT NOT NULL,                               -- Tầng
    KieuGiuong NVARCHAR(50) NOT NULL,                -- Kiểu giường
    MoTa NVARCHAR(500) NULL,                         -- Mô tả chi tiết về phòng
    UrlAnhChinh NVARCHAR(255) NOT NULL,              -- URL ảnh chính
    SucChua INT NOT NULL DEFAULT 2,                  -- Sức chứa tối đa
    SoGiuong INT NOT NULL DEFAULT 1,                 -- Số lượng giường
    DonViTinh NVARCHAR(50) NOT NULL DEFAULT '1 đêm', -- Đơn vị tính giá phòng
    SoSaoTrungBinh DECIMAL(3,2) NOT NULL DEFAULT 0   -- Số sao trung bình
);

-- Bảng PhongAnh
CREATE TABLE PhongAnh (
    MaAnh NVARCHAR(6) PRIMARY KEY,             -- Mã ảnh tự động tăng
    MaPhong NVARCHAR(6) NOT NULL,                            -- Mã phòng liên kết
    UrlAnh NVARCHAR(255) NOT NULL,                   -- URL ảnh
    FOREIGN KEY (MaPhong) REFERENCES Phong(MaPhong) ON DELETE CASCADE -- Ràng buộc với bảng Phong
);

-- Bảng TienNghi
CREATE TABLE TienNghi (
    MaTienNghi NVARCHAR(6) PRIMARY KEY,        -- Mã tiện nghi tự động tăng
    TenTienNghi NVARCHAR(100) NOT NULL,             -- Tên tiện nghi
    MoTa NVARCHAR(500) NULL                         -- Mô tả chi tiết về tiện nghi (nếu cần)
);

-- Bảng Phong_TienNghi
CREATE TABLE Phong_TienNghi (
    MaPhong NVARCHAR(6) NOT NULL,                           -- Mã phòng
    MaTienNghi NVARCHAR(6) NOT NULL,                        -- Mã tiện nghi
    FOREIGN KEY (MaPhong) REFERENCES Phong(MaPhong) ON DELETE CASCADE, 
    FOREIGN KEY (MaTienNghi) REFERENCES TienNghi(MaTienNghi) ON DELETE CASCADE,
    PRIMARY KEY (MaPhong, MaTienNghi)               -- Chỉ định khóa chính cho cặp mã
);

-- Bảng GiamGia
CREATE TABLE GiamGia (
    MaGiamGia NVARCHAR(6) PRIMARY KEY,         -- Mã giảm giá tự động tăng
    TenGiamGia NVARCHAR(50) NOT NULL,                -- Tên chương trình giảm giá
    LoaiGiamGia NVARCHAR(6) CHECK (LoaiGiamGia IN ('PhanTram', 'SoTien')), -- Loại giảm giá
    GiaTriGiam DECIMAL(18,2) NOT NULL,               -- Giá trị giảm
    NgayBatDau DATETIME NOT NULL,                    -- Ngày bắt đầu áp dụng giảm giá
    NgayKetThuc DATETIME NOT NULL,                   -- Ngày kết thúc áp dụng giảm giá
    MoTa NVARCHAR(255) NULL                          -- Mô tả chi tiết về chương trình giảm giá
);

-- Bảng Phong_GiamGia
CREATE TABLE Phong_GiamGia (
    MaPhong NVARCHAR(6) NOT NULL,                            -- Mã phòng
    MaGiamGia NVARCHAR(6) NOT NULL,                          -- Mã giảm giá
    PRIMARY KEY (MaPhong, MaGiamGia),                -- Chỉ định khóa chính cho cặp mã
    FOREIGN KEY (MaPhong) REFERENCES Phong(MaPhong),
    FOREIGN KEY (MaGiamGia) REFERENCES GiamGia(MaGiamGia)
);

-- Bảng DichVu
CREATE TABLE DichVu (
    MaDichVu NVARCHAR(6) PRIMARY KEY,         -- Mã dịch vụ tự động tăng
    TenDichVu NVARCHAR(100) NOT NULL,              -- Tên dịch vụ
    DonGia DECIMAL(18,2) NOT NULL,                 -- Đơn giá
    MoTaDichVu NVARCHAR(300) NULL,                 -- Mô tả dịch vụ
    HinhAnhDichVu NVARCHAR(255) NOT NULL,          -- URL ảnh dịch vụ
    SoLuong INT NOT NULL DEFAULT 0,                -- Số lượng dịch vụ
    TrangThai TINYINT NOT NULL DEFAULT 1,          -- Trạng thái dịch vụ
    LoaiDichVu NVARCHAR(50) NOT NULL DEFAULT 'Khac', -- Loại dịch vụ
    DonViTinh NVARCHAR(50) NOT NULL DEFAULT '1 lần', -- Đơn vị tính giá dịch vụ
    CONSTRAINT CK_DichVu_TrangThai CHECK (TrangThai IN (0, 1)), -- Kiểm tra trạng thái hợp lệ
    CONSTRAINT CK_DichVu_SoLuong CHECK (SoLuong >= 0) -- Kiểm tra số lượng không âm
);

-- Bảng DatPhong
CREATE TABLE DatPhong (
    MaDatPhong NVARCHAR(6) PRIMARY KEY,       -- Mã đặt phòng tự động tăng
    MaNguoiDung NVARCHAR(6) NOT NULL,                       -- Mã người dùng
    MaPhong NVARCHAR(6) NOT NULL,                           -- Mã phòng
    NgayDat DATETIME DEFAULT GETDATE(),             -- Ngày đặt
    NgayCheckIn DATETIME,                           -- Ngày check-in
    NgayCheckOut DATETIME,                          -- Ngày check-out
    TinhTrangDatPhong TINYINT CHECK (TinhTrangDatPhong IN (1, 2, 3)) NOT NULL, -- 1: Đã xác nhận, 2: Đang chờ, 3: Hủy
    FOREIGN KEY (MaNguoiDung) REFERENCES NguoiDung(MaNguoiDung),
    FOREIGN KEY (MaPhong) REFERENCES Phong(MaPhong)
);

-- Bảng DatDichVu
CREATE TABLE DatDichVu (
    MaDatDichVu NVARCHAR(6) PRIMARY KEY,      -- Mã đặt dịch vụ tự động tăng
    MaDatPhong NVARCHAR(6) NOT NULL,                        -- Mã đặt phòng
    MaDichVu NVARCHAR(6) NOT NULL,                          -- Mã dịch vụ
    SoLuong INT NOT NULL DEFAULT 1,                 -- Số lượng dịch vụ
    MaHoaDon NVarchar(6) NULL,                              -- Mã hóa đơn liên kết
    FOREIGN KEY (MaDatPhong) REFERENCES DatPhong(MaDatPhong),
    FOREIGN KEY (MaDichVu) REFERENCES DichVu(MaDichVu),

);

-- Xóa ràng buộc FK nếu chưa đặt tên, cần lấy tên bằng cách truy vấn (xem phía dưới nếu cần)
ALTER TABLE DatDichVu
DROP CONSTRAINT FK__DatDichVu__MaHoa__0371755F;  -- Tên này có thể khác, cần thay đúng tên

-- Xóa cột
ALTER TABLE DatDichVu
DROP COLUMN MaHoaDon;

SELECT name 
FROM sys.foreign_keys 
WHERE parent_object_id = OBJECT_ID('DatDichVu');

-- Bảng HoaDon
CREATE TABLE HoaDon (
    MaHoaDon NVARCHAR(6) PRIMARY KEY,         -- Mã hóa đơn tự động tăng
    MaNguoiDung NVARCHAR(6) NOT NULL,                       -- Mã người dùng
    MaDatPhong NVARCHAR(6) NOT NULL,                        -- Mã đặt phòng
    TongTien DECIMAL(18,2) NOT NULL,                -- Tổng tiền phải trả
    NgayTaoHoaDon DATETIME DEFAULT GETDATE(),       -- Ngày tạo hóa đơn
    NgayThanhToan DATETIME NULL,                    -- Ngày thanh toán
    TinhTrangHoaDon TINYINT CHECK (TinhTrangHoaDon IN (1, 2)) NOT NULL, -- 1: Đã thanh toán, 2: Chưa thanh toán
    FOREIGN KEY (MaNguoiDung) REFERENCES NguoiDung(MaNguoiDung),
    FOREIGN KEY (MaDatPhong) REFERENCES DatPhong(MaDatPhong)
);

-- Bảng ChiTietHoaDon
CREATE TABLE ChiTietHoaDon (
    MaChiTiet NVARCHAR(6) PRIMARY KEY,               -- Mã chi tiết hóa đơn
    MaHoaDon NVARCHAR(6) NOT NULL,                  -- Mã hóa đơn
    LoaiKhoanMuc NVARCHAR(12) NOT NULL,              -- Loại khoản mục (Phong, DichVu, GiamGia)
    MaKhoanMuc NVARCHAR(12) NOT NULL,                -- Mã khoản mục (MaPhong, MaDichVu, MaGiamGia)
    SoLuong INT NOT NULL DEFAULT 1,                  -- Số lượng
    ThanhTien DECIMAL(18,2) NOT NULL,                -- Thành tiền
    FOREIGN KEY (MaHoaDon) REFERENCES HoaDon(MaHoaDon)
);

-- Bảng ThanhToan
CREATE TABLE ThanhToan (
    MaThanhToan NVARCHAR(6) PRIMARY KEY,             -- Mã thanh toán với tiền tố 'TT'
    MaHoaDon NVARCHAR(6) NOT NULL,                   -- Mã hóa đơn
    SoTienThanhToan DECIMAL(18,2) NOT NULL,           -- Số tiền thanh toán
    NgayThanhToan DATETIME DEFAULT GETDATE(),         -- Ngày thanh toán
    PhuongThucThanhToan NVARCHAR(50) NOT NULL,        -- Phương thức thanh toán
    TinhTrangThanhToan NVARCHAR(1) CHECK (TinhTrangThanhToan IN ('1', '2')) NOT NULL, -- 1: Hoàn tất, 2: Chưa hoàn tất
    FOREIGN KEY (MaHoaDon) REFERENCES HoaDon(MaHoaDon)
);

-- Bảng Feedback
CREATE TABLE Feedback (
    MaFeedback NVARCHAR(6) PRIMARY KEY,              -- Mã phản hồi với tiền tố 'FB'
    MaPhong NVARCHAR(6) NOT NULL,                    -- Mã phòng
    MaNguoiDung NVARCHAR(6) NOT NULL,                -- Mã người dùng
    SoSao INT CHECK (SoSao BETWEEN 1 AND 5) NOT NULL, -- Số sao (1-5 sao)
    BinhLuan NVARCHAR(500) NULL,                     -- Bình luận của người dùng
    NgayFeedback DATETIME DEFAULT GETDATE(),          -- Ngày tạo phản hồi
    FOREIGN KEY (MaPhong) REFERENCES Phong(MaPhong),
    FOREIGN KEY (MaNguoiDung) REFERENCES NguoiDung(MaNguoiDung)
);

ALTER TABLE Feedback
ADD PhanLoai NVARCHAR(10) NOT NULL
    CHECK (PhanLoai IN ('TichCuc', 'TieuCuc'));

-- Bảng NhanVien
CREATE TABLE NhanVien (
    MaNhanVien NVARCHAR(6) PRIMARY KEY,             -- Mã nhân viên
    MaNguoiDung NVARCHAR(6) NOT NULL,               -- Mã người dùng
    ChucVu NVARCHAR(50) NOT NULL,                    -- Chức vụ
    Luong DECIMAL(18,2) NOT NULL,                    -- Lương
    NgayVaoLam DATETIME DEFAULT GETDATE(),           -- Ngày vào làm
    CaLamViec NVARCHAR(50) NULL,                     -- Ca làm việc (Sáng, Chiều, Tối)
    FOREIGN KEY (MaNguoiDung) REFERENCES NguoiDung(MaNguoiDung)
);

-- Bảng LichSuGiaoDich
CREATE TABLE LichSuGiaoDich (
    MaGiaoDich NVARCHAR(6) PRIMARY KEY,             -- Mã giao dịch
    MaNguoiDung NVARCHAR(6) NOT NULL,               -- Mã người dùng
    LoaiGiaoDich NVARCHAR(50) NOT NULL,              -- Loại giao dịch (DatPhong, HuyPhong, ThanhToan, ...)
    ThoiGianGiaoDich DATETIME DEFAULT GETDATE(),     -- Thời gian giao dịch
    MoTa NVARCHAR(255) NULL,                         -- Mô tả giao dịch
    FOREIGN KEY (MaNguoiDung) REFERENCES NguoiDung(MaNguoiDung)
);

-- Bảng BaoCao
CREATE TABLE BaoCao (
    MaBaoCao NVARCHAR(6) PRIMARY KEY,               -- Mã báo cáo
    LoaiBaoCao NVARCHAR(50) NOT NULL,                -- Loại báo cáo (DoanhThu, SuDungPhong, ...)
    ThoiGian DATETIME DEFAULT GETDATE(),             -- Thời gian tạo báo cáo
    NoiDung TEXT NULL                                -- Nội dung báo cáo
);

-- Bảng ChiTietBaoCao
CREATE TABLE ChiTietBaoCao (
    MaChiTiet NVARCHAR(6) PRIMARY KEY,               -- Mã chi tiết báo cáo
    MaBaoCao NVARCHAR(6) NOT NULL,                  -- Mã báo cáo
    NoiDungChiTiet NVARCHAR(255) NOT NULL,           -- Nội dung chi tiết
    GiaTri DECIMAL(18,2) NOT NULL,                   -- Giá trị (ví dụ: doanh thu, tỷ lệ sử dụng)
    FOREIGN KEY (MaBaoCao) REFERENCES BaoCao(MaBaoCao)
);



DROP TABLE NhanVien;
SELECT p.MaPhong, p.LoaiPhong, p.GiaPhong, p.TinhTrang, p.SoLuongPhong, p.Tang, 
       p.KieuGiuong, p.MoTa, p.UrlAnhChinh, p.SucChua, p.SoGiuong, 
       p.DonViTinh, p.SoSaoTrungBinh
FROM Phong p


 SELECT tn.MaTienNghi, tn.TenTienNghi, tn.MoTa
FROM TienNghi tn
JOIN Phong_TienNghi ptn ON tn.MaTienNghi = ptn.MaTienNghi
WHERE ptn.MaPhong = 'P001'
SELECT gg.MaGiamGia, gg.TenGiamGia, gg.LoaiGiamGia, gg.GiaTriGiam, gg.NgayBatDau, gg.NgayKetThuc, gg.MoTa
FROM GiamGia gg
JOIN Phong_GiamGia pg ON gg.MaGiamGia = pg.MaGiamGia
WHERE pg.MaPhong = 'P001'

Select* from Phong