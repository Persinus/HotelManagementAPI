-- Bảng Nhân Viên
-- Bảng Người Dùng (Lưu thông tin đăng nhập và phân quyền trực tiếp)
CREATE TABLE NguoiDung (
    MaNguoiDung NVARCHAR(10) PRIMARY KEY, -- Mã định danh người dùng
    Email NVARCHAR(100) UNIQUE NOT NULL, -- Email
    TenTaiKhoan NVARCHAR(50) NOT NULL,   -- Tên tài khoản
    MatKhau NVARCHAR(255) NOT NULL,      -- Mật khẩu
    VaiTro NVARCHAR(20) NOT NULL CHECK (VaiTro IN ('KhachHang', 'NhanVien', 'QuanTriVien')), -- Phân quyền
    JWK NVARCHAR(MAX) NULL,              -- JSON Web Key (tùy chọn)
    NgayTao DATETIME DEFAULT GETDATE(),  -- Ngày tạo bản ghi
    NgayCapNhat DATETIME DEFAULT GETDATE() -- Ngày cập nhật
);

-- Bảng Khách Hàng (Chi tiết riêng cho khách hàng)
CREATE TABLE KhachHang (
    MaKhachHang NVARCHAR(10) PRIMARY KEY, -- Mã định danh khách hàng
    MaNguoiDung NVARCHAR(10) NOT NULL UNIQUE, -- Liên kết với bảng NguoiDung
    HoTen NVARCHAR(50) NOT NULL,         -- Họ và tên
    CanCuocCongDan NVARCHAR(20) NOT NULL UNIQUE, -- Số CCCD
    SoDienThoai NVARCHAR(15) NOT NULL UNIQUE,    -- Số điện thoại
    DiaChi NVARCHAR(100),               -- Địa chỉ
    NgaySinh DATE,                      -- Ngày sinh
    GioiTinh NVARCHAR(10),              -- Giới tính
    NgheNghiep NVARCHAR(50),            -- Nghề nghiệp
    TrangThai NVARCHAR(20) DEFAULT 'Active', -- Trạng thái
    NgayTao DATETIME DEFAULT GETDATE(), -- Ngày tạo
    NgayCapNhat DATETIME DEFAULT GETDATE(), -- Ngày cập nhật
    HinhAnh NVARCHAR(255),              -- Đường dẫn hình ảnh
    CONSTRAINT FK_KhachHang_NguoiDung FOREIGN KEY (MaNguoiDung) REFERENCES NguoiDung(MaNguoiDung)
);

-- Bảng Nhân Viên (Chi tiết riêng cho nhân viên)
CREATE TABLE NhanVien (
    MaNhanVien NVARCHAR(10) PRIMARY KEY, -- Mã định danh nhân viên
    MaNguoiDung NVARCHAR(10) NOT NULL UNIQUE, -- Liên kết với bảng NguoiDung
    HoTen NVARCHAR(50) NOT NULL,         -- Họ và tên
    SoDienThoai NVARCHAR(15) NOT NULL UNIQUE, -- Số điện thoại
    CanCuocCongDan NVARCHAR(20) UNIQUE NOT NULL, -- Số CCCD
    HinhAnh NVARCHAR(255),              -- Đường dẫn hình ảnh
    NgayVaoLam DATE NOT NULL,           -- Ngày bắt đầu làm việc
    TrangThai NVARCHAR(20) DEFAULT 'Active', -- Trạng thái
    CONSTRAINT FK_NhanVien_NguoiDung FOREIGN KEY (MaNguoiDung) REFERENCES NguoiDung(MaNguoiDung)
);

-- Bảng Quản Trị Viên (Chi tiết riêng cho quản trị viên)
CREATE TABLE QuanTriVien (
    MaQuanTri NVARCHAR(10) PRIMARY KEY, -- Mã định danh quản trị viên
    MaNguoiDung NVARCHAR(10) NOT NULL UNIQUE, -- Liên kết với bảng NguoiDung
    TenAdmin NVARCHAR(50) NOT NULL,     -- Tên quản trị viên
    SoDienThoai NVARCHAR(15) NOT NULL UNIQUE, -- Số điện thoại
    NgayTao DATETIME DEFAULT GETDATE(), -- Ngày tạo bản ghi
    NgayCapNhat DATETIME DEFAULT GETDATE(), -- Ngày cập nhật
    CONSTRAINT FK_QuanTri_NguoiDung FOREIGN KEY (MaNguoiDung) REFERENCES NguoiDung(MaNguoiDung)
);

-- Cập nhật bảng Phong để chứa URL ảnh
CREATE TABLE PhongWithTienNghi (
    MaPhong NVARCHAR(10) PRIMARY KEY,
    LoaiPhong NVARCHAR(50) NOT NULL,
    GiaPhong DECIMAL(18,2) NOT NULL,
    TinhTrang NVARCHAR(1) CHECK (TinhTrang IN ('1', '2', '3', '4')) NOT NULL, -- 1: Trống, 2: Đã đặt, 3: Đang sử dụng, 4: Đang vệ sinh
    SoLuongPhong INT NOT NULL DEFAULT 1,
    Tang INT NOT NULL,
    KieuGiuong NVARCHAR(50) NOT NULL,
    MoTa NVARCHAR(500) NULL, -- Mô tả chi tiết về tiện nghi, view, diện tích...
    UrlAnhChinh NVARCHAR(255) NULL, -- URL ảnh chính của phòng
    UrlAnhPhu1 NVARCHAR(255) NULL, -- URL ảnh phụ 1
    UrlAnhPhu2 NVARCHAR(255) NULL, -- URL ảnh phụ 2
    TienNghi NVARCHAR(MAX) NULL -- Mảng tiện nghi dưới dạng chuỗi JSON (hoặc bạn có thể lưu tiện nghi dưới dạng các chuỗi liên kết với dấu phân cách)
);

CREATE TABLE DatPhong (
    MaDatPhong NVARCHAR(10) PRIMARY KEY,
    MaKhachHang NVARCHAR(10) NOT NULL,
    MaNhanVien NVARCHAR(10) NOT NULL,
    MaPhong NVARCHAR(10) NOT NULL,
    NgayDat DATETIME NOT NULL DEFAULT GETDATE(),
    NgayNhanPhong DATETIME NOT NULL,
    NgayTraPhong DATETIME NOT NULL,
    TrangThai NVARCHAR(1) CHECK (TrangThai IN ('1', '2')) NOT NULL, -- 1: Đặt, 2: Hủy
    FOREIGN KEY (MaKhachHang) REFERENCES KhachHang(MaKhachHang),
    FOREIGN KEY (MaNhanVien) REFERENCES NhanVien(MaNhanVien),
    FOREIGN KEY (MaPhong) REFERENCES PhongWithTienNghi(MaPhong)
);


-- Cập nhật bảng DichVu để chứa URL ảnh
CREATE TABLE DichVu (
    MaChiTietDichVu NVARCHAR(10) PRIMARY KEY,
    TenDichVu NVARCHAR(100) NOT NULL,
    DonGia DECIMAL(18,2) NOT NULL,
    MoTaDichVu NVARCHAR(300) NULL, -- Mô tả chi tiết dịch vụ
    UrlAnh NVARCHAR(255) NULL -- URL ảnh của dịch vụ
);




CREATE TABLE KhachHang_DichVu (
    MaKhachHang NVARCHAR(10),
    MaChiTietDichVu NVARCHAR(10),
    SoLuong INT NOT NULL DEFAULT 1,  -- Thêm cột SoLuong vào bảng
    PRIMARY KEY (MaKhachHang, MaChiTietDichVu),
    FOREIGN KEY (MaKhachHang) REFERENCES KhachHang(MaKhachHang),
    FOREIGN KEY (MaChiTietDichVu) REFERENCES DichVu(MaChiTietDichVu)
);

CREATE TABLE HoaDonThanhToanPhong (
    MaHoaDon NVARCHAR(10) PRIMARY KEY,
    MaDatPhong NVARCHAR(10) NOT NULL,
    MaNhanVien NVARCHAR(10) NOT NULL,
    NgayLapHoaDon DATETIME NOT NULL DEFAULT GETDATE(),
    TongTien DECIMAL(18,2) NOT NULL CHECK (TongTien >= 0),
    TrangThaiThanhToan NVARCHAR(1) CHECK (TrangThaiThanhToan IN ('0', '1')) NOT NULL,
    HinhThucThanhToan INT CHECK (HinhThucThanhToan IN (1, 2)) NOT NULL, -- 1: Tiền mặt, 2: Thẻ
    FOREIGN KEY (MaDatPhong) REFERENCES DatPhong(MaDatPhong),
    FOREIGN KEY (MaNhanVien) REFERENCES NhanVien(MaNhanVien)
);
CREATE TABLE HoaDonThanhToanDichVu (
    MaHoaDonDichVu NVARCHAR(10) PRIMARY KEY,
    MaKhachHang NVARCHAR(10),
    MaChiTietDichVu NVARCHAR(10),
	 TrangThaiThanhToan NVARCHAR(1) CHECK (TrangThaiThanhToan IN ('0', '1')) NOT NULL,
    SoLuong INT NOT NULL,
    ThanhTien DECIMAL(18, 2) NOT NULL, -- Thay vì computed column, thêm như một cột thông thường
    NgayLapHoaDon DATETIME NOT NULL DEFAULT GETDATE(),
    FOREIGN KEY (MaKhachHang) REFERENCES KhachHang(MaKhachHang),
    FOREIGN KEY (MaChiTietDichVu) REFERENCES DichVu(MaChiTietDichVu)
);

ALTER TABLE DatPhong
ADD CONSTRAINT CHK_NgayNhan_Truoc_NgayTra CHECK (NgayNhanPhong <= NgayTraPhong);

INSERT INTO DichVu (MaChiTietDichVu, TenDichVu, DonGia, MoTaDichVu, UrlAnh) VALUES
('DV001', N'Nhà hàng Á – Âu', 50000, N'Phục vụ buffet sáng, trưa, tối đa dạng món ăn', N'https://example.com/images/dv001.jpg'),
('DV002', N'Quầy bar & lounge', 80000, N'Thư giãn với cocktail, view biển về đêm', N'https://example.com/images/dv002.jpg'),
('DV003', N'Spa & massage', 150000, N'Liệu trình trị liệu, thải độc, thư giãn toàn thân', N'https://example.com/images/dv003.jpg'),
('DV004', N'Hồ bơi vô cực', 200000, N'Ngoài trời, view biển, có phục vụ đồ uống', N'https://example.com/images/dv004.jpg'),
('DV005', N'Phòng gym', 0, N'Trang bị đầy đủ máy tập, mở cửa 24/7', N'https://example.com/images/dv005.jpg'),
('DV006', N'Sân chơi trẻ em', 0, N'Khu vui chơi an toàn, nhân viên trông trẻ', N'https://example.com/images/dv006.jpg'),
('DV007', N'Dịch vụ tour', 100000, N'Tour khám phá Đà Nẵng, Hội An, Bà Nà Hills', N'https://example.com/images/dv007.jpg'),
('DV008', N'Xe đưa đón sân bay', 300000, N'Shuttle/limousine riêng có điều hòa', N'https://example.com/images/dv008.jpg'),
('DV009', N'Dịch vụ giữ hành lý', 0, N'Miễn phí trước & sau check-in/out', N'https://example.com/images/dv009.jpg'),
('DV010', N'Khu vực BBQ ngoài trời', 250000, N'Có thể đặt tiệc riêng, hướng dẫn viên nướng', N'https://example.com/images/dv010.jpg'),
('DV011', N'Trung tâm hội nghị', 400000, N'Phòng họp, hội thảo với máy chiếu, âm thanh', N'https://example.com/images/dv011.jpg'),
('DV012', N'Coffee & Bakery', 60000, N'Cà phê, bánh ngọt, trà chiều', N'https://example.com/images/dv012.jpg'),
('DV013', N'Dịch vụ giặt ủi', 90000, N'Có nhân viên lấy và trả tận phòng', N'https://example.com/images/dv013.jpg'),
('DV014', N'Mini mart / Shop quà lưu niệm', 0, N'Đặc sản Đà Nẵng, đồ dùng du lịch', N'https://example.com/images/dv014.jpg'),
('DV015', N'Thuê xe máy/ô tô', 500000, N'Có bảo hiểm, giao xe tận nơi', N'https://example.com/images/dv015.jpg'),
('DV016', N'Dịch vụ chụp ảnh chuyên nghiệp', 300000, N'Gói ảnh cưới, ảnh du lịch tại khách sạn', N'https://example.com/images/dv016.jpg'),
('DV017', N'Yoga sáng trên bãi biển', 0, N'Lớp học hàng ngày, miễn phí cho khách', N'https://example.com/images/dv017.jpg'),
('DV018', N'Dịch vụ trông trẻ', 150000, N'Có người chăm sóc chuyên nghiệp theo giờ', N'https://example.com/images/dv018.jpg'),
('DV019', N'Wifi & Internet tốc độ cao khu vực chung', 0, N'Ở sảnh, nhà hàng, hồ bơi', N'https://example.com/images/dv019.jpg'),
('DV020', N'Dịch vụ y tế – hỗ trợ khẩn cấp 24/7', 0, N'Có nhân viên sơ cứu, gọi bác sĩ đến nơi', N'https://example.com/images/dv020.jpg');
INSERT INTO PhongWithTienNghi (
    MaPhong, LoaiPhong, GiaPhong, TinhTrang, SoLuongPhong, Tang, KieuGiuong, MoTa, 
    UrlAnhChinh, UrlAnhPhu1, UrlAnhPhu2, TienNghi
) VALUES
-- STANDARD
('P201', N'Standard', 500000, '1', 1, 2, N'2 Giường đơn',
 N'View phố, Wifi, điều hòa, bộ trà/cà phê, diện tích 20m2.',
 'http://example.com/standard_p201_main.jpg',
 'http://example.com/standard_p201_1.jpg',
 'http://example.com/standard_p201_2.jpg',
 N'["Máy lạnh", "Wifi miễn phí", "Bộ trà/cà phê"]'),

('P202', N'Standard', 520000, '1', 1, 2, N'1 Giường đôi',
 N'View phố, Wifi, điều hòa, bộ trà/cà phê, diện tích 20m2.',
 'http://example.com/standard_p202_main.jpg',
 'http://example.com/standard_p202_1.jpg',
 'http://example.com/standard_p202_2.jpg',
 N'["Máy lạnh", "Wifi miễn phí", "Bộ trà/cà phê"]'),

('P301', N'Standard', 550000, '1', 1, 3, N'2 Giường đơn',
 N'View phố, Wifi, điều hòa, bộ trà/cà phê, diện tích 20m2.',
 'http://example.com/standard_p301_main.jpg',
 'http://example.com/standard_p301_1.jpg',
 'http://example.com/standard_p301_2.jpg',
 N'["Máy lạnh", "Wifi miễn phí", "Bộ trà/cà phê"]'),

('P302', N'Standard', 570000, '1', 1, 3, N'1 Giường đôi',
 N'View phố, Wifi, điều hòa, bộ trà/cà phê, diện tích 20m2.',
 'http://example.com/standard_p302_main.jpg',
 'http://example.com/standard_p302_1.jpg',
 'http://example.com/standard_p302_2.jpg',
 N'["Máy lạnh", "Wifi miễn phí", "Bộ trà/cà phê"]'),

('P401', N'Standard', 600000, '1', 1, 4, N'2 Giường đơn',
 N'View phố, Wifi, điều hòa, bộ trà/cà phê, diện tích 20m2.',
 'http://example.com/standard_p401_main.jpg',
 'http://example.com/standard_p401_1.jpg',
 'http://example.com/standard_p401_2.jpg',
 N'["Máy lạnh", "Wifi miễn phí", "Bộ trà/cà phê"]'),

('P502', N'Standard', 630000, '1', 1, 5, N'1 Giường đôi',
 N'View phố, Wifi, điều hòa, bộ trà/cà phê, diện tích 20m2.',
 'http://example.com/standard_p502_main.jpg',
 'http://example.com/standard_p502_1.jpg',
 'http://example.com/standard_p502_2.jpg',
 N'["Máy lạnh", "Wifi miễn phí", "Bộ trà/cà phê"]'),

-- DELUXE
('P203', N'Deluxe', 800000, '1', 1, 2, N'1 Giường đôi',
 N'View sân vườn, Tivi 55 inch, minibar, bộ trà/cà phê, diện tích 30m2.',
 'http://example.com/deluxe_p203_main.jpg',
 'http://example.com/deluxe_p203_1.jpg',
 'http://example.com/deluxe_p203_2.jpg',
 N'["Máy lạnh", "Tivi", "Wifi miễn phí", "Minibar", "Bộ trà/cà phê", "Tivi 55 inch"]'),

('P204', N'Deluxe', 850000, '1', 1, 2, N'1 Giường đôi',
 N'View hồ, Tivi 55 inch, minibar, bộ trà/cà phê, diện tích 30m2.',
 'http://example.com/deluxe_p204_main.jpg',
 'http://example.com/deluxe_p204_1.jpg',
 'http://example.com/deluxe_p204_2.jpg',
 N'["Máy lạnh", "Tivi", "Wifi miễn phí", "Minibar", "Bộ trà/cà phê", "Tivi 55 inch"]'),

 -- DELUXE (tiếp)
('P303', N'Deluxe', 880000, '1', 1, 3, N'1 Giường đôi',
 N'View hồ, Tivi 55 inch, minibar, bộ trà/cà phê, diện tích 30m2.',
 'http://example.com/deluxe_p303_main.jpg',
 'http://example.com/deluxe_p303_1.jpg',
 'http://example.com/deluxe_p303_2.jpg',
 N'["Máy lạnh", "Tivi", "Wifi miễn phí", "Minibar", "Bộ trà/cà phê", "Tivi 55 inch"]'),

('P304', N'Deluxe', 840000, '1', 1, 3, N'1 Giường đôi',
 N'View sân vườn, Tivi 55 inch, minibar, bộ trà/cà phê, diện tích 30m2.',
 'http://example.com/deluxe_p304_main.jpg',
 'http://example.com/deluxe_p304_1.jpg',
 'http://example.com/deluxe_p304_2.jpg',
 N'["Máy lạnh", "Tivi", "Wifi miễn phí", "Minibar", "Bộ trà/cà phê", "Tivi 55 inch"]'),

('P403', N'Deluxe', 880000, '1', 1, 4, N'1 Giường đôi',
 N'View sân vườn, Tivi 55 inch, minibar, bộ trà/cà phê, diện tích 30m2.',
 'http://example.com/deluxe_p403_main.jpg',
 'http://example.com/deluxe_p403_1.jpg',
 'http://example.com/deluxe_p403_2.jpg',
 N'["Máy lạnh", "Tivi", "Wifi miễn phí", "Minibar", "Bộ trà/cà phê", "Tivi 55 inch"]'),

('P404', N'Deluxe', 900000, '1', 1, 4, N'1 Giường đôi',
 N'View hồ, Tivi 55 inch, minibar, bộ trà/cà phê, diện tích 30m2.',
 'http://example.com/deluxe_p404_main.jpg',
 'http://example.com/deluxe_p404_1.jpg',
 'http://example.com/deluxe_p404_2.jpg',
 N'["Máy lạnh", "Tivi", "Wifi miễn phí", "Minibar", "Bộ trà/cà phê", "Tivi 55 inch"]'),

('P503', N'Deluxe', 950000, '1', 1, 5, N'1 Giường đôi',
 N'View hồ, Tivi 55 inch, minibar, bộ trà/cà phê, diện tích 30m2.',
 'http://example.com/deluxe_p503_main.jpg',
 'http://example.com/deluxe_p503_1.jpg',
 'http://example.com/deluxe_p503_2.jpg',
 N'["Máy lạnh", "Tivi", "Wifi miễn phí", "Minibar", "Bộ trà/cà phê", "Tivi 55 inch"]'),

('P504', N'Deluxe', 920000, '1', 1, 5, N'1 Giường đôi',
 N'View sân vườn, Tivi 55 inch, minibar, bộ trà/cà phê, diện tích 30m2.',
 'http://example.com/deluxe_p504_main.jpg',
 'http://example.com/deluxe_p504_1.jpg',
 'http://example.com/deluxe_p504_2.jpg',
 N'["Máy lạnh", "Tivi", "Wifi miễn phí", "Minibar", "Bộ trà/cà phê", "Tivi 55 inch"]'),

-- SUITE
('P205', N'Suite', 1500000, '1', 1, 2, N'1 Giường đôi',
 N'View toàn cảnh, Sofa, máy pha cà phê, bồn tắm, minibar, diện tích 40m2.',
 'http://example.com/suite_p205_main.jpg',
 'http://example.com/suite_p205_1.jpg',
 'http://example.com/suite_p205_2.jpg',
 N'["Máy lạnh", "Wifi miễn phí", "Minibar", "Bồn tắm", "Bộ trà/cà phê", "Sofa"]'),

('P305', N'Suite', 1550000, '1', 1, 3, N'1 Giường đôi',
 N'View toàn cảnh, Sofa, máy pha cà phê, bồn tắm, minibar, diện tích 40m2.',
 'http://example.com/suite_p305_main.jpg',
 'http://example.com/suite_p305_1.jpg',
 'http://example.com/suite_p305_2.jpg',
 N'["Máy lạnh", "Wifi miễn phí", "Minibar", "Bồn tắm", "Bộ trà/cà phê", "Sofa"]'),

('P405', N'Suite', 1600000, '1', 1, 4, N'1 Giường đôi',
 N'View toàn cảnh, Sofa, máy pha cà phê, bồn tắm, minibar, diện tích 40m2.',
 'http://example.com/suite_p405_main.jpg',
 'http://example.com/suite_p405_1.jpg',
 'http://example.com/suite_p405_2.jpg',
 N'["Máy lạnh", "Wifi miễn phí", "Minibar", "Bồn tắm", "Bộ trà/cà phê", "Sofa"]'),

('P505', N'Suite', 1700000, '1', 1, 5, N'1 Giường đôi',
 N'View toàn cảnh, Sofa, máy pha cà phê, bồn tắm, minibar, diện tích 40m2.',
 'http://example.com/suite_p505_main.jpg',
 'http://example.com/suite_p505_1.jpg',
 'http://example.com/suite_p505_2.jpg',
 N'["Máy lạnh", "Wifi miễn phí", "Minibar", "Bồn tắm", "Bộ trà/cà phê", "Sofa"]'),

-- FAMILY
('P402', N'Family', 250000, '1', 1, 4, N'6 giường tầng',
 N'Không gian chung, Wifi, diện tích 50m2.',
 'http://example.com/family_p402_main.jpg',
 'http://example.com/family_p402_1.jpg',
 'http://example.com/family_p402_2.jpg',
 N'["Wifi miễn phí", "Không gian chung"]'),

('P501', N'Family', 280000, '1', 1, 5, N'6 giường tầng',
 N'Không gian chung, Wifi, diện tích 50m2.',
 'http://example.com/family_p501_main.jpg',
 'http://example.com/family_p501_1.jpg',
 'http://example.com/family_p501_2.jpg',
 N'["Wifi miễn phí", "Không gian chung"]');





INSERT INTO NhanVien (MaNhanVien, MaNguoiDung, HoTen, SoDienThoai, CanCuocCongDan, HinhAnh, NgayVaoLam, TrangThai) VALUES
('NV001', 'ND002', N'Nguyễn Văn Hùng', '0912345678', '012345670001', 'https://i.pinimg.com/474x/dc/76/f3/dc76f3e81a73381bb74ec87ce073613b.jpg', GETDATE(), 'Active'),
('NV002', 'ND003', N'Trần Thị Bích', '0912345679', '012345670002', 'https://i.pinimg.com/474x/a7/06/97/a706974a50a70be9ab3c7d630599da15.jpg', GETDATE(), 'Active'),
('NV003', 'ND004', N'Lê Minh Tuấn', '0912345680', '012345670003', 'https://i.pinimg.com/736x/7d/53/d2/7d53d2b42fc13fad132acadc5a52bca0.jpg', GETDATE(), 'Active'),
('NV004', 'ND005', N'Phạm Thị Hạnh', '0912345681', '012345670004', 'https://i.pinimg.com/474x/3c/72/d4/3c72d4c55bb7ddfcd16dfca975e2dd67.jpg', GETDATE(), 'Active'),
('NV005', 'ND006', N'Nguyễn Thị Hồng', '0912345682', '012345670005', 'https://i.pinimg.com/474x/77/ec/77/77ec779699be6d7963604b3eb50ebc7a.jpg', GETDATE(), 'Active'),
('NV006', 'ND007', N'Hoàng Thị Hường', '0912345683', '012345670006', 'https://i.pinimg.com/474x/36/75/d8/3675d883ab6b3cb72b2e53aafdf4cc4b.jpg', GETDATE(), 'Active'),
('NV007', 'ND008', N'Phạm Minh Đức', '0912345684', '012345670007', 'https://i.pinimg.com/474x/2f/4f/22/2f4f229c6639f4689901e7730f908410.jpg', GETDATE(), 'Active'),
('NV008', 'ND009', N'Nguyễn Thị Lan', '0912345685', '012345670008', 'https://i.pinimg.com/474x/3c/4a/8c/3c4a8ca54cb120b009e43468fc35672f.jpg', GETDATE(), 'Active'),
('NV009', 'ND010', N'Trần Minh Khoa', '0912345686', '012345670009', 'https://i.pinimg.com/474x/5b/68/b6/5b68b651a1ffbb1ebd379a8e44e1143e.jpg', GETDATE(), 'Active'),
('NV010', 'ND011', N'Lê Thị Nhung', '0912345687', '012345670010', 'https://i.pinimg.com/474x/a1/de/24/a1de243c7f90cf7d923d5a9f76ad852c.jpg', GETDATE(), 'Active'),
('NV011', 'ND012', N'Hoàng Thị Yến', '0912345688', '012345670011', 'https://i.pinimg.com/474x/75/12/5a/75125ad408a0bb433e5696d6de22004b.jpg', GETDATE(), 'Active'),
('NV012', 'ND013', N'Nguyễn Minh Quân', '0912345689', '012345670012', 'https://i.pinimg.com/474x/52/bc/b4/52bcb46f3716881be7df0fcb42fc32f9.jpg', GETDATE(), 'Active'),
('NV013', 'ND014', N'Phạm Thị Hoa', '0912345690', '012345670013', 'https://i.pinimg.com/474x/37/0b/a3/370ba36299bfe577601494cca1f1a59f.jpg', GETDATE(), 'Active'),
('NV014', 'ND015', N'Lê Thị Trang', '0912345691', '012345670014', 'https://i.pinimg.com/474x/c6/1e/05/c61e05276285083e83ae1327e271092f.jpg', GETDATE(), 'Active'),
('NV015', 'ND016', N'Nguyễn Thị Mai', '0912345692', '012345670015', 'https://i.pinimg.com/474x/6f/86/92/6f86927e6374c5e6a839dfa640b88976.jpg', GETDATE(), 'Active');
-- 1. INSERT INTO KhachHang
INSERT INTO KhachHang (MaKhachHang, MaNguoiDung, HoTen, CanCuocCongDan, SoDienThoai, DiaChi, NgaySinh, GioiTinh, NgheNghiep)
VALUES
('KH001', 'ND017', N'Nguyễn Văn An', '031245678900', '0912345678', N'123 Đường ABC, Quận 1, TP.HCM', '1990-01-01', N'Nam', N'Kỹ sư cầu đường'),
('KH002', 'ND018', N'Trần Thị Bảo Ngọc', '031245678901', '0912345679', N'456 Đường DEF, Quận 2, TP.HCM', '1992-02-02', N'Nữ', N'Giáo viên trung học'),
('KH003', 'ND019', N'Lê Minh Cường', '031245678902', '0912345680', N'789 Đường GHI, Quận 3, TP.HCM', '1988-03-03', N'Nam', N'Bác sĩ nội trú');


INSERT INTO DatPhong (MaDatPhong, MaKhachHang, MaNhanVien, MaPhong, NgayDat, NgayNhanPhong, NgayTraPhong, TrangThai)
VALUES
('DP008', 'KH002', 
    (SELECT TOP 1 MaNhanVien FROM NhanVien ORDER BY NEWID()), 
    'P205', GETDATE(), '2025-04-21 14:00:00', '2025-04-22 12:00:00', '1'),
('DP001', 'KH001', 
    (SELECT TOP 1 MaNhanVien FROM NhanVien ORDER BY NEWID()), 
    'P201', GETDATE(), '2025-04-20 14:00:00', '2025-04-22 12:00:00', '1'),
('DP002', 'KH002', 
    (SELECT TOP 1 MaNhanVien FROM NhanVien ORDER BY NEWID()), 
    'P202', GETDATE(), '2025-04-18 15:00:00', '2025-04-19 11:00:00', '1'),
('DP003', 'KH001', 
    (SELECT TOP 1 MaNhanVien FROM NhanVien ORDER BY NEWID()), 
    'P203', GETDATE(), '2025-04-25 13:00:00', '2025-04-28 12:00:00', '2'); -- đã hủy

iNSERT INTO HoaDonThanhToanDichVu (MaHoaDonDichVu, MaKhachHang, MaChiTietDichVu, SoLuong, ThanhTien, TrangThaiThanhToan)
SELECT 
    'HD001',  -- Chúng ta sẽ liên kết hóa đơn này
    'KH001', 
    'DV001', 
    2,  
    d.DonGia * 2 AS ThanhTien,
    '1' -- Đã thanh toán
FROM 
    DichVu d
WHERE 
    d.MaChiTietDichVu = 'DV001';



INSERT INTO KhachHang_DichVu (MaKhachHang, MaChiTietDichVu, SoLuong) VALUES
('KH001', 'DV001', 2),  -- Khách hàng KH001 sử dụng dịch vụ Nhà hàng Á – Âu với số lượng 2
('KH001', 'DV002', 1),  -- Khách hàng KH001 sử dụng dịch vụ Quầy bar & lounge với số lượng 1
('KH002', 'DV003', 1),  -- Khách hàng KH002 sử dụng dịch vụ Spa & massage với số lượng 1
('KH003', 'DV004', 1);  -- Khách hàng KH003 sử dụng dịch vụ Hồ bơi vô cực với số lượng 1





INSERT INTO HoaDonThanhToanPhong (MaHoaDon, MaDatPhong, MaNhanVien, NgayLapHoaDon, TongTien, TrangThaiThanhToan, HinhThucThanhToan)
VALUES
(
    'HD001', 
    'DP001', 
    'NV001', 
    GETDATE(), 
    (SELECT DATEDIFF(DAY, NgayNhanPhong, NgayTraPhong) * GiaPhong 
     FROM DatPhong dp 
     JOIN PhongWithTienNghi p ON dp.MaPhong = p.MaPhong 
     WHERE dp.MaDatPhong = 'DP001'),
    '1', -- Đã thanh toán
    1   -- Tiền mặt
);

-- 1. INSERT INTO QuanTriVien
INSERT INTO QuanTriVien (MaQuanTri, MaNguoiDung, TenAdmin, SoDienThoai) VALUES
('QT001', 'ND001', N'Nguyễn Văn Mạnh', '0912345678');





-- 4. INSERT INTO NguoiDung
INSERT INTO NguoiDung (MaNguoiDung, Email, TenTaiKhoan, MatKhau, VaiTro, JWK) VALUES
-- Quản trị viên
('ND001', 'admin@example.com', 'admin', 'hashed_password_qtv', 'QuanTriVien', NULL),

-- Nhân viên ND002 - ND016
('ND002', 'nv1@example.com', 'nv1', 'hashed_pw_1', 'NhanVien', NULL),
('ND003', 'nv2@example.com', 'nv2', 'hashed_pw_2', 'NhanVien', NULL),
('ND004', 'nv3@example.com', 'nv3', 'hashed_pw_3', 'NhanVien', NULL),
('ND005', 'nv4@example.com', 'nv4', 'hashed_pw_4', 'NhanVien', NULL),
('ND006', 'nv5@example.com', 'nv5', 'hashed_pw_5', 'NhanVien', NULL),
('ND007', 'nv6@example.com', 'nv6', 'hashed_pw_6', 'NhanVien', NULL),
('ND008', 'nv7@example.com', 'nv7', 'hashed_pw_7', 'NhanVien', NULL),
('ND009', 'nv8@example.com', 'nv8', 'hashed_pw_8', 'NhanVien', NULL),
('ND010', 'nv9@example.com', 'nv9', 'hashed_pw_9', 'NhanVien', NULL),
('ND011', 'nv10@example.com', 'nv10', 'hashed_pw_10', 'NhanVien', NULL),
('ND012', 'nv11@example.com', 'nv11', 'hashed_pw_11', 'NhanVien', NULL),
('ND013', 'nv12@example.com', 'nv12', 'hashed_pw_12', 'NhanVien', NULL),
('ND014', 'nv13@example.com', 'nv13', 'hashed_pw_13', 'NhanVien', NULL),
('ND015', 'nv14@example.com', 'nv14', 'hashed_pw_14', 'NhanVien', NULL),
('ND016', 'nv15@example.com', 'nv15', 'hashed_pw_15', 'NhanVien', NULL),

-- Khách hàng ND017 - ND019
('ND017', 'kh1@example.com', 'kh1', 'hashed_pw_kh1', 'KhachHang', NULL),
('ND018', 'kh2@example.com', 'kh2', 'hashed_pw_kh2', 'KhachHang', NULL),
('ND019', 'kh3@example.com', 'kh3', 'hashed_pw_kh3', 'KhachHang', NULL);

INSERT INTO PhongAnh (MaPhong, UrlAnh, LoaiAnh) VALUES
-- Phòng P201
('P201', 'https://caominhhotel.vn/wp-content/uploads/sites/103/2023/07/do%CC%9Bn-thu%CC%9Bo%CC%9B%CC%80ng1-1-scaled.jpg', 'Chinh'),
('P201', 'https://caominhhotel.vn/wp-content/uploads/sites/103/2023/07/do%CC%9Bn-3-1-scaled.jpg', 'Phu1'),
('P201', 'https://caominhhotel.vn/wp-content/uploads/sites/103/2023/07/do%CC%9Bn-2-1-scaled.jpg', 'Phu2'),

-- Phòng P202
('P202', 'https://caominhhotel.vn/wp-content/uploads/sites/103/2023/07/Do%CC%82i-01-1-scaled.jpg', 'Chinh'),
('P202', 'https://caominhhotel.vn/wp-content/uploads/sites/103/2023/07/Do%CC%82i-02-1-scaled.jpg', 'Phu1'),
('P202', 'https://caominhhotel.vn/wp-content/uploads/sites/103/2023/07/Do%CC%82i-02-1-scaled.jpg', 'Phu2'),

-- Phòng P203
('P203', 'http://example.com/p203-main.jpg', 'Chinh'),
('P203', 'http://example.com/p203-sub1.jpg', 'Phu1'),
('P203', 'http://example.com/p203-sub2.jpg', 'Phu2'),

-- Phòng P204
('P204', 'http://example.com/p204-main.jpg', 'Chinh'),
('P204', 'http://example.com/p204-sub1.jpg', 'Phu1'),
('P204', 'http://example.com/p204-sub2.jpg', 'Phu2'),

-- Phòng P205
('P205', 'http://example.com/p205-main.jpg', 'Chinh'),
('P205', 'http://example.com/p205-sub1.jpg', 'Phu1'),
('P205', 'http://example.com/p205-sub2.jpg', 'Phu2'),

-- Phòng P301
('P301', 'http://example.com/p301-main.jpg', 'Chinh'),
('P301', 'http://example.com/p301-sub1.jpg', 'Phu1'),
('P301', 'http://example.com/p301-sub2.jpg', 'Phu2'),

-- Phòng P302
('P302', 'http://example.com/p302-main.jpg', 'Chinh'),
('P302', 'http://example.com/p302-sub1.jpg', 'Phu1'),
('P302', 'http://example.com/p302-sub2.jpg', 'Phu2'),

-- Phòng P303
('P303', 'http://example.com/p302-main.jpg', 'Chinh'),
('P303', 'http://example.com/p302-sub1.jpg', 'Phu1'),
('P303', 'http://example.com/p302-sub2.jpg', 'Phu2'),

-- Phòng P304
('P304', 'http://example.com/p302-main.jpg', 'Chinh'),
('P304', 'http://example.com/p302-sub1.jpg', 'Phu1'),
('P304', 'http://example.com/p302-sub2.jpg', 'Phu2'),

-- Phòng P305
('P305', 'http://example.com/p302-main.jpg', 'Chinh'),
('P305', 'http://example.com/p302-sub1.jpg', 'Phu1'),
('P305', 'http://example.com/p302-sub2.jpg', 'Phu2'),

-- Phòng P401
('P401', 'http://example.com/p401-main.jpg', 'Chinh'),
('P401', 'http://example.com/p401-sub1.jpg', 'Phu1'),
('P401', 'http://example.com/p401-sub2.jpg', 'Phu2'),

-- Phòng P402
('P402', 'http://example.com/p402-main.jpg', 'Chinh'),
('P402', 'http://example.com/p402-sub1.jpg', 'Phu1'),
('P402', 'http://example.com/p402-sub2.jpg', 'Phu2'),

-- Phòng P403
('P403', 'http://example.com/p402-main.jpg', 'Chinh'),
('P403', 'http://example.com/p402-sub1.jpg', 'Phu1'),
('P403', 'http://example.com/p402-sub2.jpg', 'Phu2'),

-- Phòng P404
('P404', 'http://example.com/p402-main.jpg', 'Chinh'),
('P404', 'http://example.com/p402-sub1.jpg', 'Phu1'),
('P404', 'http://example.com/p402-sub2.jpg', 'Phu2'),

-- Phòng P405
('P405', 'http://example.com/p402-main.jpg', 'Chinh'),
('P405', 'http://example.com/p402-sub1.jpg', 'Phu1'),
('P405', 'http://example.com/p402-sub2.jpg', 'Phu2'),

-- Phòng P501
('P501', 'http://example.com/p501-main.jpg', 'Chinh'),
('P501', 'http://example.com/p501-sub1.jpg', 'Phu1'),
('P501', 'http://example.com/p501-sub2.jpg', 'Phu2'),



-- Phòng P502
('P502', 'http://example.com/p502-main.jpg', 'Chinh'),
('P502', 'http://example.com/p502-sub1.jpg', 'Phu1'),
('P502', 'http://example.com/p502-sub2.jpg', 'Phu2'),

-- Phòng P503
('P503', 'http://example.com/p502-main.jpg', 'Chinh'),
('P503', 'http://example.com/p502-sub1.jpg', 'Phu1'),
('P503', 'http://example.com/p502-sub2.jpg', 'Phu2'),

-- Phòng P504
('P504', 'http://example.com/p502-main.jpg', 'Chinh'),
('P504', 'http://example.com/p502-sub1.jpg', 'Phu1'),
('P504', 'http://example.com/p502-sub2.jpg', 'Phu2'),

-- Phòng P505
('P505', 'http://example.com/p502-main.jpg', 'Chinh'),
('P505', 'http://example.com/p502-sub1.jpg', 'Phu1'),
('P505', 'http://example.com/p502-sub2.jpg', 'Phu2');


INSERT INTO DichVuAnh (MaChiTietDichVu, UrlAnh) VALUES
('DV001', 'http://example.com/dv001.jpg'), -- Nhà hàng Á – Âu
('DV002', 'http://example.com/dv002.jpg'), -- Quầy bar & lounge
('DV003', 'http://example.com/dv003.jpg'), -- Spa & massage
('DV004', 'http://example.com/dv004.jpg'), -- Hồ bơi vô cực
('DV005', 'http://example.com/dv005.jpg'), -- Phòng gym
('DV006', 'http://example.com/dv006.jpg'), -- Sân chơi trẻ em
('DV007', 'http://example.com/dv007.jpg'), -- Dịch vụ tour
('DV008', 'http://example.com/dv008.jpg'), -- Xe đưa đón sân bay
('DV009', 'http://example.com/dv009.jpg'), -- Dịch vụ giữ hành lý
('DV010', 'http://example.com/dv010.jpg'), -- Khu vực BBQ ngoài trời
('DV011', 'http://example.com/dv011.jpg'), -- Trung tâm hội nghị
('DV012', 'http://example.com/dv012.jpg'), -- Coffee & Bakery
('DV013', 'http://example.com/dv013.jpg'), -- Dịch vụ giặt ủi
('DV014', 'http://example.com/dv014.jpg'), -- Mini mart / Shop quà lưu niệm
('DV015', 'http://example.com/dv015.jpg'), -- Thuê xe máy/ô tô
('DV016', 'http://example.com/dv016.jpg'), -- Dịch vụ chụp ảnh chuyên nghiệp
('DV017', 'http://example.com/dv017.jpg'), -- Yoga sáng trên bãi biển
('DV018', 'http://example.com/dv018.jpg'), -- Dịch vụ trông trẻ
('DV019', 'http://example.com/dv019.jpg'), -- Wifi & Internet tốc độ cao khu vực chung
('DV020', 'http://example.com/dv020.jpg'); -- Dịch vụ y tế – hỗ trợ khẩn cấp 24/7


SELECT * FROM NhanVien;
SELECT * FROM QuanTrivien;
SELECT * FROM NguoiDung;
SELECT * FROM KhachHang;
Select * from PhongWithTienNghi
SELECT * FROM DatPhong;
SELECT * FROM DichVu;
Select * From KhachHang_DichVu;
SELECT * FROM HoaDonThanhToanPhong;
SELECT * FROM HoaDonThanhToanDichVu;
